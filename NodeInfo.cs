using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface IZWaveNodeInfo
	{
		long NodeId { get; }

		long Index { get; }

		string InstallerIcon { get; }
		string UserIcon { get; }

		ZWaveNodeStatus Status { get; }

		bool Ready { get; }

		[SchemaVersion(3)]
		bool SupportsBeaming { get; }


		[SchemaVersion(3)]
		ZWavePlusNodeType ZWavePlusNodeType { get; }

		[SchemaVersion(3)]
		ZWavePlusNodeRoleType ZWavePlusRoleType { get; }

		bool IsListening { get; }

		bool IsRouting { get; }

		bool IsSecure { get; }

		long ManufacturerId { get; }

		long ProductId { get; }

		long ProductType { get; }

		string FirmwareVersion { get; }

		long ZWavePlusVersion { get; }
		string Name { get; }
		string Location { get; }
		string Label { get; }
		long[] Neighbors { get; }
		bool EndpointCountIsDynamic { get; }
		bool EndpointsHaveIdenticalCapabilities { get; }
		long IndividualEndpointCount { get; }
		long AggregatedEndpointCount { get; }

		long InterviewAttempts { get; }

		ZWaveNodeInterviewStage InterviewStage { get; }

		IZWaveNodeEndpointState[] EndPoints { get; }

		// Todo - Support Parsing
		//	object DeviceConfig { get; }

		IZWaveNodeValue[] Values { get; }

		[SchemaVersion(3)]
		FLiRS IsFrequentListening { get; }

		[SchemaVersion(3)]
		long ProtocolVersion { get; }

		[SchemaVersion(3)]
		long MaxDataRate { get; }

		[SchemaVersion(3)]

		ZWaveNodeDataRate SupportedDataRates { get; }
		IZWaveNodeDeviceClass DeviceClass { get; }

		IZWaveCommandClass[] CommandClasses { get; }

		[SchemaVersion(3)]
		bool SupportsSecurity { get; }

	}

	public class ZWaveNodeInfo : IZWaveNodeInfo
	{

		public long NodeId { get; set; }

		public long Index { get; set; }

		public string InstallerIcon { get; set; }

		public string UserIcon { get; set; }

		public ZWaveNodeStatus Status { get; set; }

		public bool Ready { get; set; }

		private bool IsBeaming { set { SupportsBeaming = value; } }

		public bool SupportsBeaming { get; set; }

		[JsonPropertyName("nodeType")]
		[ObsoleteAttribute("This property has been renamed to ZWavePlusNodeType as of Schema 3. New code should use ZWavePlusNodeType instead as this property is scheduled to be removed in a future version.")]
		public ZWavePlusNodeType NodeType
		{
			get { return ZWavePlusNodeType; }
			set { ZWavePlusNodeType = value; }
		}

		[JsonPropertyName("zwavePlusNodeType")]
		public ZWavePlusNodeType ZWavePlusNodeType { get; set; }

		[JsonPropertyName("roleType")]
		[ObsoleteAttribute("This property has been renamed to ZWavePlusRoleType as of Schema 3. New code should use ZWavePlusRoleType instead as this property is scheduled to be removed in a future version.")]
		public ZWavePlusNodeRoleType RoleType
		{
			get { return ZWavePlusRoleType; }
			set { ZWavePlusRoleType = value; }
		}

		[JsonPropertyName("zwavePlusRoleType")]
		public ZWavePlusNodeRoleType ZWavePlusRoleType { get; set; }

		public bool IsListening { get; set; }

		public bool IsRouting { get; set; }

		public bool IsSecure { get; set; }

		public long ManufacturerId { get; set; }

		public long ProductId { get; set; }

		public long ProductType { get; set; }

		public string FirmwareVersion { get; set; }

		[JsonPropertyName("zwavePlusVersion")]
		public long ZWavePlusVersion { get; set; }

		public string Name { get; set; }

		public string Location { get; set; }

		public string Label { get; set; }

		public long[] Neighbors { get; set; }

		public bool EndpointCountIsDynamic { get; set; }

		public bool EndpointsHaveIdenticalCapabilities { get; set; }

		public long IndividualEndpointCount { get; set; }

		public long AggregatedEndpointCount { get; set; }

		public long InterviewAttempts { get; set; }

		public ZWaveNodeInterviewStage InterviewStage { get; set; }

		[JsonConverter(typeof(ImplementInterfaceConverter<ZWaveNodeEndpointState>))]
		public IZWaveNodeEndpointState[] EndPoints { get; set; }

		[JsonConverter(typeof(ImplementInterfaceConverter<ZWaveNodeValue>))]
		public IZWaveNodeValue[] Values { get; set; }

		[JsonConverter(typeof(FLiRSConverter))]
		public FLiRS IsFrequentListening { get; set; }

		[ObsoleteAttribute("This property has been renamed to ProtocolVersion as of Schema 3. New code should use ProtocolVersion instead as this property is scheduled to be removed in a future version.")]
		[SchemaVersion(0, 2)]
		public long Version
		{
			get { return ProtocolVersion; }
			set { ProtocolVersion = value; }
		}

		[SchemaVersion(3)]
		public long ProtocolVersion { get; set; }

		[SchemaVersion(3)]
		public long MaxDataRate { get; set; }

		[ObsoleteAttribute("This property has been renamed to MaxDataRate as of Schema 3. New code should use MaxDataRate instead as this property is scheduled to be removed in a future version.")]
		[SchemaVersion(0, 2)]
		public long MaxBaudRate
		{
			get { return MaxDataRate; }
			set { MaxDataRate = value; }
		}

		[SchemaVersion(3)]
		public ZWaveNodeDataRate SupportedDataRates { get; set; }

		[JsonConverter(typeof(ImplementInterfaceConverter<ZWaveNodeDeviceClass>))]
		public IZWaveNodeDeviceClass DeviceClass { get; set; }

		[JsonConverter(typeof(ImplementInterfaceConverter<ZWaveCommandClass>))]
		public IZWaveCommandClass[] CommandClasses { get; set; }

		[SchemaVersion(3)]
		public bool SupportsSecurity { get; set; }
	}
}