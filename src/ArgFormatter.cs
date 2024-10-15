namespace SoftPatterns.Cli.Reader
{
    public interface IArgFormatter
    {
        string FormatNameArg(string name);
        string FormatNameGroupArg(string group);
    }

    // POSIX-style formatter
    public class ArgFormatter : IArgFormatter
    {
        public static readonly ArgFormatter Instance = new();

        private ArgFormatter()
        {
        }

        public string FormatNameArg(string name)
        {
            return "--" + name;
        }

        public string FormatNameGroupArg(string group)
        {
            return "-" + group;
        }
    }

    // DOS-style formatter
    public class DosArgFormatter : IArgFormatter
    {
        public static readonly DosArgFormatter Instance = new();

        private DosArgFormatter()
        {
        }

        public string FormatNameArg(string name)
        {
            return "/" + name;
        }

        public string FormatNameGroupArg(string group)
        {
            return "/" + group;
        }
    }
}
