namespace AudreysCloud.Community.SharpZWaveJSClient.Protocol
{
	public interface IIncomingVersionMessage : IIncomingMessage
	{
		int MinSchemaVersion { get; }
		int MaxSchemaVersion { get; }
		string DriverVersion { get; }
		string ServerVersion { get; }
		long HomeId { get; }
	}

	internal class IncomingVersionMessage : IncomingMessageBase, IIncomingVersionMessage
	{
		public IncomingVersionMessage() : base(IncomingMessageType.Version)
		{
		}
		public int MinSchemaVersion { get; set; }
		public int MaxSchemaVersion { get; set; }
		public string DriverVersion { get; set; }
		public string ServerVersion { get; set; }
		public long HomeId { get; set; }

	}
}