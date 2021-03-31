using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface IZWaveNodeValueMetadata
	{
		ZWaveNodeValueStoreValueType Type { get; }
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

		IDictionary<long, string> States { get; }
	}

	public class ZWaveNodeValueMetadata : IZWaveNodeValueMetadata
	{

		public ZWaveNodeValueStoreValueType Type { get; set; }

		public object Default { get; set; }

		public bool Readable { get; set; }

		public bool Writeable { get; set; }

		public string Description { get; set; }

		public string Label { get; set; }

		[JsonPropertyName("ccSpecific")]
		public object CCSpecific { get; set; }

		public long MinLength { get; set; }

		public long MaxLength { get; set; }

		public long Min { get; set; }

		public long Max { get; set; }

		public long Steps { get; set; }

		public string Unit { get; set; }

		[JsonConverter(typeof(ZWaveNodeMetaDataStateConverter))]
		public IDictionary<long, string> States { get; set; }
	}



}