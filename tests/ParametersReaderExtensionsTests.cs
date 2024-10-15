namespace SoftPatterns.Cli.Reader.Tests
{
    [TestClass]
    public class ParametersReaderExtensionsTests : ParametersReaderTests
    {
        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ReadValue_WithMatchingDescription_ReturnsStringValueOnce(bool isNamedValue)
        {
            // setup

            const string expectedValue = "value";
            var valueTypeName = new ValueTypeName("value-type");
            var value = new ValuePredicate(expectedValue, valueTypeName: valueTypeName);
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, expectedValue));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var actualValue = reader.ReadValue(value);
            Assert.AreEqual(expectedValue, actualValue); // string value was matched

            Assert.IsFalse(reader.TryMatchValue(out _)); // string value was not matched again
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ReadValue_WithMatchingPredicate_ReturnsStringValueOnce(bool isNamedValue)
        {
            // setup

            const string expectedValue = "VALUE";
            var valueTypeName = new ValueTypeName("value-type");
            var value = new ValuePredicate(expectedValue, true, StringComparison.InvariantCultureIgnoreCase, valueTypeName); // ignore case
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, expectedValue), defaultValueComparison: StringComparison.InvariantCulture); // do not ignore case by default

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var actualValue = reader.ReadValue(value); // ignore case
            Assert.AreEqual(expectedValue, actualValue); // string value was matched

            Assert.IsFalse(reader.TryMatchValue(out _)); // string value was not matched again
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ReadValue_WithNonMatchingDescription_ThrowsInvalidValueException(bool isNamedValue)
        {
            // setup

            const string nonMatchingValue = "non-matching-value";
            var valueTypeName = new ValueTypeName("value-type");
            var value = new ValuePredicate("value", valueTypeName: valueTypeName); // ignore case
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, nonMatchingValue));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<InvalidValueException>(() => reader.ReadValue(value)); // string value was not matched
            Assert.AreEqual(nonMatchingValue, e.ValueToken.Value);
            Assert_AreEqual(valueTypeName, e.ValueTypeDescription?.ValueTypeName);
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ReadValue_WithMatchingDescriptionAndNonMatchingComparison_ThrowsInvalidValueException(bool isNamedValue)
        {
            // setup

            const string expectedValue = "VALUE";
            var valueTypeName = new ValueTypeName("value-type");
            var value = new ValuePredicate("value", true, StringComparison.InvariantCulture, valueTypeName); // case was not ignored
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, expectedValue), defaultValueComparison: StringComparison.InvariantCultureIgnoreCase); // ignore case by default

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<InvalidValueException>(() => reader.ReadValue(value));
            Assert.AreEqual(expectedValue, e.ValueToken.Value);
            Assert_AreEqual(valueTypeName, e.ValueTypeDescription?.ValueTypeName);
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ReadValue_WithStringDefaultValueAndWithoutArgsValue_ReturnsDefaultValueOnce(bool isNamedValue)
        {
            // setup

            const string defaultValue = "default-value";
            var valueTypeName = new ValueTypeName("value-type");
            var value = new ValuePredicate("value", valueTypeName: valueTypeName);
            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty value

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var actualValue = reader.ReadValue(defaultValue, value);
            Assert.AreEqual(defaultValue, actualValue); // default value was returned

            Assert.IsFalse(reader.TryMatchValue(out _)); // value was not matched again
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ReadValue_WithoutStringDefaultValueAndWithoutArgsValue_ThrowsMissingValueException(bool isNamedValue)
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

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.ReadValue(value)); // value is missing
            Assert_AreEqual(valueTypeName, e.ValueTypeDescription?.ValueTypeName);
        }
    }
}
