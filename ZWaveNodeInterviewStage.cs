namespace AudreysCloud.Community.SharpZWaveJSClient
{


	// MIT License

	// Copyright (c) 2018-2020 AlCalzone

	// Permission is hereby granted, free of charge, to any person obtaining a copy
	// of this software and associated documentation files (the "Software"), to deal
	// in the Software without restriction, including without limitation the rights
	// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	// copies of the Software, and to permit persons to whom the Software is
	// furnished to do so, subject to the following conditions:

	// The above copyright notice and this permission notice shall be included in all
	// copies or substantial portions of the Software.

	// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	// SOFTWARE.
	public enum ZWaveNodeInterviewStage
	{
		/** The interview process hasn't started for this node */
		None = 0,
		/** The node's protocol information has been queried from the controller */
		ProtocolInfo = 1,
		/** The node has been queried for supported and controlled command classes */
		NodeInfo = 2,
		/**
		 * Information for all command classes has been queried.
		 * This includes static information that is requested once as well as dynamic
		 * information that is requested on every restart.
		 */
		CommandClasses = 3,
		/**
		 * Device information for the node has been loaded from a config file.
		 * If defined, some of the reported information will be overwritten based on the
		 * config file contents.
		 */
		OverwriteConfig = 4,
		/** The node has been queried for its current neighbor list */
		Neighbors = 5,
		/** The interview process has finished */
		Complete = 6
	}

}