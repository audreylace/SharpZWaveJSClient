namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public enum ServerEventSource
	{
		Controller,
		Node
	}
	public interface IServerEvent
	{
		ServerEventSource Source { get; }
		string Event { get; }
	}

	public class ServerEventBase : IServerEvent
	{
		public ServerEventSource Source { get; protected set; }

		public string Event { get; protected set; }
	}
}