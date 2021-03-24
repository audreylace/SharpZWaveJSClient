using System;

namespace AudreysCloud.Community.SharpZWaveJSClient
{

	public interface IZWaveJSCommand
	{
		string MessageId { get; }
		string Command { get; }
	}

	public class ZWaveJSCommand : IZWaveJSCommand
	{
		public ZWaveJSCommand()
		{
			MessageId = Guid.NewGuid().ToString();
		}
		public ZWaveJSCommand(string command)
		{
			MessageId = Guid.NewGuid().ToString();
			Command = command;
		}
		public ZWaveJSCommand(string command, string messageId)
		{
			MessageId = messageId;
			Command = command;
		}
		public string MessageId { get; set; }
		public string Command { get; set; }
	}
}