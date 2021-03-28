using System;

namespace AudreysCloud.Community.SharpZWaveJSClient.Exceptions
{

	public class ProtocolException : SharpZWaveJSClientException
	{
		public ProtocolException() : base("ZWaveJS Protocol Exception") { }
		public ProtocolException(string message) : base(message) { }
		public ProtocolException(string message, Exception innerException) : base(message, innerException) { }
	}

}