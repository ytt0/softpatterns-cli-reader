namespace Notes
{
    public interface IConfiguration
    {
        Dictionary<string, string> Aliases { get; }
        IEnumerable<string> Categories { get; }
        string Editor { get; }
    }

    public class Configuration : IConfiguration
    {
        public Dictionary<string, string> Aliases { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public string Editor { get; set; }

        public Configuration()
        {
            this.Aliases = new Dictionary<string, string>();
            this.Categories = ["Normal", "Important", "Archive"];
            this.Editor = "notepad";
        }

        public static Configuration Load(string configurationPath)
        {
            if (!File.Exists(configurationPath))
            {
                return new Configuration();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            return JsonSerializer.Deserialize<Configuration>(File.ReadAllText(configurationPath), options)!;
        }
    }
}
