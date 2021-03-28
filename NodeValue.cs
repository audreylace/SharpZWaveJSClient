using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface INodeValue
	{
		long Endpoint { get; }
		CommandClasses CommandClass { get; }
		string CommandClassName { get; }

		[JsonPropertyName("property")]
		[JsonConverter(typeof(TryConvertToStringConverter))]
		string PropertyString { get; }

		[JsonPropertyName("property")]
		[JsonConverter(typeof(TryConvertToNumberConverter))]
		long PropertyNumber { get; }

		[JsonPropertyName("propertyKey")]
		[JsonConverter(typeof(TryConvertToStringConverter))]
		string PropertyKeyString { get; }

		[JsonPropertyName("propertyKey")]
		[JsonConverter(typeof(TryConvertToNumberConverter))]
		long PropertyKeyNumber { get; }

		string PropertyName { get; }

		string PropertyKeyName { get; }

		[JsonPropertyName("ccVersion")]
		long CCVersion { get; }

		//TODO - Converter
		IValueMetadataAny Metadata { get; }

		NodeValueStoreValueType ValueType { get; }

		object Value { get; }
	}

	public interface IDurationType
	{
		DurationUnit Unit { get; }
		long Value { get; }
	}

}