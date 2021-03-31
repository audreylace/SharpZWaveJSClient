
using System.Collections.Generic;

namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public interface IZWaveNodeDeviceClass
	{
		string Basic { get; }
		string Generic { get; }
		string Specific { get; }


		ZWaveCommandClasses[] MandatorySupportedCCs { get; }
		ZWaveCommandClasses[] MandatoryControlledCCs { get; }
	}
}