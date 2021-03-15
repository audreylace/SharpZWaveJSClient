using System;

namespace AudreysCloud.Community.SharpZWaveJSClient.Protocol
{

	internal class OutgoingCommandMessage
	{
		public OutgoingCommandMessage(string command, object payload)
		{
			MessageId = Guid.NewGuid().ToString();
			Command = command;
			Payload = payload;
		}

		public string Command { get; set; }
		public object Payload { get; set; }
		public string MessageId { get; set; }

	}

}