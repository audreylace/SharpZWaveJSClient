using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpZWaveJSClient.Converters;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface IZWaveNodeValue
	{
		long Endpoint { get; }
		ZWaveCommandClasses CommandClass { get; }

		[JsonConverter(typeof(StringOrNumberOrBoolConverter))]
		IStringOrNumberOrBool Property { get; }

		[JsonConverter(typeof(StringOrNumberOrBoolConverter))]
		IStringOrNumberOrBool PropertyKey { get; }

		string PropertyName { get; }

		string PropertyKeyName { get; }

		[JsonPropertyName("ccVersion")]
		long CCVersion { get; }

		IZWaveNodeValueMetadata Metadata { get; }
		object Value { get; }
	}


	public class ZWaveNodeValue : IZWaveNodeValue
	{
		public long Endpoint { get; set; }

		public ZWaveCommandClasses CommandClass { get; set; }

		[JsonConverter(typeof(StringOrNumberOrBoolConverter))]
		public IStringOrNumberOrBool Property { get; set; }

		[JsonConverter(typeof(StringOrNumberOrBoolConverter))]
		public IStringOrNumberOrBool PropertyKey { get; set; }

		public string PropertyName { get; set; }

		public string PropertyKeyName { get; set; }

		[JsonPropertyName("ccVersion")]
		public long CCVersion { get; set; }

		[JsonConverter(typeof(ImplementInterfaceConverter<IZWaveNodeValueMetadata, ZWaveNodeValueMetadata>))]
		public IZWaveNodeValueMetadata Metadata { get; set; }

		public object Value { get; set; }
	}


}