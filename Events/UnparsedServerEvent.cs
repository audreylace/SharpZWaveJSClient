namespace AudreysCloud.Community.SharpZWaveJSClient.Events
{

	public class UnparsedServerEvent : RemoteServerEventBase
	{
		public string RawJson { get; internal set; }
	}
}