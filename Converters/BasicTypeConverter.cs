using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient.Converters
{
	public interface IStringOrNumberOrBool
	{
		System.Type Type { get; }
		bool ValueBool { get; }
		double ValueNumber { get; }
		string ValueString { get; }
	}

	public class StringOrNumberOrBool : IStringOrNumberOrBool
	{
		public Type Type { get; set; }

		public bool ValueBool { get; set; }

		public double ValueNumber { get; set; }

		public string ValueString { get; set; }
	}

	public class StringOrNumberOrBoolConverter : JsonConverter<IStringOrNumberOrBool>
	{
		public override IStringOrNumberOrBool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.StartObject || reader.TokenType == JsonTokenType.StartArray)
			{
				throw new JsonException();
			}

			StringOrNumberOrBool unionType;
			switch (reader.TokenType)
			{
				case JsonTokenType.String:
					unionType = new StringOrNumberOrBool();
					unionType.Type = typeof(string);
					unionType.ValueString = reader.GetString();
					return unionType;
				case JsonTokenType.Number:
					unionType = new StringOrNumberOrBool();
					unionType.Type = typeof(double);
					unionType.ValueNumber = reader.GetDouble();
					return unionType;
				case JsonTokenType.True:
				case JsonTokenType.False:
					unionType = new StringOrNumberOrBool();
					unionType.Type = typeof(bool);
					unionType.ValueBool = reader.GetBoolean();
					return unionType;
				default:
					throw new JsonException();
			}
		}

		public override void Write(Utf8JsonWriter writer, IStringOrNumberOrBool value, JsonSerializerOptions options)
		{
			if (value.Type == typeof(bool))
			{
				writer.WriteBooleanValue(value.ValueBool);
			}
			else if (value.Type == typeof(string))
			{
				writer.WriteStringValue(value.ValueString);
			}
			else if (value.Type == typeof(double))
			{
				writer.WriteNumberValue(value.ValueNumber);
			}
			else
			{
				throw new NotImplementedException();
			}

		}
	}


}