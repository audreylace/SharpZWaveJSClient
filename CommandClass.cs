namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface IZWaveCommandClass
	{
		ZWaveCommandClasses Id { get; }
		long Version { get; }

		bool IsSecure { get; }

	}

	public class ZWaveCommandClass : IZWaveCommandClass
	{
		public ZWaveCommandClasses Id { get; set; }

		public long Version { get; set; }

		public bool IsSecure { get; set; }
	}
}