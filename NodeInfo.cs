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

		IZWaveNodeStatus Status { get; }

		bool Ready { get; }

		[SchemaVersion(3)]
		bool SupportsBeaming { get; }


		[SchemaVersion(3)]
		ZWavePlusNodeType ZwavePlusNodeType { get; }

		[SchemaVersion(3)]
		ZWavePlusNodeRoleType zwavePlusRoleType { get; }

		bool IsListening { get; }

		bool IsRouting { get; }

		bool IsSecure { get; }

		long ManufacturerId { get; }

		long ProductId { get; }

		long ProductType { get; }

		string FirmwareVersion { get; }

		long ZwavePlusVersion { get; }
		string Name { get; }
		string Location { get; }
		string Label { get; }
		long[] Neighbors { get; }
		bool EndpointCountIsDynamic { get; }
		bool EndpointsHaveIdenticalCapabilities { get; }
		long IndividualEndpointCount { get; }
		long AggregatedEndpointCount { get; }

		long InterviewAttempts { get; }

		IZWaveNodeInterviewStage InterviewStage { get; }

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

		IZWaveNodeDataRate SupportedDataRates { get; }
		IZWaveNodeDeviceClass DeviceClass { get; }

		IZWaveCommandClass[] CommandClasses { get; }

		[SchemaVersion(3)]
		bool SupportsSecurity { get; }

	}

	public class NodeInfo : IZWaveNodeInfo
	{
		public long NodeId => throw new NotImplementedException();

		public long Index => throw new NotImplementedException();

		public string InstallerIcon => throw new NotImplementedException();

		public string UserIcon => throw new NotImplementedException();

		public IZWaveNodeStatus Status => throw new NotImplementedException();

		public bool Ready => throw new NotImplementedException();

		public bool SupportsBeaming => throw new NotImplementedException();

		public ZWavePlusNodeType ZwavePlusNodeType => throw new NotImplementedException();

		public ZWavePlusNodeRoleType zwavePlusRoleType => throw new NotImplementedException();

		public bool IsListening => throw new NotImplementedException();

		public bool IsRouting => throw new NotImplementedException();

		public bool IsSecure => throw new NotImplementedException();

		public long ManufacturerId => throw new NotImplementedException();

		public long ProductId => throw new NotImplementedException();

		public long ProductType => throw new NotImplementedException();

		public string FirmwareVersion => throw new NotImplementedException();

		public long ZwavePlusVersion => throw new NotImplementedException();

		public string Name => throw new NotImplementedException();

		public string Location => throw new NotImplementedException();

		public string Label => throw new NotImplementedException();

		public long[] Neighbors => throw new NotImplementedException();

		public bool EndpointCountIsDynamic => throw new NotImplementedException();

		public bool EndpointsHaveIdenticalCapabilities => throw new NotImplementedException();

		public long IndividualEndpointCount => throw new NotImplementedException();

		public long AggregatedEndpointCount => throw new NotImplementedException();

		public long InterviewAttempts => throw new NotImplementedException();

		public IZWaveNodeInterviewStage InterviewStage => throw new NotImplementedException();

		public IZWaveNodeEndpointState[] EndPoints => throw new NotImplementedException();

		public IZWaveNodeValue[] Values => throw new NotImplementedException();

		public FLiRS IsFrequentListening => throw new NotImplementedException();

		public long ProtocolVersion => throw new NotImplementedException();

		public long MaxDataRate => throw new NotImplementedException();

		public IZWaveNodeDataRate SupportedDataRates => throw new NotImplementedException();

		public IZWaveNodeDeviceClass DeviceClass => throw new NotImplementedException();

		public IZWaveCommandClass[] CommandClasses => throw new NotImplementedException();

		public bool SupportsSecurity => throw new NotImplementedException();
	}

	//Version 1.1.1 of the node info, will be deleted once we have version 1.1.3 to test against. 
	// public interface INodeInfo111
	// {
	// 	long NodeId { get; }

	// 	long Index { get; }

	// 	string InstallerIcon { get; }
	// 	string UserIcon { get; }

	// 	NodeStatus Status { get; }

	// 	bool Ready { get; }

	// 	//todo - rename to supportsBeaming
	// 	bool IsBeaming { get; }


	// 	//todo - being renamed to zwavePlusNodeType
	// 	ZWavePlusNodeType NodeType { get; }

	// 	//todo - being renamed to zwavePlusRoleType
	// 	ZWavePlusNodeRoleType RoleType { get; }

	// 	bool IsListening { get; }

	// 	bool IsRouting { get; }

	// 	//todo - The supportsSecurity property was split off from the isSecure property because they have a different meaning.
	// 	bool IsSecure { get; }

	// 	long ManufacturerId { get; }

	// 	long ProductId { get; }

	// 	long ProductType { get; }

	// 	string FirmwareVersion { get; }

	// 	long ZwavePlusVersion { get; }
	// 	string Name { get; }
	// 	string Location { get; }
	// 	string Label { get; }
	// 	long[] Neighbors { get; }
	// 	bool EndpointCountIsDynamic { get; }
	// 	bool EndpointsHaveIdenticalCapabilities { get; }
	// 	long IndividualEndpointCount { get; }
	// 	long AggregatedEndpointCount { get; }

	// 	long InterviewAttempts { get; }

	// 	NodeInterviewStage InterviewStage { get; }

	// 	INodeEndpointState[] EndPoints { get; }

	// 	// Todo - Support Parsing
	// 	//	object DeviceConfig { get; }

	// 	INodeValue[] Values { get; }

	// 	//TODO - this is being changed to a enum type
	// 	bool IsFrequentListening { get; }

	// 	//todo - this is being renamed to protocolVersion
	// 	long Version { get; }

	// 	//TODO - this is being changed to maxDataRate
	// 	long MaxBaudRate { get; }

	// 	// Broken in version 1.1.1. Uncomment out when upgrading to new version
	// 	//INodeDeviceClass DeviceClass { get; }

	// 	ICommandClass[] CommandClasses { get; }

	// 	//todo - add supportedDataRates

	// }


}