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

		public int CurrentSchemaVersion { get; private set; }

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
				IIncomingVersionMessage versionInfo;

				using (System.IO.MemoryStream stream = await ReceiveWebsocketMessageAsync(cancellationToken))
				{
					IIncomingMessage message = await ParseMessage(stream, cancellationToken);
					if (message.Type != IncomingMessageType.Version)
					{
						throw new HandshakeException("Expected first message to be the version message from the remote server");
					}
					versionInfo = (IIncomingVersionMessage)message;
				}

				int versionToUse = SelectSchemaVersion(versionInfo);

				SetApiSchemaCommand setApiSchemaCommand = new SetApiSchemaCommand(versionToUse);
				await SendMessageAsync(setApiSchemaCommand, cancellationToken);

				using (System.IO.MemoryStream stream = await ReceiveWebsocketMessageAsync(cancellationToken))
				{
					IIncomingMessage message = await ParseMessage(stream, cancellationToken);
					if (message.Type != IncomingMessageType.Result)
					{
						throw new HandshakeException("Expected message sent back from command to be result reply message");
					}

					IIncomingResultMessage resultMessage = (IIncomingResultMessage)message;
					if (resultMessage.MessageId != setApiSchemaCommand.MessageId)
					{
						throw new HandshakeException("Expected message sent back from command to be result reply message for initial set api schema message.");
					}

					if (!resultMessage.Success)
					{
						throw new HandshakeException("Failed to negotiate API schema version.");
					}
				}

				ServerInfo = new ServerVersionInfo(versionInfo, versionToUse);
				State = SharpZWaveJSConnectionState.Open;
				ShutdownTokenSource = new CancellationTokenSource();
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

		public async Task<IIncomingMessage> ReceiveMessageAsync(CancellationToken cancellationToken)
		{
			ThrowIfStateNotOpen();

			CancellationTokenSource tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ShutdownTokenSource.Token);
			CancellationToken token = tokenSource.Token;

			using (System.IO.MemoryStream stream = await ReceiveWebsocketMessageAsync(token))
			{
				return await ParseMessage(stream, cancellationToken);
			}
		}

		private async Task<IIncomingMessage> ParseMessage(System.IO.MemoryStream stream, CancellationToken token)
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
						IIncomingRemoteServerEvent serverEventMessage = (IIncomingRemoteServerEvent)message;
						foreach (RemoteServerEventParserBase parser in RemoteServerEventConverters)
						{
							try
							{
								if (parser.CanParse(serverEventMessage))
								{
									IIncomingRemoteServerEvent parsedMessage = parser.Parse(serverEventMessage);
									return parsedMessage;
								}
							}
							catch (JsonException)
							{
								// Go to next one...
							}
						}

						return serverEventMessage;
					case IncomingMessageType.Result:
						return message;
					case IncomingMessageType.Version:
						if (State != SharpZWaveJSConnectionState.Connecting)
						{
							throw new ProtocolException("Version message received after connection handshake completed. This is not allowed.");
						}
						return message;
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

		private int SelectSchemaVersion(IIncomingVersionMessage message)
		{
			if (message.MinSchemaVersion > 3)
			{
				throw new HandshakeException("Remote server miniumium schema level exceeds the level supported by this implementation");
			}

			return Math.Min(message.MinSchemaVersion, 3);
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
