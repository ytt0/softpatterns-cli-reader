namespace SoftPatterns.Cli.Reader
{
    public delegate T Parser<T>(string value);

    public interface IParametersReader
    {
        bool TryPeekValue(ValuePredicate? value, [MaybeNullWhen(false)] out IParameterToken valueToken);
        bool TryMatchValue(ValuePredicate? value, [MaybeNullWhen(false)] out IParameterToken valueToken);
        IParameterToken MatchValue(ValuePredicate? value = null, ValueTypeDescription? valueTypeDescription = null);

        T ReadValue<T>(Parser<T> parser, ValueTypeDescription? valueTypeDescription = null);
        T ReadValue<T>(Parser<T> parser, T defaultValue, ValueTypeDescription? valueTypeDescription = null);

        bool TryPeekParameter(NamePredicate name, [MaybeNullWhen(false)] out IParameterToken nameToken);
        bool TryMatchParameter(NamePredicate name, [MaybeNullWhen(false)] out IParameterToken nameToken);
        IParameterToken MatchParameter(NamePredicate name);
        void MatchParameterEnd();

        bool TrySubstituteParameter(NamePredicate oldName, string newName);
        bool TrySubstituteValue(ValuePredicate oldValue, string newValue);
        bool TrySubstituteValueArgs(ValuePredicate oldValue, string[] newArgs);

        void ValidateEmpty();

        void ThrowUnmatchedValueException(ValueTypeName? valueTypeName = null);
    }

    public class ParametersReader : IParametersReader
    {
        private record TokenMatch(IParameterToken Token) { public bool IsMatched { get; set; } }

        private readonly IParametersLexer lexer;
        private readonly StringComparison defaultNameComparison;
        private readonly StringComparison defaultValueComparison;
        private readonly IUsageExceptionMessageFormatter messageFormatter;
        private readonly List<TokenMatch> stream;

        private readonly List<NamePredicate> names;
        private readonly List<ValuePredicate> values;
        private readonly List<ValuePredicate> parameterValues;

        private bool isParameterMatch;
        private int parameterIndex;
        private int parameterValueIndex;
        private string? parameterName;
        private IParameterToken? parameterNameToken;
        private IParameterToken? parameterPreviousValueToken;
        private IParameterToken? previousValueToken;
        private int valueIndex;

        public ParametersReader(string[] args, IParametersLexer? lexer = null, StringComparison defaultNameComparison = default, StringComparison defaultValueComparison = default, IUsageExceptionMessageFormatter? messageFormatter = null)
        {
            this.lexer = lexer ?? ParametersLexer.Instance;
            this.defaultNameComparison = defaultNameComparison;
            this.defaultValueComparison = defaultValueComparison;
            this.messageFormatter = messageFormatter ?? new UsageExceptionMessageFormatter(this.lexer.ArgFormatter, null, null, this.defaultNameComparison);
            this.names = [];
            this.values = [];
            this.parameterValues = [];

            this.stream = this.lexer.GetTokens(args).Select(token => new TokenMatch(ValidateTokenType(token))).ToList();
        }

        public bool TryPeekValue(ValuePredicate? value, [MaybeNullWhen(false)] out IParameterToken valueToken)
        {
            AddValue(value);

            if (TryGetValueToken(out var token) && value?.Match(token, this.defaultValueComparison) != false)
            {
                valueToken = token;
                return true;
            }

            valueToken = default;
            return false;
        }

        public bool TryMatchValue(ValuePredicate? value, [MaybeNullWhen(false)] out IParameterToken valueToken)
        {
            AddValue(value);

            if (TryGetValueToken(out var token) && value?.Match(token, this.defaultValueComparison) != false)
            {
                MatchValueToken();
                valueToken = token;
                return true;
            }

            valueToken = default;
            return false;
        }

        public IParameterToken MatchValue(ValuePredicate? value = null, ValueTypeDescription? valueTypeDescription = null)
        {
            if (TryGetValueToken(out var token))
            {
                if (value?.Match(token, this.defaultValueComparison) != false)
                {
                    MatchValueToken();
                    return token;
                }

                if (this.isParameterMatch)
                {
                    throw new InvalidParameterValueException(this.parameterName!, this.parameterNameToken!, token, valueTypeDescription ?? value.ValueTypeDescription, this.messageFormatter);
                }

                throw new InvalidValueException(token, valueTypeDescription ?? value.ValueTypeDescription, this.messageFormatter);
            }

            var index = this.isParameterMatch ? this.parameterValueIndex : this.valueIndex;
            var nextToken = index < this.stream.Count ? this.stream[index].Token : null;

            if (this.isParameterMatch)
            {
                throw new MissingParameterValueException(this.parameterName!, this.parameterNameToken!, this.parameterPreviousValueToken, nextToken, valueTypeDescription ?? value?.ValueTypeDescription, this.messageFormatter);
            }

            throw new MissingValueException(this.previousValueToken, nextToken, valueTypeDescription ?? value?.ValueTypeDescription, this.messageFormatter);
        }

        public T ReadValue<T>(Parser<T> parser, ValueTypeDescription? valueTypeDescription = null)
        {
            return ParseValueToken(MatchValue(null, valueTypeDescription), parser, valueTypeDescription);
        }

        public T ReadValue<T>(Parser<T> parser, T defaultValue, ValueTypeDescription? valueTypeDescription = null)
        {
            return TryMatchValue(null, out var valueToken) ? ParseValueToken(valueToken, parser, valueTypeDescription) : defaultValue;
        }

        public bool TryPeekParameter(NamePredicate name, [MaybeNullWhen(false)] out IParameterToken nameToken)
        {
            if (this.isParameterMatch)
            {
                throw new InvalidOperationException($"Parameter {this.parameterName} is currently being matched");
            }

            this.names.Add(name);

            nameToken = this.stream.FirstOrDefault(tokenMatch => !tokenMatch.IsMatched && (tokenMatch.Token.Type == ParameterTokenType.Name || tokenMatch.Token.Type == ParameterTokenType.ShortName) && name.Match(tokenMatch.Token, this.defaultNameComparison))?.Token;
            return nameToken != null;
        }

        public bool TryMatchParameter(NamePredicate name, [MaybeNullWhen(false)] out IParameterToken nameToken)
        {
            if (this.isParameterMatch)
            {
                throw new InvalidOperationException($"Parameter {this.parameterName} is currently being matched");
            }

            this.names.Add(name);

            var index = this.stream.FindIndex(tokenMatch => !tokenMatch.IsMatched && (tokenMatch.Token.Type == ParameterTokenType.Name || tokenMatch.Token.Type == ParameterTokenType.ShortName) && name.Match(tokenMatch.Token, this.defaultNameComparison));
            if (index != -1)
            {
                var tokenMatch = this.stream[index];

                this.isParameterMatch = true;
                this.parameterIndex = index;
                this.parameterValueIndex = index + 1;
                this.parameterName = name.Name;
                this.parameterNameToken = tokenMatch.Token;
                this.parameterPreviousValueToken = null;
                this.parameterValues.Clear();

                tokenMatch.IsMatched = true;

                nameToken = tokenMatch.Token;
                return true;
            }

            nameToken = default;
            return false;
        }

        public IParameterToken MatchParameter(NamePredicate name)
        {
            if (this.isParameterMatch)
            {
                throw new InvalidOperationException($"Parameter {this.parameterName} is currently being matched");
            }

            return TryMatchParameter(name, out var nameToken) ? nameToken : throw new MissingParameterException(name.Name, this.messageFormatter);
        }

        public void MatchParameterEnd()
        {
            if (!this.isParameterMatch)
            {
                throw new InvalidOperationException("No parameter is currently being matched");
            }

            if (TryGetValueToken(out var valueToken) && valueToken.Type == ParameterTokenType.AttachedValue)
            {
                throw new UnexpectedSwitchValueException(this.parameterName!, this.parameterNameToken!, valueToken, this.messageFormatter);
            }

            this.isParameterMatch = false;
            this.parameterIndex = 0;
            this.parameterValueIndex = 0;
            this.parameterName = null;
            this.parameterNameToken = null;
            this.parameterPreviousValueToken = null;
            this.parameterValues.Clear();

            this.valueIndex = this.stream.TakeWhile(tokenMatch => tokenMatch.IsMatched).Count();
        }

        public bool TrySubstituteParameter(NamePredicate oldName, string newName)
        {
            if (TryMatchParameter(oldName, out var oldNameToken))
            {
                var newNameToken = new ParameterToken(ParameterTokenType.Name, 0, newName, 0, oldNameToken);
                this.stream[this.parameterIndex] = new TokenMatch(ValidateTokenType(newNameToken));
                MatchParameterEnd();
                return true;
            }

            return false;
        }

        public bool TrySubstituteValue(ValuePredicate oldValue, string newValue)
        {
            if (TryPeekValue(oldValue, out var oldValueToken))
            {
                var newValueToken = new ParameterToken(ParameterTokenType.Value, 0, newValue, 0, oldValueToken);

                var index = this.isParameterMatch ? this.parameterValueIndex : this.valueIndex;
                this.stream[index] = new TokenMatch(ValidateTokenType(newValueToken));
                return true;
            }

            return false;
        }

        public bool TrySubstituteValueArgs(ValuePredicate oldValue, string[] newArgs)
        {
            if (TryPeekValue(oldValue, out var oldValueToken))
            {
                var index = this.isParameterMatch ? this.parameterValueIndex : this.valueIndex;
                var newArgsTokens = this.lexer.GetTokens(newArgs, oldValueToken);

                this.stream.RemoveAt(index);
                this.stream.InsertRange(index, newArgsTokens.Select(token => new TokenMatch(ValidateTokenType(token))).ToArray());
                return true;
            }

            return false;
        }

        public void ValidateEmpty()
        {
            if (this.isParameterMatch)
            {
                throw new InvalidOperationException($"Parameter \"{this.parameterName}\" is still being matched");
            }

            var tokens = this.stream.SkipWhile(tokenMatch => tokenMatch.IsMatched).Select(tokenMatch => tokenMatch.Token).Take(2);
            var token = tokens.FirstOrDefault();
            var nextToken = tokens.ElementAtOrDefault(1);

            if (token != null)
            {
                if (token.Type == ParameterTokenType.Name || token.Type == ParameterTokenType.ShortName)
                {
                    var name = this.names.FirstOrDefault(name => name.Match(token, this.defaultNameComparison));
                    if (name != null)
                    {
                        if (nextToken?.Type == ParameterTokenType.Value || nextToken?.Type == ParameterTokenType.AttachedValue)
                        {
                            throw new UnexpectedRepeatingParameterException(name.Name, token, this.messageFormatter);
                        }

                        throw new UnexpectedRepeatingSwitchException(name.Name, token, this.messageFormatter);
                    }

                    if (nextToken?.Type == ParameterTokenType.Value || nextToken?.Type == ParameterTokenType.AttachedValue)
                    {
                        throw new UnexpectedParameterException(token, nextToken, this.names, this.messageFormatter);
                    }

                    throw new UnexpectedSwitchException(token, this.names, this.messageFormatter);
                }

                if (token.Type == ParameterTokenType.Value || token.Type == ParameterTokenType.AttachedValue)
                {
                    throw new UnexpectedValueException(token, this.messageFormatter);
                }
            }
        }

        public void ThrowUnmatchedValueException(ValueTypeName? valueTypeName = null)
        {
            if (TryGetValueToken(out var token))
            {
                if (token.Type == ParameterTokenType.Value || this.isParameterMatch && token.Type == ParameterTokenType.AttachedValue)
                {
                    if (this.isParameterMatch)
                    {
                        throw new InvalidParameterValueException(this.parameterName!, this.parameterNameToken!, token, new ValueTypeDescription(valueTypeName, this.parameterValues.SelectMany(value => value.ValueTypeDescription?.AvailableValues ?? []).ToArray()), this.messageFormatter, null);
                    }

                    throw new InvalidValueException(token, new ValueTypeDescription(valueTypeName, this.values.SelectMany(value => value.ValueTypeDescription?.AvailableValues ?? []).ToArray()), this.messageFormatter, null);
                }
            }
            else
            {
                token = null;
            }

            if (this.isParameterMatch)
            {
                throw new MissingParameterValueException(this.parameterName!, this.parameterNameToken!, this.parameterPreviousValueToken, token, new ValueTypeDescription(valueTypeName, this.parameterValues.SelectMany(value => value.ValueTypeDescription?.AvailableValues ?? []).ToArray()), this.messageFormatter);
            }

            throw new MissingValueException(this.previousValueToken, token, new ValueTypeDescription(valueTypeName, this.values.SelectMany(value => value.ValueTypeDescription?.AvailableValues ?? []).ToArray()), this.messageFormatter);
        }

        private bool TryGetValueToken([MaybeNullWhen(false)] out IParameterToken valueToken)
        {
            var index = this.isParameterMatch ? this.parameterValueIndex : this.valueIndex;
            if (index < this.stream.Count)
            {
                var tokenMatch = this.stream[index];
                if (!tokenMatch.IsMatched && (tokenMatch.Token.Type == ParameterTokenType.Value || tokenMatch.Token.Type == ParameterTokenType.AttachedValue && this.isParameterMatch))
                {
                    valueToken = tokenMatch.Token;
                    return true;
                }
            }

            valueToken = null;
            return false;
        }

        private void MatchValueToken()
        {
            if (this.isParameterMatch)
            {
                var tokenMatch = this.stream[this.parameterValueIndex];
                tokenMatch.IsMatched = true;
                this.parameterPreviousValueToken = tokenMatch.Token;
                this.parameterValues.Clear();
                this.parameterValueIndex++;
            }
            else
            {
                var tokenMatch = this.stream[this.valueIndex];
                tokenMatch.IsMatched = true;
                this.previousValueToken = tokenMatch.Token;
                this.values.Clear();
                this.valueIndex = this.stream.TakeWhile(tokenMatch => tokenMatch.IsMatched).Count();
            }
        }

        private T ParseValueToken<T>(IParameterToken valueToken, Parser<T> parser, ValueTypeDescription? valueTypeDescription)
        {
            try
            {
                return parser(valueToken.Value);
            }
            catch (Exception e)
            {
                if (this.isParameterMatch)
                {
                    throw new InvalidParameterValueException(this.parameterName!, this.parameterNameToken!, valueToken, valueTypeDescription, this.messageFormatter, e);
                }

                throw new InvalidValueException(valueToken, valueTypeDescription, this.messageFormatter, e);
            }
        }

        private void AddValue(ValuePredicate? value)
        {
            if (value != null)
            {
                if (this.isParameterMatch)
                {
                    this.parameterValues.Add(value);
                }
                else
                {
                    this.values.Add(value);
                }
            }
        }

        private static IParameterToken ValidateTokenType(IParameterToken token)
        {
            return token.Type == ParameterTokenType.Name || token.Type == ParameterTokenType.ShortName || token.Type == ParameterTokenType.Value || token.Type == ParameterTokenType.AttachedValue ? token :
                throw new NotSupportedException($"Unavailable {nameof(ParameterTokenType)} \"{token.Type}\"");
        }
    }
}
