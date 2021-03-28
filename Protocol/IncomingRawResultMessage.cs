
using System;
using System.Text.Json;

namespace AudreysCloud.Community.SharpZWaveJSClient.Protocol
{
	public interface IIncomingResultMessage : IIncomingMessage
	{
		bool Success { get; }
		string ErrorCode { get; }

		string MessageId { get; }
		string ResultJson { get; }
	}

	internal class IncomingResultMessage : IncomingMessageBase, IIncomingResultMessage
	{
		public IncomingResultMessage() : base(IncomingMessageType.Result)
		{
		}

		internal IncomingResultMessage(JsonDocument document) : this()
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
					ResultJson = result.GetRawText();
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

		public string ResultJson { get; private set; }
	}
}