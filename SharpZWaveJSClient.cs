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
		//TODO - Change type to a general purpose event type
		private BroadcastBlock<ISharpZWaveJSClientEvent> EventsBroadcaster { get; set; }

		private ClientWebSocket Socket { get; set; }
		private CancellationTokenSource ShutdownTokenSource { get; set; }


		public SharpZWaveJSConnectionState State
		{ get; private set; }
		public List<JsonConverter<IRemoteServerEvent>> EventConverters { get; private set; }
		public int MaxMessageSize { get; set; }
		public IServerVersionInfo ServerInfo { get; private set; }

		public SharpZWaveJSClient()
		{
			EventsBroadcaster = new BroadcastBlock<ISharpZWaveJSClientEvent>((ISharpZWaveJSClientEvent e) => throw new NotImplementedException());
			EventConverters = new List<JsonConverter<IRemoteServerEvent>>();
			Socket = new ClientWebSocket();
			MaxMessageSize = 1024 * 1024; //1 Megabyte
			State = SharpZWaveJSConnectionState.Closed;
		}
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

				while (Socket.State == WebSocketState.Open)
				{
					cancellationToken.ThrowIfCancellationRequested();

					using (System.IO.MemoryStream stream = await ReceiveWebsocketMessage(cancellationToken))
					using (JsonDocument document = await JsonDocument.ParseAsync(stream, default(JsonDocumentOptions), cancellationToken))
					{
						if (document.RootElement.TryGetProperty("type", out var typeProperty))
						{
							string typeValue = typeProperty.GetString();
							if (typeValue == "version")
							{
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

								//
								// Subscribe to server states and event messages
								//
								string listeningId = await SendCommandMessageAsync("start_listening", null, cancellationToken);
								IIncomingMessage result = await ReceiveMessageWithIdAsync(listeningId, cancellationToken);


								if (result.Type != IncomingMessageType.Result)
								{
									//
									// Attempt to close gracefully because of error and
									// then throw an exception to complete the task.
									//

									HandshakeException exception = new HandshakeException("Expected result message in reply to start_listening command.");
									await Socket.CloseAsync(
											WebSocketCloseStatus.InvalidMessageType,
											exception.Message,
											cancellationToken
										);

									throw exception;
								}

								ParseStartListeningResultMessage(result);

								State = SharpZWaveJSConnectionState.Open;
								ShutdownTokenSource = new CancellationTokenSource();

								// TODO - Start Async task to continue receiving message
								// TODO - Configure data buffer to send message from stuff

								return;

							}
						}
					}
				}
			}
			catch (Exception)
			{
				ResetAllState();
				throw;
			}
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
							//TODO
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
				}
			}
		}

		private void ParseStartListeningResultMessage(IIncomingMessage result)
		{
			throw new NotImplementedException();
		}

		private Task<IIncomingMessage> ReceiveMessageWithIdAsync(string listeningId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		private async Task<string> SendCommandMessageAsync(string command, object message, CancellationToken cancellationToken)
		{
			JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
			options.Converters.Add(new OutgoingCommandMessageConverter());

			OutgoingCommandMessage messageToSend = new OutgoingCommandMessage(command, message);

			await Socket.SendAsync(JsonSerializer.SerializeToUtf8Bytes(messageToSend, options),
				  WebSocketMessageType.Text,
				  true,
				  cancellationToken);

			return messageToSend.MessageId;


		}
		private void RecycleConnection()
		{
			Socket.Abort();
			Socket.Dispose();

			Socket = new ClientWebSocket();
		}

		private void ResetAllState()
		{
			ServerInfo = null;
			State = SharpZWaveJSConnectionState.Closed;
			RecycleConnection();
			ShutdownTokenSource.Cancel();


		}
		public async Task<ICommandResultEvent> SendCommandAsync(string command, object payload, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		public async Task CloseAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		public void Abort()
		{
			ResetAllState();
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
