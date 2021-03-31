
namespace AudreysCloud.Community.SharpZWaveJSClient
{
	public class SchemaRenameAttribute : System.Attribute
	{
		public SchemaRenameAttribute(int min, int max, string name)
		{
			MinVersion = min;
			MaxVersion = max;
			Name = name;
		}
		public int MinVersion { get; set; }
		public int MaxVersion { get; set; }
		public string Name { get; set; }
	}
}