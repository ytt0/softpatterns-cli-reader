namespace SoftPatterns.Cli.Reader
{
    public enum ParameterTokenType
    {
        None,
        RawArg,
        Name,
        ShortName,
        NameGroup,
        Value,
        AttachedValue
    }

    public interface IParameterToken
    {
        ParameterTokenType Type { get; }
        int Index { get; }
        string Value { get; }
        int ValueStartIndex { get; }
        IParameterToken? SubstitutedToken { get; }
    }

    public record ParameterToken(ParameterTokenType Type, int Index, string Value, int ValueStartIndex, IParameterToken? SubstitutedToken) : IParameterToken;
}
