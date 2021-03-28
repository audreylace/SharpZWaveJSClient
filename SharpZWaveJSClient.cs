using System;
using System.Collections.Generic;
using System.IO;
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
		Aborted,
		Error,
		Open,
		Hijacked
	}

	public sealed class SharpZWaveJSClient
	{

		#region Class State Variables

		#region Private Class State Variables
		private SemaphoreSlim SendSemaphore { get; set; }
		private SemaphoreSlim ReceiveSemaphore { get; set; }
		private CancellationTokenSource ShutdownTokenSource { get; set; }
		#endregion

		#region Public Class State Variables
		public SharpZWaveJSConnectionState State { get; private set; }
		public List<RemoteServerEventParserBase> RemoteServerEventConverters { get; private set; }
		public int MaxMessageSize { get; set; }
		public IServerVersionInfo ServerInfo { get; private set; }
		public int CurrentSchemaVersion { get; private set; }
		public Exception Error { get; private set; }
		public ClientWebSocket Socket { get; private set; }
		#endregion

		#endregion

		public SharpZWaveJSClient()
		{
			RemoteServerEventConverters = new List<RemoteServerEventParserBase>();
			Socket = new ClientWebSocket();
			MaxMessageSize = 1024 * 1024; //1 Megabyte
			SendSemaphore = new SemaphoreSlim(1, 1);
			ReceiveSemaphore = new SemaphoreSlim(1, 1);
			State = SharpZWaveJSConnectionState.Closed;
			ServerInfo = null;
			ShutdownTokenSource = new CancellationTokenSource();
			ShutdownTokenSource.Cancel();
		}

		#region Public API

		public async Task ConnectUsingSocketAsync(ClientWebSocket socket, CancellationToken cancellationToken)
		{
			ThrowIfStateNotClosed();
			Socket.Dispose();

			Socket = socket;

			State = SharpZWaveJSConnectionState.Connecting;
			try
			{
				await NegotiateConnection(cancellationToken);
				ServerVersionInfo info = await NegotiateConnection(cancellationToken);
				SetConnectionStateAsOpen(info);
			}
			catch (Exception ex)
			{
				Abort(ex);
				throw;
			}
		}

		public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
		{
			ThrowIfStateNotClosed();

			State = SharpZWaveJSConnectionState.Connecting;
			try
			{
				await Socket.ConnectAsync(uri, cancellationToken);
				ServerVersionInfo info = await NegotiateConnection(cancellationToken);
				SetConnectionStateAsOpen(info);

			}
			catch (Exception ex)
			{
				Abort(ex);
				throw;
			}
		}

		public WebSocket HijackConnection()
		{
			ThrowIfStateNotOpen();
			State = SharpZWaveJSConnectionState.Hijacked;
			Abort();
			return Socket;
		}

		public async Task SendCommandAsync(IZWaveJSCommand command, CancellationToken cancellationToken)
		{
			ThrowIfStateNotOpen();

			CancellationToken token = GetJoinedToken(cancellationToken).Token;
			await SendMessageAsync(command, token);
		}

		public async Task CloseAsync(CancellationToken cancellationToken)
		{
			if (State == SharpZWaveJSConnectionState.Closed)
			{
				return;
			}

			State = SharpZWaveJSConnectionState.Closing;
			ShutdownTokenSource.Cancel();
			await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close requested", cancellationToken);

			if (Socket.State != WebSocketState.Closed)
			{
				Abort(new Exception(String.Format("Expected Websocket to be closed but instead it was {0}", Socket.State.ToString())));
			}
			State = SharpZWaveJSConnectionState.Closed;
		}

		public void Abort()
		{
			if (State != SharpZWaveJSConnectionState.Hijacked)
			{
				Socket.Abort();
				State = SharpZWaveJSConnectionState.Aborted;
			}

			ShutdownTokenSource.Cancel();
		}

		public async Task<ISharpZWaveJSClientReceiveResult> ReceiveMessageAsync(CancellationToken cancellationToken)
		{

			if (State != SharpZWaveJSConnectionState.Open)
			{
				throw new InvalidOperationException("This operation can only be performed when connection is open");
			}

			if (Socket.State == WebSocketState.Closed || Socket.State == WebSocketState.CloseReceived)
			{
				State = SharpZWaveJSConnectionState.Closing;
				return new ReceiveResultConnectionClosed(Socket.CloseStatus, Socket.CloseStatusDescription);
			}

			CancellationToken token = GetJoinedToken(cancellationToken).Token;

			using (System.IO.MemoryStream stream = await ReceiveWebsocketMessageAsync(token))
			{

				if (Socket.State == WebSocketState.Closed || Socket.State == WebSocketState.CloseReceived)
				{
					State = SharpZWaveJSConnectionState.Closing;
					return new ReceiveResultConnectionClosed(Socket.CloseStatus, Socket.CloseStatusDescription);
				}

				try
				{
					IIncomingMessage message = await ParseMessage(stream, cancellationToken);
					return new ReceiveResultMessage(message);
				}
				catch (JsonException)
				{
					stream.Seek(0, System.IO.SeekOrigin.Begin);
					StreamReader reader = new StreamReader(stream);
					string text = reader.ReadToEnd();
					return new ReceiveResultParseFailure(text);
				}
			}
		}
		#endregion Public API

		private void Abort(Exception ex)
		{
			Abort();
			State = SharpZWaveJSConnectionState.Error;
			Error = ex;
		}

		private async Task<ServerVersionInfo> NegotiateConnection(CancellationToken cancellationToken)
		{
			IIncomingVersionMessage versionInfo;
			using (System.IO.MemoryStream stream = await ReceiveWebsocketMessageAsync(cancellationToken))
			{
				IIncomingMessage message = await ParseMessage(stream, cancellationToken);
				if (message.Type != IncomingMessageType.Version)
				{
					throw new ProtocolException("Expected first message to be the version message from the remote server");
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
					throw new ProtocolException("Expected next message sent back to be result reply message to SetApiSchemaCommand.");
				}

				IIncomingResultMessage resultMessage = (IIncomingResultMessage)message;
				if (resultMessage.MessageId != setApiSchemaCommand.MessageId)
				{
					throw new ProtocolException("Expected next message sent back to be result reply message to SetApiSchemaCommand.");
				}

				if (!resultMessage.Success)
				{
					throw new ProtocolException("Remote server indicated a failure in response to setApiSchemaCommand. Failed to negotiate API schema version so aborting connection.");
				}
			}

			return new ServerVersionInfo(versionInfo, versionToUse);
		}

		private void SetConnectionStateAsOpen(ServerVersionInfo info)
		{
			ServerInfo = info;
			State = SharpZWaveJSConnectionState.Open;
			ShutdownTokenSource = new CancellationTokenSource();
		}

		private void ThrowIfStateNotOpen()
		{
			if (State != SharpZWaveJSConnectionState.Open)
			{
				throw new InvalidOperationException("This operation can only be performed when connection is open");
			}

			ThrowIfWebSocketNotReady();
			ShutdownTokenSource.Token.ThrowIfCancellationRequested();
		}

		private void ThrowIfStateNotClosed()
		{
			if (State != SharpZWaveJSConnectionState.Closed)
			{
				throw new InvalidOperationException("Can only call this method when the state of the connection is closed.");
			}
		}

		private void ThrowIfWebSocketNotReady()
		{
			if (Socket.State != WebSocketState.Open)
			{
				throw new InvalidOperationException("Operation expects web socket to be open.");
			}
		}

		private CancellationTokenSource GetJoinedToken(CancellationToken cancellationToken)
		{
			return CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ShutdownTokenSource.Token);
		}

		private async Task<IIncomingMessage> ParseMessage(System.IO.MemoryStream stream, CancellationToken token)
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

		private int SelectSchemaVersion(IIncomingVersionMessage message)
		{
			if (message.MinSchemaVersion > 3)
			{
				throw new ProtocolException("Remote server miniumium schema level exceeds the level supported by this implementation");
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

					if (result.MessageType == WebSocketMessageType.Close)
					{
						return stream;
					}
					else if (result.MessageType == WebSocketMessageType.Binary)
					{
						throw new ProtocolException("Got binary message from websocket but expected message to be in text format.");
					}
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
