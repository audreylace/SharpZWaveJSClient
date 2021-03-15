using System;

namespace AudreysCloud.Community.SharpZWaveJSClient.Exceptions
{

	public class SharpZWaveJSClientException : Exception
	{
		public SharpZWaveJSClientException() : base() { }

		public SharpZWaveJSClientException(string message) : base(message) { }

		public SharpZWaveJSClientException(string message, Exception innerException) : base(message, innerException) { }
	}
}