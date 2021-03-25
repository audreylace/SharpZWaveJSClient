using System.Threading;
using System.Threading.Tasks;

namespace AudreysCloud.Community.SharpZWaveJSClient.Extentions.Commands
{
	public class StartListeningCommand : ZWaveJSCommand
	{
		public StartListeningCommand() : base("start_listening") { }
	}

	public static class StartListeningCommandExtensions
	{
		public static async Task<string> SendStartListeningCommandAsync(this SharpZWaveJSClient connection, CancellationToken cancellationToken)
		{
			StartListeningCommand command = new StartListeningCommand();
			await connection.SendCommandAsync(command, cancellationToken);
			return command.MessageId;
		}
	}

	public class StartListeningCommandResult
	{
		//TODO
	}

}