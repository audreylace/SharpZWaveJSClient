using System.Net.WebSockets;
using AudreysCloud.Community.SharpZWaveJSClient.Protocol;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface ISharpZWaveJSClientReceiveResult
	{
	}

	public interface IReceiveResultMessage : ISharpZWaveJSClientReceiveResult
	{
		IIncomingMessage Message { get; }
	}

	public interface IReceiveResultConnectionClosed : ISharpZWaveJSClientReceiveResult
	{
		WebSocketCloseStatus? CloseStatus { get; }
		string CloseStatusDescription { get; }
	}

	public interface IReceiveResultParseFailure : ISharpZWaveJSClientReceiveResult
	{
		string WebsocketMessage { get; }
	}


	internal class ReceiveResultMessage : IReceiveResultMessage
	{
		public ReceiveResultMessage(IIncomingMessage message)
		{
			Message = message;
		}
		public IIncomingMessage Message { get; private set; }
	}

	internal class ReceiveResultConnectionClosed : IReceiveResultConnectionClosed
	{
		public ReceiveResultConnectionClosed(WebSocketCloseStatus? closeStatus, string statusDescription)
		{
			CloseStatus = closeStatus;
			CloseStatusDescription = statusDescription;
		}
		public WebSocketCloseStatus? CloseStatus { get; private set; }

		public string CloseStatusDescription { get; private set; }
	}

	internal class ReceiveResultParseFailure : IReceiveResultParseFailure
	{
		public ReceiveResultParseFailure(string message)
		{
			WebsocketMessage = message;
		}
		public string WebsocketMessage { get; private set; }
	}
}