
using System;
using System.Text.Json;

namespace AudreysCloud.Community.SharpZWaveJSClient.Protocol
{
	public interface IRawResultMessage : IIncomingMessage
	{
		bool Success { get; }
		string ErrorCode { get; }

		string MessageId { get; }
		string Result { get; }
	}

	internal class RawResultMessage : IncomingMessageBase, IRawResultMessage
	{
		public RawResultMessage()
		{
			Type = IncomingMessageType.Result;
		}

		internal RawResultMessage(JsonDocument document) : this()
		{
			try
			{
				JsonElement success = document.RootElement.GetProperty("success");
				JsonElement messageId = document.RootElement.GetProperty("messageId");

				Success = success.GetBoolean();
				MessageId = messageId.GetString();

				if (Success)
				{
					JsonElement result = document.RootElement.GetProperty("result");
					Result = result.GetRawText();
				}
				else
				{
					JsonElement errorCode = document.RootElement.GetProperty("errorCode");
					ErrorCode = errorCode.GetString();
				}
			}
			catch (Exception ex)
			{
				throw new JsonException("Result message parse fail", ex);
			}
		}

		public bool Success { get; private set; }
		public string ErrorCode { get; private set; }

		public string MessageId { get; private set; }

		public string Result { get; private set; }
	}
}