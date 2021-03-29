
using System.Collections.Generic;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface INodeDeviceClass
	{
		string Basic { get; }
		string Generic { get; }
		string Specific { get; }

		//TODO - Custom converter needed!
		CommandClasses[] MandatorySupportedCCs { get; }
		CommandClasses[] MandatoryControlledCCs { get; }
	}
}