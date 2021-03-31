using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{



	public interface IZWaveNodeEndpointState
	{
		long NodeId { get; }
		long Index { get; }
		string InstallerIcon { get; }
		string UserIcon { get; }

		[SchemaVersion(3)]
		IZWaveNodeDeviceClass DeviceClass { get; }
	}

	public class ZWaveNodeEndpointState : IZWaveNodeEndpointState
	{

		public long NodeId { get; set; }


		public long Index { get; set; }


		public string InstallerIcon { get; set; }


		public string UserIcon { get; set; }

		[SchemaVersion(3)]

		[JsonConverter(typeof(ImplementInterfaceConverter<ZWaveNodeDeviceClass>))]
		public IZWaveNodeDeviceClass DeviceClass { get; set; }

	}
}