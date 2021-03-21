using AudreysCloud.Community.SharpZWaveJSClient.Protocol;

namespace AudreysCloud.Community.SharpZWaveJSClient.Events
{

	public interface IRemoteServerEvent : ISharpZWaveJSClientEvent
	{
		RemoteServerEventSource EventSource { get; }
		string EventName { get; }
	}

	public abstract class RemoteServerEventBase : SharpZWaveJSClientEventBase, IRemoteServerEvent
	{
		public RemoteServerEventBase() : base(SharpZWaveJSClientEventType.RemoteServerEvent)
		{

		}
		public RemoteServerEventSource EventSource { get; protected set; }

		public string EventName { get; protected set; }
	}
}