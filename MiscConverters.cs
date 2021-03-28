using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public class SaveArrayOrObjectAsJsonConverter : JsonConverter<string>
	{
		public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject && reader.TokenType != JsonTokenType.StartArray)
			{
				throw new JsonException();
			}
			return reader.GetString();
		}

		public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}

	public class TryConvertToStringConverter : JsonConverter<string>
	{
		public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.String && reader.TokenType != JsonTokenType.Number)
			{
				throw new JsonException();
			}

			if (reader.TokenType == JsonTokenType.Number)
			{
				return "";
			}
			return reader.GetString();
		}

		public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}


	public class TryConvertToNumberConverter : JsonConverter<double>
	{
		public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.Number && reader.TokenType != JsonTokenType.String)
			{
				throw new JsonException();
			}

			if (reader.TokenType == JsonTokenType.String)
			{
				return 0;
			}

			return reader.GetDouble();
		}

		public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}