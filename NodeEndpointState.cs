namespace AudreysCloud.Community.SharpZWaveJSClient
{



	public interface INodeEndpointState
	{
		long NodeId { get; }
		long Index { get; }
		string InstallerIcon { get; }

		string UserIcon { get; }

		[SchemaVersion(3)]
		INodeDeviceClass DeviceClass { get; }
	}

}