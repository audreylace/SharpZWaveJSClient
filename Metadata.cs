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


	public interface IValueMetadataAny
	{
		NodeValueStoreValueType Type { get; }
		object Default { get; }

		bool Readable { get; }

		bool Writeable { get; }

		string Description { get; }

		string Label { get; }

		[JsonPropertyName("ccSpecific")]
		[JsonConverter(typeof(SaveArrayOrObjectAsJsonConverter))]
		string CCSpecific { get; }
	}
	public enum DurationUnit
	{
		Seconds,
		Minutes,
		Unknown,
		Default
	}
	public interface IValueDurationMetadata : IValueMetadataAny
	{
		new IDurationType Default { get; }
	}

	public interface IValueBufferMetadata : IValueMetadataAny
	{
		long MinLength { get; }
		long MaxLength { get; }
	}

	public interface IValueStringMetadata : IValueMetadataAny
	{
		long MinLength { get; }
		long MaxLength { get; }
		new string Default { get; }
	}

	public interface IBoolMetadata : IValueMetadataAny
	{
		new bool Default { get; }
	}


	public interface INumericMetadata : IValueMetadataAny
	{
		long Min { get; }
		long Max { get; }
		long Steps { get; }

		new long Default { get; }

		string Unit { get; }

		Dictionary<long, string> States { get; }
	}
}