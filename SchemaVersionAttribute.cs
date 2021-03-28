
namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public class SchemaVersionAttribute : System.Attribute
	{

		public SchemaVersionAttribute(int min)
		{
			MinSchemaVersion = min;
		}

		public SchemaVersionAttribute(int min, int max)
		{
			MaxSchemaVersion = max;
			MinSchemaVersion = min;
		}
		public int MinSchemaVersion { get; private set; }

		public int MaxSchemaVersion { get; private set; }
	}
}