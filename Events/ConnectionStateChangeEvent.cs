namespace AudreysCloud.Community.SharpZWaveJSClient.Events
{
	public interface IConnectionStateChangeEvent : ISharpZWaveJSClientEvent
	{
		SharpZWaveJSConnectionState PreviousState { get; }
		SharpZWaveJSConnectionState CurrentState { get; }
	}

	internal class ConnectionStateChangeEvent : SharpZWaveJSClientEventBase, IConnectionStateChangeEvent
	{
		public ConnectionStateChangeEvent(SharpZWaveJSConnectionState previous, SharpZWaveJSConnectionState current) : base(SharpZWaveJSClientEventType.ConnectionStateChange)
		{
			PreviousState = previous;
			CurrentState = current;
		}

		public SharpZWaveJSConnectionState PreviousState { get; private set; }

		public SharpZWaveJSConnectionState CurrentState { get; private set; }
	}
}