using System;

namespace AudreysCloud.Community.SharpZWaveJSClient.Exceptions
{

	public class HandshakeException : SharpZWaveJSClientException
	{
		public HandshakeException() : base("Handshake Exception") { }
		public HandshakeException(string message) : base("Handshake Error : " + message) { }
		public HandshakeException(string message, Exception innerException) : base("Handshake Error : " + message, innerException) { }
	}

}