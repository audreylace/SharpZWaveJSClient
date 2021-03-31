
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface IZWaveNodeDeviceClass
	{
		KeyValuePair<long, string> Basic { get; }
		KeyValuePair<long, string> Generic { get; }
		KeyValuePair<long, string> Specific { get; }


		ZWaveCommandClasses[] MandatorySupportedCCs { get; }
		ZWaveCommandClasses[] MandatoryControlledCCs { get; }
	}

	public class ZWaveNodeDeviceClass : IZWaveNodeDeviceClass
	{

		[JsonConverter(typeof(KeyLabelPairConverter))]
		public KeyValuePair<long, string> Basic { get; set; }

		[JsonConverter(typeof(KeyLabelPairConverter))]
		public KeyValuePair<long, string> Generic { get; set; }

		[JsonConverter(typeof(KeyLabelPairConverter))]
		public KeyValuePair<long, string> Specific { get; set; }


		public ZWaveCommandClasses[] MandatorySupportedCCs { get; set; }


		public ZWaveCommandClasses[] MandatoryControlledCCs { get; set; }
	}

	public class KeyLabelPairConverter : JsonConverter<KeyValuePair<long, string>>
	{
		public override KeyValuePair<long, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			using (JsonDocument document = JsonDocument.ParseValue(ref reader))
			{
				JsonElement element = document.RootElement;
				long key = element.GetProperty("key").GetInt64();
				string label = element.GetProperty("label").GetString();

				return new KeyValuePair<long, string>(key, label);
			}
		}

		public override void Write(Utf8JsonWriter writer, KeyValuePair<long, string> value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber("key", value.Key);
			writer.WriteString("label", value.Value);
			writer.WriteEndObject();
		}
	}


}