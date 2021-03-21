using AudreysCloud.Community.SharpZWaveJSClient.Protocol;

namespace AudreysCloud.Community.SharpZWaveJSClient.Events
{
	public interface ICommandResultEvent : ISharpZWaveJSClientEvent
	{
		bool Success { get; }

		string ErrorCode { get; }

		string JsonResult { get; }
	}

	internal class CommandResultEvent : SharpZWaveJSClientEventBase, ICommandResultEvent
	{
		public CommandResultEvent() : base(SharpZWaveJSClientEventType.ServerCommandResult)
		{
		}
		public CommandResultEvent(IIncomingResultMessage message) : this()
		{
			Success = message.Success;
			ErrorCode = message.ErrorCode;
			JsonResult = message.Result;
		}
		public bool Success { get; internal set; }

		public string ErrorCode { get; internal set; }

		public string JsonResult { get; internal set; }
	}
}