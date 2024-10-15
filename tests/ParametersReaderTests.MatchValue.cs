namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void MatchValue_WithValue_ReturnsValueTokenOnce(bool isNamedValue)
        {
            // setup

            const string value = "value";
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, value));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var valueToken = reader.MatchValue();
            Assert.AreEqual(value, valueToken.Value); // value was matched

            Assert.IsFalse(reader.TryMatchValue(null, out _)); // value was not matched again
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void MatchValue_WithoutValue_ThrowsMissingValueException(bool isNamedValue)
        {
            // setup

            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty value

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.MatchValue()); // value was not matched
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void MatchValue_WithFollowingMatchedParameterValue_ThrowsMissingValueException(bool isNamedValue)
        {
            // setup

            var reader = new ParametersReader(CreateValueArgs(isNamedValue, "--following-parameter", "following-value")); // empty value

            reader.MatchParameter("following-parameter");
            reader.MatchValue();
            reader.MatchParameterEnd();

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.MatchValue()); // value was not matched
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void MatchValue_WithPrecedingParameterName_ThrowsMissingValueExceptionWithNextToken(bool isNamedValue)
        {
            // setup

            const string parameterValue = "value";
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, "--preceding-parameter", parameterValue));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.MatchValue()); // value is missing
            Assert.AreEqual("preceding-parameter", e.NextToken?.Value); // previous token was set
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void MatchValue_WithPredicateAndWithoutArgsValue_ThrowsMissingValueExceptionDescription(bool isNamedValue)
        {
            // setup

            var valueTypeName = new ValueTypeName("value-type");
            var value = new ValuePredicate("value", valueTypeName: valueTypeName);
            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty value

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.MatchValue(value)); // value is missing
            Assert_AreEqual(value.ValueTypeDescription, e.ValueTypeDescription);
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void MatchValue_WithValueTypeDescriptionAndWithoutArgsValue_ThrowsMissingValueExceptionDescription(bool isNamedValue)
        {
            // setup

            var valueTypeDescription = new ValueTypeDescription(new ValueTypeName("value-type"));
            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty value

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.MatchValue(null, valueTypeDescription)); // value is missing
            Assert_AreEqual(valueTypeDescription, e.ValueTypeDescription);
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void MatchValue_WithNonMatchingPredicate_ThrowsInvalidValueExceptionDescription(bool isNamedValue)
        {
            // setup

            const string nonMatchingValue = "non-matching-value";
            var valueTypeName = new ValueTypeName("value-type");
            var value = new ValuePredicate("value", valueTypeName: valueTypeName);
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, nonMatchingValue)); // non matching value

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<InvalidValueException>(() => reader.MatchValue(value)); // value was not matched
            Assert_AreEqual(value.ValueTypeDescription, e.ValueTypeDescription);
        }

        [TestMethod]
        public void MatchValue_WithPrecedingMatchedParameter_ReturnsValueToken()
        {
            // setup

            const string value = "value";
            var reader = new ParametersReader(["--preceding-parameter", value]);

            reader.MatchParameter("preceding-parameter"); // match preceding parameter
            reader.MatchParameterEnd();

            // test

            var valueToken = reader.MatchValue(); // match value
            Assert.AreEqual(value, valueToken.Value); // value was matched
        }

        [TestMethod]
        public void MatchParameterValue_WithPrecedingMatchedParameter_ThrowsMissingParameterValueException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            const string value = "value";
            var reader = new ParametersReader([nameArg, "--preceding-parameter", value]);

            reader.MatchParameter("preceding-parameter"); // match preceding parameter
            reader.MatchParameterEnd();

            reader.MatchParameter(name); // match parameter

            // test

            var e = Assert.ThrowsException<MissingParameterValueException>(() => reader.MatchValue()); // parameter value cannot be matched
            Assert.AreEqual(name, e.Name);
            Assert.AreEqual(name, e.NameToken.Value);
        }
    }
}
