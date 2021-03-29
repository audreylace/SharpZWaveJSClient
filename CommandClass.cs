namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface ICommandClass
	{
		CommandClasses Id { get; }
		long Version { get; }

		bool IsSecure { get; }

	}
}