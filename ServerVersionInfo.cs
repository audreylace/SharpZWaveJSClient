using AudreysCloud.Community.SharpZWaveJSClient.Protocol;

namespace AudreysCloud.Community.SharpZWaveJSClient
{

	public interface IServerVersionInfo
	{
		string DriverVersion { get; }
		string ServerVersion { get; }
		long HomeId { get; }

		int SchemaVersion { get; }
	}

	internal class ServerVersionInfo : IServerVersionInfo
	{
		public ServerVersionInfo() { }
		public ServerVersionInfo(IIncomingVersionMessage message, int schemaVersion)
		{
			DriverVersion = message.DriverVersion;
			ServerVersion = message.ServerVersion;
			HomeId = message.HomeId;
			SchemaVersion = schemaVersion;
		}
		public string DriverVersion { get; private set; }
		public string ServerVersion { get; private set; }
		public long HomeId { get; private set; }

		public int SchemaVersion { get; internal set; }
	}
}