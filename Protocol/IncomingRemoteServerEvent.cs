using System;
using System.Text.Json;

namespace AudreysCloud.Community.SharpZWaveJSClient.Protocol
{
	public enum RemoteServerEventSource
	{
		Controller,
		Node
	}


	public interface IIncomingRemoteServerEvent : IIncomingMessage
	{
		string EventName { get; }
		RemoteServerEventSource Source { get; }
		string EventPayload { get; }
	}

	internal class IncomingRemoteServerEvent : IncomingMessageBase, IIncomingRemoteServerEvent
	{


		public IncomingRemoteServerEvent() : base(IncomingMessageType.Event) { }

		public IncomingRemoteServerEvent(JsonDocument jsonDocument) : this()
		{
			try
			{
				JsonElement EventNode = jsonDocument.RootElement.GetProperty("event");

				JsonElement SourceNode = EventNode.GetProperty("source");
				JsonElement EventNameNode = EventNode.GetProperty("event");

				EventName = EventNameNode.GetString();
				Source = Enum.Parse<RemoteServerEventSource>(SourceNode.GetString(), true);
				EventPayload = EventNode.GetRawText();

			}
			catch (Exception ex)
			{
				throw new JsonException("Event message parse fail", ex);
			}

		}

		public string EventName { get; set; }

		public RemoteServerEventSource Source { get; set; }

		public string EventPayload { get; set; }
	}


}