using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{

	public enum NodeValueStoreValueType
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

	public interface IValueMetadata
	{
		NodeValueStoreValueType Type { get; }
		object Default { get; }
		bool Readable { get; }
		bool Writeable { get; }
		string Description { get; }
		string Label { get; }
		[JsonPropertyName("ccSpecific")]
		object CCSpecific { get; }
		long MinLength { get; }
		long MaxLength { get; }
		long Min { get; }
		long Max { get; }
		long Steps { get; }
		string Unit { get; }
		Dictionary<long, string> States { get; }
	}

	public enum DurationUnit
	{
		Seconds,
		Minutes,
		Unknown,
		Default
	}

}