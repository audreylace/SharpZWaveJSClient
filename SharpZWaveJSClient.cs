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

		#region Class State Variables

		#region Private Class State Variables
		private BroadcastBlock<ISharpZWaveJSClientEvent> EventsBroadcaster { get; set; }
		private SemaphoreSlim SendSemaphore { get; set; }
		private ClientWebSocket Socket { get; set; }
		private CancellationTokenSource ShutdownTokenSource { get; set; }
		#endregion

		#region Public Class State Variables
		public SharpZWaveJSConnectionState State { get; private set; }
		public List<RemoteServerEventParserBase> RemoteServerEventConverters { get; private set; }
		public int MaxMessageSize { get; set; }
		public IServerVersionInfo ServerInfo { get; private set; }
		public bool ReceivingEvents { get; set; }
		#endregion

		#endregion

		public SharpZWaveJSClient()
		{
			EventsBroadcaster = new BroadcastBlock<ISharpZWaveJSClientEvent>((ISharpZWaveJSClientEvent e) => throw new NotImplementedException());
			RemoteServerEventConverters = new List<RemoteServerEventParserBase>();
			Socket = new ClientWebSocket();
			MaxMessageSize = 1024 * 1024; //1 Megabyte
			SendSemaphore = new SemaphoreSlim(1, 1);

			ResetAllState();
		}


		#region Public API

		public IDisposable LinkTo(ITargetBlock<ISharpZWaveJSClientEvent> targetBlock, DataflowLinkOptions linkOptions)
		{
			return EventsBroadcaster.LinkTo(targetBlock, linkOptions);
		}
		public IDisposable LinkTo(ITargetBlock<ISharpZWaveJSClientEvent> targetBlock)
		{
			return EventsBroadcaster.LinkTo(targetBlock);
		}


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

				using (System.IO.MemoryStream stream = await ReceiveWebsocketMessage(cancellationToken))
				using (JsonDocument document = await JsonDocument.ParseAsync(stream, default(JsonDocumentOptions), cancellationToken))
				{
					if (document.RootElement.TryGetProperty("type", out var typeProperty))
					{
						string typeValue = typeProperty.GetString();
						if (typeValue != "version")
						{
							new HandshakeException("Expected result message in reply to start_listening command.");
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

						Task recieveAndDispatchTask = ReceiveAndDispatchMessages();
					}
				}
			}
			catch (Exception)
			{
				ResetAllState();
				throw;
			}
		}
		public async Task<ICommandResultEvent> SendCommandAsync(IZWaveJSCommand command, CancellationToken cancellationToken)
		{
			ThrowIfStateNotOpen();

			CancellationToken token = GetJoinedToken(cancellationToken).Token;

			string messageId = command.MessageId;

			SemaphoreSlim done = new SemaphoreSlim(0, 1);
			ICommandResultEvent commandResult = null;

			ActionBlock<ISharpZWaveJSClientEvent> messageActionBlock = new ActionBlock<ISharpZWaveJSClientEvent>(classEvent =>
			{
				if (classEvent.EventType == SharpZWaveJSClientEventType.ServerCommandResult)
				{
					ICommandResultEvent result = (ICommandResultEvent)classEvent;
					if (result.MessageId == messageId)
					{
						commandResult = result;
						done.Release();
					}
				}
			});

			using (LinkTo(messageActionBlock))
			{
				await SendMessageAsync(command, token);
				await done.WaitAsync(token);

				if (commandResult == null)
				{
					throw new InvalidProgramException("Command Result should have been defined at this point");
				}

				return commandResult;
			}
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
			RecycleConnection();
			ShutdownTokenSource.Cancel();
			ReceivingEvents = false;
		}
		private async Task ReceiveAndDispatchMessages()
		{
			CancellationToken token = ShutdownTokenSource.Token;

			while (Socket.State == WebSocketState.Open && !token.IsCancellationRequested)
			{
				try
				{
					IIncomingMessage message;
					using (System.IO.MemoryStream stream = await ReceiveWebsocketMessage(ShutdownTokenSource.Token))
					{
						JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
						options.Converters.Add(new IncomingMessageConverter());
						message = await JsonSerializer.DeserializeAsync<IIncomingMessage>(stream, options, token);
					}

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

							if (serverEvent != null)
							{
								await EventsBroadcaster.SendAsync(serverEvent);
							}

							break;
						case IncomingMessageType.Result:
							CommandResultEvent resultEvent = new CommandResultEvent((IIncomingResultMessage)message);
							await EventsBroadcaster.SendAsync(resultEvent);
							break;
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

		private void RecycleConnection()
		{
			Socket.Abort();
			Socket.Dispose();

			Socket = new ClientWebSocket();
		}

		private async Task<System.IO.MemoryStream> ReceiveWebsocketMessage(CancellationToken cancellationToken)
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
	}
}
