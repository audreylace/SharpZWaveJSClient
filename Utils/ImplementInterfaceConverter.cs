using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient.Utils
{
	public class ImplementInterfaceConverter<InterfaceType, ImplementationType> : JsonConverter<InterfaceType> where ImplementationType : InterfaceType
	{

		public override InterfaceType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{

			if (reader.TokenType != JsonTokenType.StartObject && reader.TokenType != JsonTokenType.StartArray)
			{
				throw new JsonException();
			}

			return (InterfaceType)JsonSerializer.Deserialize<ImplementationType>(ref reader, options);

		}

		public override void Write(Utf8JsonWriter writer, InterfaceType value, JsonSerializerOptions options)
		{
			JsonSerializer.Serialize<ImplementationType>(writer, (ImplementationType)value, options);
		}
	}
}