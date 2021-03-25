using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AudreysCloud.Community.SharpZWaveJSClient.Exceptions;
using AudreysCloud.Community.SharpZWaveJSClient.Protocol;
using AudreysCloud.Community.SharpZWaveJSClient.Events;

namespace AudreysCloud.Community.SharpZWaveJSClient
{

	public enum SharpZWaveJSConnectionState
	{
		Closed,
		Closing,
		Connecting,
		Open
	}
	public sealed class SharpZWaveJSClient
	{

		private const int MaxSchemaVersion = 3;
		#region Class State Variables

		#region Private Class State Variables
		private SemaphoreSlim SendSemaphore { get; set; }
		private SemaphoreSlim ReceiveSemaphore { get; set; }
		private ClientWebSocket Socket { get; set; }
		private CancellationTokenSource ShutdownTokenSource { get; set; }
		#endregion

		#region Public Class State Variables
		public SharpZWaveJSConnectionState State { get; private set; }
		public List<RemoteServerEventParserBase> RemoteServerEventConverters { get; private set; }
		public int MaxMessageSize { get; set; }
		public IServerVersionInfo ServerInfo { get; private set; }

		#endregion

		#endregion

		public SharpZWaveJSClient()
		{
			RemoteServerEventConverters = new List<RemoteServerEventParserBase>();
			Socket = new ClientWebSocket();
			MaxMessageSize = 1024 * 1024; //1 Megabyte
			SendSemaphore = new SemaphoreSlim(1, 1);
			ReceiveSemaphore = new SemaphoreSlim(1, 1);

			ResetAllState();
		}

		#region Public API
		public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
		{
			if (State != SharpZWaveJSConnectionState.Closed)
			{
				throw new InvalidOperationException();
			}

			State = SharpZWaveJSConnectionState.Connecting;
			try
			{
				await Socket.ConnectAsync(uri, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				using (System.IO.MemoryStream stream = await ReceiveWebsocketMessageAsync(cancellationToken))
				using (JsonDocument document = await JsonDocument.ParseAsync(stream, default(JsonDocumentOptions), cancellationToken))
				{
					if (document.RootElement.TryGetProperty("type", out var typeProperty))
					{
						string typeValue = typeProperty.GetString();
						if (typeValue != "version")
						{
							new HandshakeException("Expected first message to be the version message from the remote server");
						}
						//
						// Free the document since we don't need it now
						//
						document.Dispose();

						//
						// Parse and store the server version info for access by consumers of this class.
						//
						stream.Seek(0, System.IO.SeekOrigin.Begin);
						IIncomingVersionMessage versionMessage = await JsonSerializer.DeserializeAsync<IIncomingVersionMessage>
							(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web), cancellationToken);
						ServerInfo = new ServerVersionInfo(versionMessage);

						State = SharpZWaveJSConnectionState.Open;
						ShutdownTokenSource = new CancellationTokenSource();
					}
				}
			}
			catch (Exception)
			{
				ResetAllState();
				throw;
			}
		}
		public async Task SendCommandAsync(IZWaveJSCommand command, CancellationToken cancellationToken)
		{
			ThrowIfStateNotOpen();

			CancellationToken token = GetJoinedToken(cancellationToken).Token;
			await SendMessageAsync(command, token);

		}

		public async Task CloseAsync(CancellationToken cancellationToken)
		{
			ThrowIfStateNotOpen();

			State = SharpZWaveJSConnectionState.Closing;
			ShutdownTokenSource.Cancel();

			await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close requested", cancellationToken);
			ResetAllState();
		}

		public void Abort()
		{
			ResetAllState();
		}

		#endregion Public API

		private void ThrowIfStateNotOpen()
		{
			ThrowIfWebSocketNotReady();

			if (State != SharpZWaveJSConnectionState.Open)
			{
				throw new InvalidOperationException();
			}

			ShutdownTokenSource.Token.ThrowIfCancellationRequested();
		}

		private void ThrowIfWebSocketNotReady()
		{
			if (Socket.State != WebSocketState.Open)
			{
				throw new InvalidOperationException();
			}
		}

		private CancellationTokenSource GetJoinedToken(CancellationToken cancellationToken)
		{
			return CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ShutdownTokenSource.Token);
		}

		private void ResetAllState()
		{
			ServerInfo = null;
			State = SharpZWaveJSConnectionState.Closed;

			ForceRecycleWebsocketConnection();
			ShutdownTokenSource.Cancel();
		}

		public async Task<ISharpZWaveJSClientEvent> ReceiveMessageAsync(CancellationToken cancellationToken)
		{
			ThrowIfStateNotOpen();

			CancellationTokenSource tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ShutdownTokenSource.Token);
			CancellationToken token = tokenSource.Token;

			using (System.IO.MemoryStream stream = await ReceiveWebsocketMessageAsync(token))
			{
				return await ParseMessage(stream, cancellationToken);
			}
		}

		private async Task<ISharpZWaveJSClientEvent> ParseMessage(System.IO.MemoryStream stream, CancellationToken token)
		{
			try
			{
				IIncomingMessage message;
				JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				options.Converters.Add(new IncomingMessageConverter());
				message = await JsonSerializer.DeserializeAsync<IIncomingMessage>(stream, options, token);


				switch (message.Type)
				{
					case IncomingMessageType.Event:

						RemoteServerEventBase serverEvent = null;
						IIncomingRemoteServerEvent serverEventMessage = (IIncomingRemoteServerEvent)message;
						foreach (RemoteServerEventParserBase parser in RemoteServerEventConverters)
						{
							try
							{
								if (parser.CanParse(serverEventMessage))
								{
									serverEvent = parser.Parse(serverEventMessage);
									break;
								}
							}
							catch (JsonException)
							{
								// Go to next one...
							}
						}

						if (serverEvent == null)
						{
							serverEvent = new UnparsedServerEvent(serverEventMessage);
						}

						return serverEvent;
					case IncomingMessageType.Result:
						return new CommandResultEvent((IIncomingResultMessage)message);
					case IncomingMessageType.Version:
						throw new ProtocolException("Version message received after connection handshake completed. This is not allowed.");
					default:
						throw new NotImplementedException();
				}
			}
			catch (Exception)
			{
				Abort();
				throw;
			}
		}

		private async Task SendMessageAsync(object message, CancellationToken cancellationToken)
		{
			await SendSemaphore.WaitAsync(cancellationToken);
			try
			{
				JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
				await Socket.SendAsync(JsonSerializer.SerializeToUtf8Bytes(message, options),
					  WebSocketMessageType.Text,
					  true,
					  cancellationToken);
			}
			finally
			{
				SendSemaphore.Release();
			}
		}

		private void ForceRecycleWebsocketConnection()
		{
			Socket.Abort();
			Socket.Dispose();

			Socket = new ClientWebSocket();
		}

		private async Task<System.IO.MemoryStream> ReceiveWebsocketMessageAsync(CancellationToken cancellationToken)
		{
			await ReceiveSemaphore.WaitAsync(cancellationToken);
			try
			{
				System.IO.MemoryStream stream = new System.IO.MemoryStream();
				byte[] buffer = new byte[128];
				WebSocketReceiveResult result;
				do
				{
					result = await Socket.ReceiveAsync(buffer, cancellationToken);
					await stream.WriteAsync(buffer, 0, result.Count, cancellationToken);

					if (MaxMessageSize > 0 && stream.Length > MaxMessageSize)
					{
						throw new OverflowException(String.Format("Websocket message exceeds max size of {0}bytes", MaxMessageSize));
					}
				} while (!result.EndOfMessage);

				stream.Seek(0, System.IO.SeekOrigin.Begin);
				return stream;
			}
			finally
			{
				ReceiveSemaphore.Release();
			}
		}
	}
}
