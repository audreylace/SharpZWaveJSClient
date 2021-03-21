using System;

namespace AudreysCloud.Community.SharpZWaveJSClient.Exceptions
{

	public class ProtocolException : SharpZWaveJSClientException
	{
		public ProtocolException() : base("ZWaveJS Protocol Exception") { }
		public ProtocolException(string message) : base("WaveJS Protocol Exception : " + message) { }
		public ProtocolException(string message, Exception innerException) : base("WaveJS Protocol Exception : " + message, innerException) { }
	}

}