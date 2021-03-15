using AudreysCloud.Community.SharpZWaveJSClient.Protocol;

namespace AudreysCloud.Community.SharpZWaveJSClient
{

	public interface IServerVersionInfo
	{
		string DriverVersion { get; }
		string ServerVersion { get; }
		long HomeId { get; }
	}

	internal class ServerVersionInfo : IServerVersionInfo
	{
		public ServerVersionInfo() { }
		public ServerVersionInfo(IVersionMessage message)
		{
			DriverVersion = message.DriverVersion;
			ServerVersion = message.ServerVersion;
			HomeId = message.HomeId;
		}
		public string DriverVersion { get; set; }
		public string ServerVersion { get; set; }
		public long HomeId { get; set; }
	}
}