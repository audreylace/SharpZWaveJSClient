using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AudreysCloud.Community.SharpZWaveJSClient
{

	public interface INodeInfo
	{
		long NodeId { get; }

		long Index { get; }

		string InstallerIcon { get; }
		string UserIcon { get; }

		NodeStatus Status { get; }

		bool Ready { get; }

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

		/// <summary>Device config JSON from the remote server. </summary>
		[JsonPropertyName("deviceConfig")]
		[JsonConverter(typeof(SaveArrayOrObjectAsJsonConverter))]
		string DeviceConfigJson { get; }

		INodeValue[] Values { get; }

	}


}