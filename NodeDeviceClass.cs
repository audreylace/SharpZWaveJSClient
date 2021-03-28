
using System.Collections.Generic;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface INodeDeviceClass
	{
		KeyValuePair<long, string> Basic { get; }
		KeyValuePair<long, string> Generic { get; }
		KeyValuePair<long, string> Specific { get; }
		CommandClasses[] MandatorySupportedCCs { get; }
		CommandClasses[] MandatoryControlledCCs { get; }
	}
}