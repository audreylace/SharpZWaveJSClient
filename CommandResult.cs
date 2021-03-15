using AudreysCloud.Community.SharpZWaveJSClient.Protocol;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface ICommandResult
	{
		bool Success { get; }

		string ErrorCode { get; }

		string JsonResult { get; }
	}

	internal class CommandResult : ICommandResult
	{
		public CommandResult() { }
		public CommandResult(RawResultMessage message)
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