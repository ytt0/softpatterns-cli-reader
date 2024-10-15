namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ReadValue_WithParser_ReturnsParsedValueOnce(bool isNamedValue)
        {
            // setup

            const string expectedValue = "value";
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, expectedValue));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var actualValue = reader.ReadValue(TestParser);
            Assert.AreEqual(expectedValue, actualValue); // value was matched

            Assert.IsFalse(reader.TryMatchValue(null, out _)); // value was not matched again
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ReadValue_WithFailingParser_ThrowsInvalidValueException(bool isNamedValue)
        {
            // setup

            const string nonMatchingValue = "non-matching-value";
            var valueTypeName = new ValueTypeDescription("type", ["value1", "value2"]);
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, nonMatchingValue));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<InvalidValueException>(() => reader.ReadValue(TestFailingParser, valueTypeName)); // parsing failed
            Assert.AreEqual(nonMatchingValue, e.ValueToken.Value);
            Assert_AreEqual(valueTypeName, e.ValueTypeDescription);
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ReadValue_WithDefaultValueAndWithoutArgsValue_ReturnsDefaultValueOnce(bool isNamedValue)
        {
            // setup

            const string defaultValue = "default-value";
            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty value

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var actualValue = reader.ReadValue(TestParser, defaultValue);
            Assert.AreEqual(defaultValue, actualValue); // default value was returned

            Assert.IsFalse(reader.TryMatchValue(null, out _)); // was not matched again
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ReadValue_WithoutDefaultValueAndWithoutArgsValue_ThrowsMissingValueException(bool isNamedValue)
        {
            // setup

            var valueTypeName = new ValueTypeDescription("value-type", ["value1", "value2"]);
            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty value

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.ReadValue(TestParser, valueTypeName)); // value is missing
            Assert_AreEqual(valueTypeName, e.ValueTypeDescription);
        }
    }
}
