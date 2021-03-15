namespace AudreysCloud.Community.SharpZWaveJSClient.Protocol
{
	public interface IVersionMessage : IIncomingMessage
	{
		int MinSchemaVersion { get; }
		int MaxSchemaVersion { get; }
		string DriverVersion { get; }
		string ServerVersion { get; }
		long HomeId { get; }
	}

	internal class VersionMessage : IncomingMessageBase, IVersionMessage
	{
		public VersionMessage()
		{
			Type = IncomingMessageType.Version;
		}
		public int MinSchemaVersion { get; set; }
		public int MaxSchemaVersion { get; set; }
		public string DriverVersion { get; set; }
		public string ServerVersion { get; set; }
		public long HomeId { get; set; }

	}
}