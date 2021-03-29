using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	//Version 1.1.1 of the node info, will be deleted once we have version 1.1.3 to test against. 
	public interface INodeInfo111
	{
		long NodeId { get; }

		long Index { get; }

		string InstallerIcon { get; }
		string UserIcon { get; }

		NodeStatus Status { get; }

		bool Ready { get; }

		//todo - rename to supportsBeaming
		bool IsBeaming { get; }


		//todo - being renamed to zwavePlusNodeType
		ZWavePlusNodeType NodeType { get; }

		//todo - being renamed to zwavePlusRoleType
		ZWavePlusRoleType RoleType { get; }

		bool IsListening { get; }

		bool IsRouting { get; }

		//todo - The supportsSecurity property was split off from the isSecure property because they have a different meaning.
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

		NodeInterviewStage InterviewStage { get; }

		INodeEndpointState[] EndPoints { get; }

		// Todo - Support Parsing
		//	object DeviceConfig { get; }

		INodeValue[] Values { get; }

		//TODO - this is being changed to a enum type
		bool IsFrequentListening { get; }

		//todo - this is being renamed to protocolVersion
		long Version { get; }

		//TODO - this is being changed to maxDataRate
		long MaxBaudRate { get; }

		// Broken in version 1.1.1. Uncomment out when upgrading to new version
		//INodeDeviceClass DeviceClass { get; }

		ICommandClass[] CommandClasses { get; }

		//todo - add supportedDataRates

	}


}