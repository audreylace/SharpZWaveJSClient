using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{



	public interface IZWaveNodeEndpointState
	{
		long NodeId { get; }
		long Index { get; }
		long InstallerIcon { get; }
		long UserIcon { get; }

		[SchemaVersion(3)]
		IZWaveNodeDeviceClass DeviceClass { get; }
	}

	public class ZWaveNodeEndpointState : IZWaveNodeEndpointState
	{

		public long NodeId { get; set; }


		public long Index { get; set; }


		public long InstallerIcon { get; set; }


		public long UserIcon { get; set; }

		[SchemaVersion(3)]

		[JsonConverter(typeof(ImplementInterfaceConverter<IZWaveNodeDeviceClass, ZWaveNodeDeviceClass>))]
		public IZWaveNodeDeviceClass DeviceClass { get; set; }

	}
}