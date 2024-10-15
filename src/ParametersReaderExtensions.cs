namespace SoftPatterns.Cli.Reader
{
    public static class ParametersReaderExtensions
    {
        #region Parameters default values

        public static bool TryPeekValue(this IParametersReader reader, [MaybeNullWhen(false)] out IParameterToken valueToken)
        {
            return reader.TryPeekValue(null, out valueToken);
        }

        public static bool TryMatchValue(this IParametersReader reader, [MaybeNullWhen(false)] out IParameterToken valueToken)
        {
            return reader.TryMatchValue(null, out valueToken);
        }

        #endregion

        #region Expand predicate parameters

        public static bool TryPeekParameter(this IParametersReader reader, string name, string? shortName, [MaybeNullWhen(false)] out IParameterToken nameToken)
        {
            return reader.TryPeekParameter(new NamePredicate(name, shortName), out nameToken);
        }

        public static bool TryMatchParameter(this IParametersReader reader, string name, string? shortName, [MaybeNullWhen(false)] out IParameterToken nameToken)
        {
            return reader.TryMatchParameter(new NamePredicate(name, shortName), out nameToken);
        }

        public static IParameterToken MatchParameter(this IParametersReader reader, string name, string? shortName = null, bool isVisible = true, StringComparison? nameComparison = null)
        {
            return reader.MatchParameter(new NamePredicate(name, shortName, isVisible, nameComparison));
        }

        public static bool TrySubstituteParameter(this IParametersReader reader, string oldName, string newName, bool isVisible = false, StringComparison? nameComparison = null)
        {
            return reader.TrySubstituteParameter(new NamePredicate(oldName, null, isVisible, nameComparison), newName);
        }

        public static bool TrySubstituteParameter(this IParametersReader reader, string oldName, string? oldShortName, string newName, bool isVisible = false, StringComparison? nameComparison = null)
        {
            return reader.TrySubstituteParameter(new NamePredicate(oldName, oldShortName, isVisible, nameComparison), newName);
        }

        public static bool TrySubstituteValue(this IParametersReader reader, string oldValue, string newValue, bool isVisible = false, StringComparison? valueComparison = null)
        {
            return reader.TrySubstituteValue(new ValuePredicate(oldValue, isVisible, valueComparison), newValue);
        }

        public static bool TrySubstituteValueArgs(this IParametersReader reader, string oldValue, string[] newArgs, bool isVisible = false, StringComparison? valueComparison = null)
        {
            return reader.TrySubstituteValueArgs(new ValuePredicate(oldValue, isVisible, valueComparison), newArgs);
        }

        #endregion

        #region Read a single value parameter

        public static string ReadParameterValue(this IParametersReader reader, string name, string? shortName, ValuePredicate? value = null, bool isVisible = true, StringComparison? nameComparison = null)
        {
            return reader.ReadParameterValue(new NamePredicate(name, shortName, isVisible, nameComparison), value);
        }

        public static string ReadParameterValue(this IParametersReader reader, NamePredicate name, ValuePredicate? value = null)
        {
            reader.MatchParameter(name);
            var valueToken = reader.MatchValue(value);
            reader.MatchParameterEnd();
            return valueToken.Value;
        }

        public static string? ReadParameterValue(this IParametersReader reader, string name, string? shortName, string? defaultValue, ValuePredicate? value = null, bool isVisible = true, StringComparison? nameComparison = null)
        {
            return reader.ReadParameterValue(new NamePredicate(name, shortName, isVisible, nameComparison), defaultValue, value);
        }

        public static string? ReadParameterValue(this IParametersReader reader, NamePredicate name, string? defaultValue, ValuePredicate? value = null)
        {
            if (reader.TryMatchParameter(name, out _))
            {
                var valueToken = reader.MatchValue(value);
                reader.MatchParameterEnd();
                return valueToken.Value;
            }

            return defaultValue;
        }

        public static T ReadParameterValue<T>(this IParametersReader reader, string name, string? shortName, Parser<T> parser, ValueTypeDescription? valueTypeDescription = null, bool isVisible = true, StringComparison? nameComparison = null)
        {
            return reader.ReadParameterValue(new NamePredicate(name, shortName, isVisible, nameComparison), parser, valueTypeDescription);
        }

        public static T ReadParameterValue<T>(this IParametersReader reader, NamePredicate name, Parser<T> parser, ValueTypeDescription? valueTypeDescription = null)
        {
            reader.MatchParameter(name);
            var result = reader.ReadValue(parser, valueTypeDescription);
            reader.MatchParameterEnd();
            return result;
        }

        public static T ReadParameterValue<T>(this IParametersReader reader, string name, string? shortName, Parser<T> parser, T defaultValue, ValueTypeDescription? valueTypeDescription = null, bool isVisible = true, StringComparison? nameComparison = null)
        {
            return reader.ReadParameterValue(new NamePredicate(name, shortName, isVisible, nameComparison), parser, defaultValue, valueTypeDescription);
        }

        public static T ReadParameterValue<T>(this IParametersReader reader, NamePredicate name, Parser<T> parser, T defaultValue, ValueTypeDescription? valueTypeDescription = null)
        {
            if (reader.TryMatchParameter(name, out var nameToken))
            {
                var result = reader.ReadValue(parser, valueTypeDescription);
                reader.MatchParameterEnd();
                return result;
            }

            return defaultValue;
        }

        #endregion

        #region Additional read methods

        public static bool TryReadValue(this IParametersReader reader, ValuePredicate value)
        {
            return reader.TryMatchValue(value, out _);
        }

        public static bool TryReadValue(this IParametersReader reader, string value, bool isVisible = true, StringComparison? valueComparison = null)
        {
            return reader.TryMatchValue(new ValuePredicate(value, isVisible, valueComparison), out _);
        }

        public static string ReadValue(this IParametersReader reader)
        {
            return reader.MatchValue(null).Value;
        }

        public static string ReadValue(this IParametersReader reader, ValuePredicate value)
        {
            return reader.MatchValue(value).Value;
        }

        public static string ReadValue(this IParametersReader reader, ValueTypeDescription valueTypeDescription)
        {
            return reader.MatchValue(null, valueTypeDescription).Value;
        }

        public static string? ReadValue(this IParametersReader reader, string? defaultValue, ValuePredicate? value = null)
        {
            return reader.TryMatchValue(value, out var token) ? token.Value : defaultValue;
        }

        public static bool ReadSwitch(this IParametersReader reader, string name, string? shortName, bool isVisible = true, StringComparison? nameComparison = null)
        {
            return reader.ReadSwitch(new NamePredicate(name, shortName, isVisible, nameComparison));
        }

        public static bool ReadSwitch(this IParametersReader reader, NamePredicate name)
        {
            if (reader.TryMatchParameter(name, out _))
            {
                reader.MatchParameterEnd();
                return true;
            }

            return false;
        }

        #endregion
    }
}
