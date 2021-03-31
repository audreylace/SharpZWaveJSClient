using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface IControllerInfo
	{
		string LibraryVersion { get; }
		ZWaveLibraryTypes Type { get; }
		long HomeId { get; }
		long OwnNodeId { get; }
		bool IsSecondary { get; }
		bool IsUsingHomeIdFromOtherNetwork { get; }
		bool IsSISPresent { get; }
		bool WasRealPrimary { get; }
		bool IsStaticUpdateController { get; }
		bool IsSlave { get; }
		string SerialApiVersion { get; }
		long ManufacturerId { get; }
		long ProductType { get; }
		long ProductId { get; }
		ControllerDataMessageFunctionIDs[] SupportedFunctionTypes { get; }
		long SucNodeId { get; }
		bool SupportsTimers { get; }
	}

	public class ControllerInfo : IControllerInfo
	{
		public string LibraryVersion { get; set; }

		public ZWaveLibraryTypes Type { get; set; }

		public long HomeId { get; set; }

		public long OwnNodeId { get; set; }

		public bool IsSecondary { get; set; }

		public bool IsUsingHomeIdFromOtherNetwork { get; set; }

		public bool IsSISPresent { get; set; }

		public bool WasRealPrimary { get; set; }

		public bool IsStaticUpdateController { get; set; }

		public bool IsSlave { get; set; }

		public string SerialApiVersion { get; set; }

		public long ManufacturerId { get; set; }

		public long ProductType { get; set; }

		public long ProductId { get; set; }

		public ControllerDataMessageFunctionIDs[] SupportedFunctionTypes { get; set; }

		public long SucNodeId { get; set; }

		public bool SupportsTimers { get; set; }
	}

}