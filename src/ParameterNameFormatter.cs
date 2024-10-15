namespace SoftPatterns.Cli.Reader
{
    public interface IParameterNameFormatter
    {
        string FormatParameter(IParameterToken nameToken, string? name);
        string FormatSwitch(IParameterToken nameToken, string? name);
    }

    public class ParameterNameFormatter : IParameterNameFormatter
    {
        private readonly IArgFormatter argFormatter;

        public ParameterNameFormatter(IArgFormatter? argFormatter = null)
        {
            this.argFormatter = argFormatter ?? ArgFormatter.Instance;
        }

        public string FormatParameter(IParameterToken nameToken, string? name)
        {
            return Format(nameToken, name, "parameter");
        }

        public string FormatSwitch(IParameterToken nameToken, string? name)
        {
            return Format(nameToken, name, "switch");
        }

        private string Format(IParameterToken nameToken, string? name, string parameterTypeName)
        {
            var isShortName = nameToken.Type == ParameterTokenType.ShortName;
            var isOnlyShortNameInGroup = isShortName && nameToken.Value == nameToken.SubstitutedToken?.Value;

            var nameGroup = isShortName ? this.argFormatter.FormatNameGroupArg(nameToken.SubstitutedToken!.Value) : null;

            if (!isShortName || name == null && isOnlyShortNameInGroup)
            {
                // -p parameter
                // --param parameter
                return $"{(isShortName ? nameGroup : this.argFormatter.FormatNameArg(nameToken.Value))} {parameterTypeName}";
            }

            return
                // param parameter (-p)
                isOnlyShortNameInGroup ? $"{name} {parameterTypeName} ({nameGroup})" :
                // param parameter ('p' at -pxyz parameter group)
                name != null ? $"{name} {parameterTypeName} ('{nameToken.Value}' at '{nameGroup}' parameter group)" :
                // 'p' parameter (at -pxyz parameter group)
                $"'{nameToken.Value}' {parameterTypeName} (at '{nameGroup}' parameter group)";
        }
    }
}
