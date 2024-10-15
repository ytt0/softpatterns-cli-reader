namespace SoftPatterns.Cli.Reader
{
    public class NamePredicate
    {
        public string Name { get; }

        public virtual IEnumerable<string>? AvailableNames { get; private set; }

        private readonly string? shortName;
        private readonly StringComparison? nameComparison;

        public NamePredicate(string name, string? shortName = null, bool isVisible = true, StringComparison? nameComparison = null)
        {
            this.Name = name;
            this.AvailableNames = isVisible ? [name] : null;

            this.shortName = shortName == null || shortName.Length == 1 ? shortName : throw new ArgumentException($"Short name ('{shortName}') is expected to be a single character", nameof(shortName));
            this.nameComparison = nameComparison;
        }

        public virtual bool Match(IParameterToken token, StringComparison defaultComparison)
        {
            return token.Value.Equals(this.Name, this.nameComparison ?? defaultComparison) ||
                token.Type == ParameterTokenType.ShortName && token.Value.Equals(this.shortName, this.nameComparison ?? defaultComparison);
        }

        public static implicit operator NamePredicate(string name)
        {
            return new NamePredicate(name);
        }
    }

    public class ValuePredicate
    {
        public virtual ValueTypeDescription? ValueTypeDescription { get; private set; }

        private readonly string? value;
        private readonly StringComparison? valueComparison;

        public ValuePredicate(string? value = null, bool isVisible = true, StringComparison? valueComparison = null, ValueTypeName? valueTypeName = null)
        {
            this.ValueTypeDescription = new ValueTypeDescription(valueTypeName, value != null && isVisible ? [value] : null);

            this.value = value;
            this.valueComparison = valueComparison;
        }

        public virtual bool Match(IParameterToken token, StringComparison defaultComparison)
        {
            return token.Value.Equals(this.value, this.valueComparison ?? defaultComparison);
        }

        public static implicit operator ValuePredicate(string value)
        {
            return new ValuePredicate(value);
        }
    }

    public class ValueTypeName
    {
        public static readonly ValueTypeName Value = new("value");

        public string Name { get; }
        public string NamePlural { get; }

        public ValueTypeName(string name) :
            this(name, name + 's')
        {
        }

        public ValueTypeName(string name, string namePlural)
        {
            this.Name = name;
            this.NamePlural = namePlural;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public static implicit operator ValueTypeName(string name)
        {
            return new ValueTypeName(name);
        }
    }

    public class ValueTypeDescription
    {
        public ValueTypeName? ValueTypeName { get; }
        public IEnumerable<string>? AvailableValues { get; }

        public ValueTypeDescription(ValueTypeName? valueTypeName = null, IEnumerable<string>? availableValues = null)
        {
            this.ValueTypeName = valueTypeName;
            this.AvailableValues = availableValues;
        }

        public static implicit operator ValueTypeDescription(string name)
        {
            return new ValueTypeDescription(new ValueTypeName(name));
        }

        public static implicit operator ValueTypeDescription(ValueTypeName valueTypeName)
        {
            return new ValueTypeDescription(valueTypeName);
        }
    }
}
