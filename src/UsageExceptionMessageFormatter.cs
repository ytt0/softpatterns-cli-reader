namespace SoftPatterns.Cli.Reader
{
    public interface IUsageExceptionMessageFormatter
    {
        string FormatMissingValue(IParameterToken? previousValueToken, IParameterToken? nextToken, ValueTypeDescription? valueTypeDescription);
        string FormatMissingParameter(string name);
        string FormatMissingParameterValue(string name, IParameterToken nameToken, IParameterToken? previousValueToken, IParameterToken? nextToken, ValueTypeDescription? valueTypeDescription);
        string FormatUnexpectedValue(IParameterToken valueToken);
        string FormatUnexpectedParameter(IParameterToken nameToken, IParameterToken valueToken, IEnumerable<NamePredicate> names);
        string FormatUnexpectedSwitch(IParameterToken nameToken, IEnumerable<NamePredicate> names);
        string FormatUnexpectedSwitchValue(string name, IParameterToken nameToken, IParameterToken valueToken);
        string FormatUnexpectedRepeatingParameter(string name, IParameterToken nameToken);
        string FormatUnexpectedRepeatingSwitch(string name, IParameterToken nameToken);
        string FormatInvalidValue(IParameterToken valueToken, ValueTypeDescription? valueTypeDescription, Exception? innerException);
        string FormatInvalidParameterValue(string name, IParameterToken nameToken, IParameterToken valueToken, ValueTypeDescription? valueTypeDescription, Exception? innerException = null);
    }

    public class UsageExceptionMessageFormatter : IUsageExceptionMessageFormatter
    {
        private readonly IArgFormatter argFormatter;
        private readonly IParameterNameFormatter nameFormatter;
        private readonly ISimilarityComparer similarityComparer;
        private readonly StringComparison defaultNameComparison;

        public UsageExceptionMessageFormatter(IArgFormatter? argFormatter = null, IParameterNameFormatter? nameFormatter = null, ISimilarityComparer? similarityComparer = null, StringComparison defaultNameComparison = default)
        {
            this.argFormatter = argFormatter ?? ArgFormatter.Instance;
            this.nameFormatter = nameFormatter ?? new ParameterNameFormatter(this.argFormatter);
            this.similarityComparer = similarityComparer ?? new LcsSimilarityComparer(2);
            this.defaultNameComparison = defaultNameComparison;
        }

        public string FormatMissingValue(IParameterToken? previousValueToken, IParameterToken? nextToken, ValueTypeDescription? valueTypeDescription)
        {
            var valueTypeName = valueTypeDescription?.ValueTypeName ?? ValueTypeName.Value;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(
                previousValueToken != null ? $"{valueTypeName.Name} is missing after '{GetSourceValue(previousValueToken)}'" :
                nextToken != null ? $"{valueTypeName.Name} is missing before '{GetSourceValue(nextToken)}'" :
                $"{valueTypeName.Name} is missing");

            stringBuilder.Append(FormatValuesList(null, valueTypeDescription?.AvailableValues, valueTypeName));

            return stringBuilder.ToString();
        }

        public string FormatMissingParameter(string name)
        {
            return $"{this.argFormatter.FormatNameArg(name)} parameter is missing";
        }

        public string FormatMissingParameterValue(string name, IParameterToken nameToken, IParameterToken? previousValueToken, IParameterToken? nextToken, ValueTypeDescription? valueTypeDescription)
        {
            var valueTypeName = valueTypeDescription?.ValueTypeName ?? ValueTypeName.Value;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(
                previousValueToken != null ? $"{this.nameFormatter.FormatParameter(nameToken, name)} additional {valueTypeName.Name} is missing after '{GetSourceValue(previousValueToken)}'" :
                nextToken != null ? $"{this.nameFormatter.FormatParameter(nameToken, name)} {valueTypeName.Name} is missing before '{GetSourceValue(nextToken)}'" :
                $"{this.nameFormatter.FormatParameter(nameToken, name)} {valueTypeName.Name} is missing");

            stringBuilder.Append(FormatValuesList(null, valueTypeDescription?.AvailableValues, valueTypeName));

            return stringBuilder.ToString();
        }

        public string FormatUnexpectedValue(IParameterToken valueToken)
        {
            return $"value '{valueToken.Value}' is unexpected";
        }

        public string FormatUnexpectedParameter(IParameterToken nameToken, IParameterToken valueToken, IEnumerable<NamePredicate> names)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"Unexpected {this.nameFormatter.FormatParameter(nameToken, null)}");

            if (valueToken.Type != ParameterTokenType.AttachedValue)
            {
                stringBuilder.Append($" '{valueToken.Value}'");
            }

            var availableNames = names.SelectMany(name => name.AvailableNames ?? []).Distinct().ToArray();
            if (availableNames.Length > 0)
            {
                var similarNames = this.similarityComparer.GetSimilarValues(nameToken.Value, availableNames).Select(this.argFormatter.FormatNameArg).ToArray();

                stringBuilder.Append(FormatValuesList(similarNames, null, "parameter"));
            }

            return stringBuilder.ToString();
        }

        public string FormatUnexpectedSwitch(IParameterToken nameToken, IEnumerable<NamePredicate> names)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"Unexpected {this.nameFormatter.FormatSwitch(nameToken, null)}");

            if (nameToken.Type == ParameterTokenType.ShortName && nameToken.SubstitutedToken?.Type == ParameterTokenType.NameGroup && nameToken.Value != nameToken.SubstitutedToken.Value)
            {
                var longNameToken = new ParameterToken(ParameterTokenType.Name, 0, nameToken.SubstitutedToken.Value, 0, nameToken.SubstitutedToken.SubstitutedToken);
                if (names.Any(name => name.Match(longNameToken, this.defaultNameComparison)))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append($"Did you mean '{this.argFormatter.FormatNameArg(longNameToken.Value)}'?");

                    return stringBuilder.ToString();
                }
            }

            var availableNames = names.SelectMany(name => name.AvailableNames ?? []).Distinct().ToArray();
            if (availableNames.Length > 0)
            {
                var similarNames = this.similarityComparer.GetSimilarValues(nameToken.Value, availableNames).Select(this.argFormatter.FormatNameArg).ToArray();

                stringBuilder.Append(FormatValuesList(similarNames, null, "parameter"));
            }

            return stringBuilder.ToString();
        }

        public string FormatUnexpectedSwitchValue(string name, IParameterToken nameToken, IParameterToken valueToken)
        {
            return $"Unexpected {this.nameFormatter.FormatSwitch(nameToken, name)} value '{valueToken.Value}'";
        }

        public string FormatUnexpectedRepeatingParameter(string name, IParameterToken nameToken)
        {
            return $"Unexpected repeating {this.nameFormatter.FormatParameter(nameToken, name)}";
        }

        public string FormatUnexpectedRepeatingSwitch(string name, IParameterToken nameToken)
        {
            return $"Unexpected repeating {this.nameFormatter.FormatSwitch(nameToken, name)}";
        }

        public string FormatInvalidValue(IParameterToken valueToken, ValueTypeDescription? valueTypeDescription, Exception? innerException)
        {
            var valueTypeName = valueTypeDescription?.ValueTypeName ?? ValueTypeName.Value;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{valueTypeName.Name} '{valueToken.Value}' is invalid");

            if (innerException != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(innerException.Message);
            }

            if (valueTypeDescription?.AvailableValues?.Any() == true)
            {
                var similarValues = this.similarityComparer.GetSimilarValues(valueToken.Value, valueTypeDescription.AvailableValues);

                stringBuilder.Append(FormatValuesList(similarValues, valueTypeDescription.AvailableValues, valueTypeName));
            }

            return stringBuilder.ToString();

        }

        public string FormatInvalidParameterValue(string name, IParameterToken nameToken, IParameterToken valueToken, ValueTypeDescription? valueTypeDescription, Exception? innerException = null)
        {
            var valueTypeName = valueTypeDescription?.ValueTypeName ?? ValueTypeName.Value;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{nameFormatter.FormatParameter(nameToken, name)} {valueTypeName.Name} '{valueToken.Value}' is invalid");

            if (innerException != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(innerException.Message);
            }

            if (valueTypeDescription?.AvailableValues?.Any() == true)
            {
                var similarValues = this.similarityComparer.GetSimilarValues(valueToken.Value, valueTypeDescription.AvailableValues);

                stringBuilder.Append(FormatValuesList(similarValues, valueTypeDescription.AvailableValues, valueTypeName));
            }

            return stringBuilder.ToString();
        }

        private static string FormatValuesList(IEnumerable<string>? similarValues, IEnumerable<string>? availableValues, ValueTypeName valueTypeName)
        {
            var stringBuilder = new StringBuilder();

            if (similarValues != null && similarValues.Any())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append(similarValues.Count() > 1 ? $"The most similar {valueTypeName.NamePlural} are:" : $"The most similar {valueTypeName.Name} is:");

                foreach (var value in similarValues)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append("    ");
                    stringBuilder.Append(value);
                }
            }
            else if (availableValues != null && availableValues.Any())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append(availableValues.Count() > 1 ? $"Available {valueTypeName.NamePlural} are:" : $"The most similar {valueTypeName.Name} is:");

                foreach (var value in availableValues)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append("    ");
                    stringBuilder.Append(value);
                }
            }

            return stringBuilder.ToString();
        }

        private static string GetSourceValue(IParameterToken token)
        {
            while (token.SubstitutedToken != null)
            {
                token = token.SubstitutedToken;
            }

            return token.Value;
        }
    }
}
