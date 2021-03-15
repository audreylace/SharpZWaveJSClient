using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient.Protocol
{
	internal class IncomingMessageConverter : JsonConverter<IIncomingMessage>
	{
		// Nested conversions are not allowed! There should be at most one top level incoming message converter.
		private bool _conversionActive = false;
		public override bool CanConvert(Type typeToConvert) =>
		   typeof(IIncomingMessage).IsAssignableFrom(typeToConvert) && !_conversionActive;
		public override IIncomingMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
			{
				throw new JsonException();
			}

			using (var jsonDocument = JsonDocument.ParseValue(ref reader))
			{
				if (!jsonDocument.RootElement.TryGetProperty("type", out var typeProperty))
				{
					throw new JsonException();
				}

				if (typeProperty.ValueKind != JsonValueKind.String)
				{
					throw new JsonException();
				}

				string typePropertyValue = typeProperty.GetString();

				if (Enum.TryParse<IncomingMessageType>(typePropertyValue, true, out IncomingMessageType messageType))
				{
					switch (messageType)
					{
						case IncomingMessageType.Event:
							break;
						case IncomingMessageType.Result:
							return new RawResultMessage(jsonDocument);
						case IncomingMessageType.Version:

							_conversionActive = true;
							try
							{
								return JsonSerializer.Deserialize<VersionMessage>(jsonDocument.RootElement.GetRawText(), options);
							}
							finally
							{
								_conversionActive = false;
							}

						default:
							break;

					}
				}
			}

			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, IIncomingMessage value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}

}