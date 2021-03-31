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

		IValueMetadata Metadata { get; }

		NodeValueStoreValueType ValueType { get; }
		object Value { get; }
	}

	public interface IZWaveValueDurationType
	{
		DurationUnit Unit { get; }
		long Value { get; }
	}

}