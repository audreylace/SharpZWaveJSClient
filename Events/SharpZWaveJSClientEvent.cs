namespace AudreysCloud.Community.SharpZWaveJSClient.Events
{
	public interface ISharpZWaveJSClientEvent
	{
		SharpZWaveJSClientEventType EventType { get; }
	}

	public enum SharpZWaveJSClientEventType
	{
		RemoteServerEvent,
		ServerCommandResult,
		ConnectionStateChange
	}

	public abstract class SharpZWaveJSClientEventBase : ISharpZWaveJSClientEvent
	{
		internal SharpZWaveJSClientEventBase(SharpZWaveJSClientEventType eventType)
		{
			EventType = eventType;
		}
		public SharpZWaveJSClientEventType EventType { get; protected set; }

	}
}