using AudreysCloud.Community.SharpZWaveJSClient.Protocol;

namespace AudreysCloud.Community.SharpZWaveJSClient.Events
{

	public class UnparsedServerEvent : RemoteServerEventBase
	{

		public UnparsedServerEvent() { }
		internal UnparsedServerEvent(IIncomingRemoteServerEvent remoteEvent)
		{
			RawJson = remoteEvent.EventPayload;
			EventName = remoteEvent.EventName;
			EventSource = remoteEvent.Source;
		}

		public string RawJson { get; internal set; }
	}
}