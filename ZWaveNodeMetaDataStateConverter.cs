using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public class ZWaveNodeMetaDataStateConverter : JsonConverter<IDictionary<long, string>>
	{
		public override IDictionary<long, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{

			IDictionary<string, string> inDict = JsonSerializer.Deserialize<IDictionary<string, string>>(ref reader, options);
			Dictionary<long, string> outDict = new Dictionary<long, string>();


			foreach (KeyValuePair<string, string> pair in inDict)
			{
				outDict.TryAdd(long.Parse(pair.Key), pair.Value);
			}

			return outDict;
		}

		public override void Write(Utf8JsonWriter writer, IDictionary<long, string> sourceDict, JsonSerializerOptions options)
		{
			Dictionary<string, string> outDict = new Dictionary<string, string>();
			foreach (KeyValuePair<long, string> pair in sourceDict)
			{
				outDict.TryAdd(pair.Key.ToString(), pair.Value);
			}

			JsonSerializer.Serialize<IDictionary<string, string>>(writer, outDict, options);

		}
	}
}