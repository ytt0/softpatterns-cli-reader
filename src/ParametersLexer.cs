namespace SoftPatterns.Cli.Reader
{
    public interface IParametersLexer
    {
        IArgFormatter ArgFormatter { get; }

        IEnumerable<IParameterToken> GetTokens(string[] args, IParameterToken? substitutedToken = null);
    }

    // POSIX-style lexer
    public partial class ParametersLexer : IParametersLexer
    {
        private static readonly Regex ParameterRegex = new("^--(?<name>[^=:\\s]+)(([=:]|\\s+)(?<value>.*))?$");
        private static readonly Regex ParameterGroupRegex = new("^-(?<group>[^=:\\s]+)(([=:]|\\s+)(?<value>.*))?$");

        private const string EndOfOptionsDelimiter = "--";

        public static readonly ParametersLexer Instance = new();

        public IArgFormatter ArgFormatter { get; }

        private ParametersLexer()
        {
            this.ArgFormatter = SoftPatterns.Cli.Reader.ArgFormatter.Instance;
        }

        public IEnumerable<IParameterToken> GetTokens(string[] args, IParameterToken? substitutedToken = null)
        {
            var tokens = new List<IParameterToken>();

            var allowOptionParsing = true;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg == EndOfOptionsDelimiter)
                {
                    allowOptionParsing = false;
                    continue;
                }

                var argToken = new ParameterToken(ParameterTokenType.RawArg, i, arg, 0, substitutedToken);

                if (allowOptionParsing)
                {
                    var match = ParameterRegex.Match(arg);
                    if (match.Success)
                    {
                        var nameMatch = match.Groups["name"];
                        var valueMatch = match.Groups["value"];

                        tokens.Add(new ParameterToken(ParameterTokenType.Name, 0, nameMatch.Value, nameMatch.Index, argToken));

                        if (valueMatch.Length > 0)
                        {
                            tokens.Add(new ParameterToken(ParameterTokenType.AttachedValue, 0, valueMatch.Value, valueMatch.Index, argToken));
                        }

                        continue;
                    }

                    match = ParameterGroupRegex.Match(arg);
                    if (match.Success)
                    {
                        var groupMatch = match.Groups["group"];
                        var valueMatch = match.Groups["value"];

                        var groupToken = new ParameterToken(ParameterTokenType.NameGroup, 0, groupMatch.Value.ToString(), groupMatch.Index, argToken);

                        for (var j = 0; j < groupMatch.Length; j++)
                        {
                            tokens.Add(new ParameterToken(ParameterTokenType.ShortName, j, groupMatch.Value[j].ToString(), j, groupToken));
                        }

                        if (valueMatch.Length > 0)
                        {
                            tokens.Add(new ParameterToken(ParameterTokenType.AttachedValue, 0, valueMatch.Value, valueMatch.Index, argToken));
                        }

                        continue;
                    }
                }

                tokens.Add(new ParameterToken(ParameterTokenType.Value, 0, arg, 0, argToken));
            }

            return tokens;
        }
    }

    // DOS-style lexer
    public partial class DosParametersLexer : IParametersLexer
    {
        private static readonly Regex ParameterRegex = new("^/(?<name>[^=:\\s]+)(([=:]|\\s+)(?<value>.*))?$");

        public static readonly DosParametersLexer Instance = new();

        public IArgFormatter ArgFormatter { get; }

        private DosParametersLexer()
        {
            this.ArgFormatter = DosArgFormatter.Instance;
        }

        public IEnumerable<IParameterToken> GetTokens(string[] args, IParameterToken? substitutedToken = null)
        {
            var tokens = new List<IParameterToken>();

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                var argToken = new ParameterToken(ParameterTokenType.RawArg, i, arg, 0, substitutedToken);

                var match = ParameterRegex.Match(arg);
                if (match.Success)
                {
                    var nameMatch = match.Groups["name"];
                    var valueMatch = match.Groups["value"];

                    tokens.Add(new ParameterToken(ParameterTokenType.Name, 0, nameMatch.Value, nameMatch.Index, argToken));

                    if (valueMatch.Length > 0)
                    {
                        tokens.Add(new ParameterToken(ParameterTokenType.AttachedValue, 0, valueMatch.Value, valueMatch.Index, argToken));
                    }

                    continue;
                }

                tokens.Add(new ParameterToken(ParameterTokenType.Value, 0, arg, 0, argToken));
            }

            return tokens;
        }
    }
}
