namespace AudreysCloud.Community.SharpZWaveJSClient
{

	public class SetApiSchemaCommand : ZWaveJSCommand
	{
		public SetApiSchemaCommand(int version) : base("set_api_schema")
		{
			SchemaVersion = version;
		}
		public int SchemaVersion { get; set; }

	}
}