namespace SoftPatterns.Cli.Reader
{
    public class UsageException : Exception
    {
        public UsageException(string message, Exception? innerException = null) :
            base(message, innerException)
        {
        }
    }

    public class MissingValueException : UsageException
    {
        public IParameterToken? PreviousValueToken { get; }
        public IParameterToken? NextToken { get; }
        public ValueTypeDescription? ValueTypeDescription { get; }

        public MissingValueException(IParameterToken? previousValueToken, IParameterToken? nextToken, ValueTypeDescription? valueTypeDescription, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(previousValueToken, nextToken, valueTypeDescription, messageFormatter?.FormatMissingValue(previousValueToken, nextToken, valueTypeDescription) ?? "Value is missing", innerException)
        {
        }

        public MissingValueException(IParameterToken? previousValueToken, IParameterToken? nextToken, ValueTypeDescription? valueTypeDescription, string message, Exception? innerException = null) :
            base(message, innerException)
        {
            this.PreviousValueToken = previousValueToken;
            this.NextToken = nextToken;
            this.ValueTypeDescription = valueTypeDescription;
        }
    }

    public class MissingParameterException : UsageException
    {
        public string Name { get; }

        public MissingParameterException(string name, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(name, messageFormatter?.FormatMissingParameter(name) ?? $"Parameter \"{name}\" is missing", innerException)
        {
        }

        public MissingParameterException(string name, string message, Exception? innerException = null) :
            base(message, innerException)
        {
            this.Name = name;
        }
    }

    public class MissingParameterValueException : MissingValueException
    {
        public string Name { get; }
        public IParameterToken NameToken { get; }

        public MissingParameterValueException(string name, IParameterToken nameToken, IParameterToken? previousValueToken, IParameterToken? nextToken, ValueTypeDescription? valueTypeDescription, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(name, nameToken, previousValueToken, nextToken, valueTypeDescription, messageFormatter?.FormatMissingParameterValue(name, nameToken, previousValueToken, nextToken, valueTypeDescription) ?? $"Parameter \"{nameToken.Value}\" value is missing", innerException)
        {
        }

        public MissingParameterValueException(string name, IParameterToken nameToken, IParameterToken? previousValueToken, IParameterToken? nextToken, ValueTypeDescription? valueTypeDescription, string message, Exception? innerException = null) :
            base(previousValueToken, nextToken, valueTypeDescription, message, innerException)
        {
            this.Name = name;
            this.NameToken = nameToken;
        }
    }

    public class UnexpectedValueException : UsageException
    {
        public IParameterToken ValueToken { get; }

        public UnexpectedValueException(IParameterToken valueToken, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(valueToken, messageFormatter?.FormatUnexpectedValue(valueToken) ?? $"Unexpected value \"{valueToken.Value}\"", innerException)
        {
        }

        public UnexpectedValueException(IParameterToken valueToken, string message, Exception? innerException = null) :
            base(message, innerException)
        {
            this.ValueToken = valueToken;
        }
    }

    public class UnexpectedParameterException : UsageException
    {
        public IParameterToken NameToken { get; }
        public IParameterToken ValueToken { get; }
        public IEnumerable<NamePredicate> MatchedNames { get; }

        public UnexpectedParameterException(IParameterToken nameToken, IParameterToken valueToken, IEnumerable<NamePredicate> matchedNames, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(nameToken, valueToken, matchedNames, messageFormatter?.FormatUnexpectedParameter(nameToken, valueToken, matchedNames) ?? $"Unexpected parameter \"{nameToken.Value}\" (value \"{valueToken.Value}\")", innerException)
        {
            this.NameToken = nameToken;
            this.ValueToken = valueToken;
            this.MatchedNames = matchedNames;
        }

        public UnexpectedParameterException(IParameterToken nameToken, IParameterToken valueToken, IEnumerable<NamePredicate> matchedNames, string message, Exception? innerException = null) :
            base(message, innerException)
        {
            this.NameToken = nameToken;
            this.ValueToken = valueToken;
            this.MatchedNames = matchedNames;
        }
    }

    public class UnexpectedSwitchException : UsageException
    {
        public IParameterToken NameToken { get; }
        public IEnumerable<NamePredicate> MatchedNames { get; }

        public UnexpectedSwitchException(IParameterToken nameToken, IEnumerable<NamePredicate> matchedNames, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(nameToken, matchedNames, messageFormatter?.FormatUnexpectedSwitch(nameToken, matchedNames) ?? $"Unexpected switch \"{nameToken.Value}\"", innerException)
        {
        }

        public UnexpectedSwitchException(IParameterToken nameToken, IEnumerable<NamePredicate> matchedNames, string message, Exception? innerException = null) :
            base(message, innerException)
        {
            this.NameToken = nameToken;
            this.MatchedNames = matchedNames;
        }
    }

    public class UnexpectedSwitchValueException : UsageException
    {
        public string Name { get; }
        public IParameterToken NameToken { get; }
        public IParameterToken ValueToken { get; }

        public UnexpectedSwitchValueException(string name, IParameterToken nameToken, IParameterToken valueToken, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(name, nameToken, valueToken, messageFormatter?.FormatUnexpectedSwitchValue(name, nameToken, valueToken) ?? $"Unexpected switch \"{nameToken.Value}\" value \"{valueToken.Value}\"", innerException)
        {
            this.NameToken = nameToken;
            this.ValueToken = valueToken;
            this.Name = name;
        }

        public UnexpectedSwitchValueException(string name, IParameterToken nameToken, IParameterToken valueToken, string message, Exception? innerException = null) :
            base(message, innerException)
        {
            this.NameToken = nameToken;
            this.ValueToken = valueToken;
            this.Name = name;
        }
    }

    public class UnexpectedRepeatingParameterException : UsageException
    {
        public string Name { get; }
        public IParameterToken NameToken { get; }

        public UnexpectedRepeatingParameterException(string name, IParameterToken nameToken, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(name, nameToken, messageFormatter?.FormatUnexpectedRepeatingParameter(name, nameToken) ?? $"Unexpected repeating parameter \"{nameToken.Value}\"", innerException)
        {
            this.NameToken = nameToken;
            this.Name = name;
        }

        public UnexpectedRepeatingParameterException(string name, IParameterToken nameToken, string message, Exception? innerException = null) :
            base(message, innerException)
        {
            this.NameToken = nameToken;
            this.Name = name;
        }
    }

    public class UnexpectedRepeatingSwitchException : UsageException
    {
        public string Name { get; }
        public IParameterToken NameToken { get; }

        public UnexpectedRepeatingSwitchException(string name, IParameterToken nameToken, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(name, nameToken, messageFormatter?.FormatUnexpectedRepeatingSwitch(name, nameToken) ?? $"Unexpected repeating switch \"{nameToken.Value}\"", innerException)
        {
            this.NameToken = nameToken;
            this.Name = name;
        }

        public UnexpectedRepeatingSwitchException(string name, IParameterToken nameToken, string message, Exception? innerException = null) :
            base(message, innerException)
        {
            this.NameToken = nameToken;
            this.Name = name;
        }
    }

    public class InvalidValueException : UsageException
    {
        public IParameterToken ValueToken { get; }
        public ValueTypeDescription? ValueTypeDescription { get; }

        public InvalidValueException(IParameterToken valueToken, ValueTypeDescription? valueTypeDescription, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(valueToken, valueTypeDescription, messageFormatter?.FormatInvalidValue(valueToken, valueTypeDescription, innerException) ?? $"Invalid value \"{valueToken.Value}\"", innerException)
        {
        }

        public InvalidValueException(IParameterToken valueToken, ValueTypeDescription? valueTypeDescription, string message, Exception? innerException = null) :
            base(message, innerException)
        {
            this.ValueToken = valueToken;
            this.ValueTypeDescription = valueTypeDescription;
        }
    }

    public class InvalidParameterValueException : InvalidValueException
    {
        public string Name { get; }
        public IParameterToken NameToken { get; }

        public InvalidParameterValueException(string name, IParameterToken nameToken, IParameterToken valueToken, ValueTypeDescription? valueTypeDescription, IUsageExceptionMessageFormatter? messageFormatter = null, Exception? innerException = null) :
            this(name, nameToken, valueToken, valueTypeDescription, messageFormatter?.FormatInvalidParameterValue(name, nameToken, valueToken, valueTypeDescription, innerException) ?? $"Invalid parameter {name} value \"{valueToken.Value}\"", innerException)
        {
            this.Name = name;
            this.NameToken = nameToken;
        }

        public InvalidParameterValueException(string name, IParameterToken nameToken, IParameterToken valueToken, ValueTypeDescription? valueTypeDescription, string message, Exception? innerException = null) :
            base(valueToken, valueTypeDescription, message, innerException)
        {
            this.Name = name;
            this.NameToken = nameToken;
        }
    }
}
