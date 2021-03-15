using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient.Protocol
{
	internal class OutgoingCommandMessageConverter : JsonConverter<OutgoingCommandMessage>
	{
		public override OutgoingCommandMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, OutgoingCommandMessage value, JsonSerializerOptions options)
		{

			writer.WriteStartObject();

			if (value.Payload != null)
			{
				string encodedObject = JsonSerializer.Serialize(value.Payload, options);
				using (JsonDocument document = JsonDocument.Parse(encodedObject))
				{
					foreach (JsonProperty property in document.RootElement.EnumerateObject())
					{
						//Skip reserved property names
						if (!property.NameEquals("messageId") && !property.NameEquals("command"))
						{
							property.WriteTo(writer);
						}
					}
				}
			}

			writer.WriteString("messageId", value.MessageId);
			writer.WriteString("command", value.Command);
			writer.WriteEndObject();

			writer.Flush();
		}
	}
}