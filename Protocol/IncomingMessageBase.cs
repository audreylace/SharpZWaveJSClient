using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient.Protocol
{
	public enum IncomingMessageType
	{
		Version,
		Result,
		Event,
	}
	public interface IIncomingMessage
	{
		IncomingMessageType Type { get; }
	}

	internal abstract class IncomingMessageBase : IIncomingMessage
	{
		[JsonIgnore]
		public IncomingMessageType Type { get; protected set; }
	}
}