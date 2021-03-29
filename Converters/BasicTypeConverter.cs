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

	internal class StringOrNumberOrBool : IStringOrNumberOrBool
	{
		public Type Type { get; internal set; }

		public bool ValueBool { get; internal set; }

		public double ValueNumber { get; internal set; }

		public string ValueString { get; internal set; }
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
			throw new NotImplementedException();
		}
	}


}