
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

	public class ZWaveNodeDeviceClass : IZWaveNodeDeviceClass
	{

		public string Basic { get; set; }


		public string Generic { get; set; }


		public string Specific { get; set; }


		public ZWaveCommandClasses[] MandatorySupportedCCs { get; set; }


		public ZWaveCommandClasses[] MandatoryControlledCCs { get; set; }
	}
}