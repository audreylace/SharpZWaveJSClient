using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

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

		public static StartListeningCommandResult ParseStartListeningCommand(this SharpZWaveJSClient connection, JsonElement result)
		{
			JsonElement stateElement = result.GetProperty("state");
			return JsonSerializer.Deserialize<StartListeningCommandResult>(stateElement.GetRawText(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
		}
	}

	public class StartListeningCommandResult
	{
		[JsonConverter(typeof(ImplementInterfaceConverter<IZWaveControllerInfo, ZWaveControllerInfo>))]
		[JsonInclude]
		public IZWaveControllerInfo Controller { get; private set; }

		[JsonConverter(typeof(ImplementInterfaceConverter<IZWaveNodeInfo[], ZWaveNodeInfo[]>))]
		[JsonInclude]
		public IZWaveNodeInfo[] Nodes { get; private set; }


	}

}