
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{


	[JsonConverter(typeof(ZWaveNodeValueStoreValueTypeConverter))]
	public enum ZWaveNodeValueStoreValueType
	{
		Any,
		Number,
		Boolean,
		String,
		Duration,
		Buffer,
		BooleanArray,
		StringArray,
		NumberArray,
		Color
	}

	public class ZWaveNodeValueStoreValueTypeConverter : JsonConverter<ZWaveNodeValueStoreValueType>
	{
		public override ZWaveNodeValueStoreValueType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.String)
			{
				throw new JsonException();
			}


			string incomingValue = reader.GetString().ToUpperInvariant();
			switch (incomingValue)
			{
				case "ANY":
					return ZWaveNodeValueStoreValueType.Any;
				case "NUMBER":
					return ZWaveNodeValueStoreValueType.Number;
				case "BOOLEAN":
					return ZWaveNodeValueStoreValueType.Boolean;
				case "STRING":
					return ZWaveNodeValueStoreValueType.String;
				case "NUMBER[]":
					return ZWaveNodeValueStoreValueType.NumberArray;
				case "BOOLEAN[]":
					return ZWaveNodeValueStoreValueType.BooleanArray;
				case "STRING[]":
					return ZWaveNodeValueStoreValueType.StringArray;
				case "DURATION":
					return ZWaveNodeValueStoreValueType.Duration;
				case "COLOR":
					return ZWaveNodeValueStoreValueType.Color;
				case "BUFFER":
					return ZWaveNodeValueStoreValueType.Buffer;
				default:
					throw new NotSupportedException("Unsupported value present for ZWaveNodeStoreValueType.");
			}
		}

		public override void Write(Utf8JsonWriter writer, ZWaveNodeValueStoreValueType value, JsonSerializerOptions options)
		{
			switch (value)
			{
				case ZWaveNodeValueStoreValueType.Any:
					writer.WriteStringValue("any");
					break;
				case ZWaveNodeValueStoreValueType.Number:
					writer.WriteStringValue("number");
					break;
				case ZWaveNodeValueStoreValueType.Boolean:
					writer.WriteStringValue("boolean");
					break;
				case ZWaveNodeValueStoreValueType.String:
					writer.WriteStringValue("string");
					break;
				case ZWaveNodeValueStoreValueType.NumberArray:
					writer.WriteStringValue("number[]");
					break;
				case ZWaveNodeValueStoreValueType.BooleanArray:
					writer.WriteStringValue("boolean[]");
					break;
				case ZWaveNodeValueStoreValueType.StringArray:
					writer.WriteStringValue("string[]");
					break;
				case ZWaveNodeValueStoreValueType.Duration:
					writer.WriteStringValue("duration");
					break;
				case ZWaveNodeValueStoreValueType.Color:
					writer.WriteStringValue("color");
					break;
				case ZWaveNodeValueStoreValueType.Buffer:
					writer.WriteStringValue("buffer");
					break;
				default:
					throw new NotImplementedException();
			}

			return;
		}
	}


}