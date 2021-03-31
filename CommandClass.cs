namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface IZWaveCommandClass
	{
		ZWaveCommandClasses Id { get; }
		long Version { get; }

		bool IsSecure { get; }

	}
}