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
		Connecting,
		Open
	}

	public sealed class SharpZWaveJSClient
	{
		internal BroadcastBlock<IServerEvent> Events { get; private set; }
		internal ClientWebSocket Socket { get; private set; }

		public SharpZWaveJSConnectionState State { get; private set; }

		public List<JsonConverter<IServerEvent>> EventConverters { get; private set; }
		public int MaxMessageSize { get; set; }
		public IServerVersionInfo ServerInfo { get; private set; }

		public SharpZWaveJSClient()
		{
			Events = new BroadcastBlock<IServerEvent>((IServerEvent e) => throw new NotImplementedException());
			EventConverters = new List<JsonConverter<IServerEvent>>();
			Socket = new ClientWebSocket();
			MaxMessageSize = 1024 * 1024; //1 Megabyte
			State = SharpZWaveJSConnectionState.Closed;
		}
		public IDisposable LinkTo(ITargetBlock<IServerEvent> targetBlock, DataflowLinkOptions linkOptions)
		{
			return Events.LinkTo(targetBlock, linkOptions);
		}
		public IDisposable LinkTo(ITargetBlock<IServerEvent> targetBlock)
		{
			return Events.LinkTo(targetBlock);
		}
		public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
		{
			State = SharpZWaveJSConnectionState.Connecting;
			try
			{
				await Socket.ConnectAsync(uri, cancellationToken);

				while (Socket.State == WebSocketState.Open)
				{
					cancellationToken.ThrowIfCancellationRequested();

					using (System.IO.MemoryStream stream = await ReceiveJsonMessage(cancellationToken))
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

								stream.Seek(0, System.IO.SeekOrigin.Begin);
								IVersionMessage versionMessage = await JsonSerializer.DeserializeAsync<IVersionMessage>
									(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web), cancellationToken);

								ServerInfo = new ServerVersionInfo(versionMessage);

								string listeningId = await SendCommandMessageAsync("start_listening", null, cancellationToken);
								IIncomingMessage result = await ReceiveMessageWithIdAsync(listeningId, cancellationToken);

								//
								// Attempt to close gracefully because of error and
								// then throw an exception to complete the task.
								//
								if (result.Type != IncomingMessageType.Result)
								{
									HandshakeException exception = new HandshakeException("Expected result message in reply to start_listening command.");
									await Socket.CloseAsync(
											WebSocketCloseStatus.InvalidMessageType,
											exception.Message,
											cancellationToken
										);

									throw exception;
								}

								// TODO - Parse Start Listening Message

								State = SharpZWaveJSConnectionState.Open;

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
		}
		public async Task<ICommandResult> SendCommandAsync(string command, object payload, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		public async Task CloseAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		public void Abort()
		{
			State = SharpZWaveJSConnectionState.Closed;
			RecycleConnection();
			ServerInfo = null;
		}

		private async Task<System.IO.MemoryStream> ReceiveJsonMessage(CancellationToken cancellationToken)
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
