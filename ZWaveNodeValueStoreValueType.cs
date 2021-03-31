
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{


	[JsonConverter(typeof(ZWaveNodeValueStoreValueTypeConverter))]
	public enum ZWaveNodeValueStoreValueType
	{
		Any,
		Numeric,
		Boolean,
		String,
		Duration,
		Buffer,
		BooleanArray,
		StringArray,
		NumericArray,
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
					return ZWaveNodeValueStoreValueType.Numeric;
				case "BOOLEAN":
					return ZWaveNodeValueStoreValueType.Boolean;
				case "STRING":
					return ZWaveNodeValueStoreValueType.String;
				case "NUMBER[]":
					return ZWaveNodeValueStoreValueType.NumericArray;
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
			throw new NotImplementedException();
		}
	}


}