using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{

	public interface ICommandClass
	{
		CommandClasses Id { get; }
		long Version { get; }

		bool IsSecure { get; }

	}

	public enum ZWavePlusNodeType
	{
		Node = 0,
		IPGateway = 2
	}

	public enum ZWavePlusRoleType
	{
		CentralStaticController = 0,
		SubStaticController = 1,
		PortableController = 2,
		PortableReportingController = 3,
		PortableSlave = 4,
		AlwaysOnSlave = 5,
		SleepingReportingSlave = 6,
		SleepingListeningSlave = 7
	}

	public interface INodeInfo
	{
		long NodeId { get; }

		long Index { get; }

		string InstallerIcon { get; }
		string UserIcon { get; }

		NodeStatus Status { get; }

		bool Ready { get; }
		bool IsBeaming { get; }

		ZWavePlusNodeType NodeType { get; }
		ZWavePlusRoleType RoleType { get; }

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

		NodeInterviewStage InterviewStage { get; }

		INodeEndpointState[] EndPoints { get; }

		// Todo - Support Parsing
		//	object DeviceConfig { get; }

		INodeValue[] Values { get; }

		bool IsFrequentListening { get; }

		long Version { get; }

		long MaxBaudRate { get; }

		INodeDeviceClass DeviceClass { get; }

		ICommandClass[] CommandClasses { get; }

	}


}