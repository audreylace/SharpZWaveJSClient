using System.Text.Json.Serialization;
using AudreysCloud.Community.SharpZWaveJSClient.Converters;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface INodeValue
	{
		long Endpoint { get; }
		CommandClasses CommandClass { get; }

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

	public interface IDurationType
	{
		DurationUnit Unit { get; }
		long Value { get; }
	}

}