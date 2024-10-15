namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ThrowUnmatchedValueException_WithoutValue_ThrowsMissingValueExceptionWithValueTypeName(bool isNamedValue)
        {
            // setup

            var valueTypeName = new ValueTypeName("named value", "named values");

            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty values

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.ThrowUnmatchedValueException(valueTypeName));
            Assert_AreEqual(valueTypeName, e.ValueTypeDescription?.ValueTypeName);
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ThrowUnmatchedValueException_WithoutValuePrecededByValueMismatch_ThrowsMissingValueExceptionWithAvailableValues(bool isNamedValue)
        {
            // setup

            const string mismatchedValue = "mismatched-value";

            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty values

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            Assert.IsFalse(reader.TryMatchValue(mismatchedValue, out _)); // mismatched value added to available values

            // test

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.ThrowUnmatchedValueException());
            CollectionAssert.AreEquivalent(new[] { mismatchedValue }, e.ValueTypeDescription?.AvailableValues?.ToArray());
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ThrowUnmatchedValueException_WithoutValuePrecededByNonVisibleValueMismatch_ThrowsMissingValueExceptionWithoutAvailableValues(bool isNamedValue)
        {
            // setup

            const string mismatchedValue = "mismatched-value";

            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty values

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            Assert.IsFalse(reader.TryMatchValue(new ValuePredicate(mismatchedValue, isVisible: false), out _)); // mismatched value is not added to available values

            // test

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.ThrowUnmatchedValueException());
            Assert.AreEqual(0, e.ValueTypeDescription?.AvailableValues?.Count());
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ThrowUnmatchedValueException_WithoutValueAndWithPrecedingMatchedValue_ThrowsMissingValueExceptionWithPreviousValueToken(bool isNamedValue)
        {
            // setup

            const string precedingValue = "preceding-value";

            var reader = new ParametersReader(CreateValueArgs(isNamedValue, precedingValue));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            reader.MatchValue(precedingValue); // match preceding value

            // test

            var e = Assert_ThrowsExceptionBase<MissingValueException>(() => reader.ThrowUnmatchedValueException());
            Assert.AreEqual(precedingValue, e.PreviousValueToken?.Value);
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ThrowUnmatchedValueException_WithValue_ThrowsInvalidValueExceptionWithValueToken(bool isNamedValue)
        {
            // setup

            const string value = "value";

            var reader = new ParametersReader(CreateValueArgs(isNamedValue, value));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<InvalidValueException>(() => reader.ThrowUnmatchedValueException());
            Assert.AreEqual(value, e.ValueToken?.Value);
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ThrowUnmatchedValueException_WithValue_ThrowsInvalidValueExceptionWithValueTypeName(bool isNamedValue)
        {
            // setup

            const string value = "value";
            var valueTypeName = new ValueTypeName("named value", "named values");

            var reader = new ParametersReader(CreateValueArgs(isNamedValue, value));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            var e = Assert_ThrowsExceptionBase<InvalidValueException>(() => reader.ThrowUnmatchedValueException(valueTypeName));
            Assert_AreEqual(valueTypeName, e.ValueTypeDescription?.ValueTypeName);
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ThrowUnmatchedValueException_WithValuePrecededByValueMismatch_ThrowsInvalidValueExceptionWithAvailableValues(bool isNamedValue)
        {
            // setup

            const string value = "value";
            const string mismatchedValue = "mismatched-value";

            var reader = new ParametersReader(CreateValueArgs(isNamedValue, value));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            Assert.IsFalse(reader.TryMatchValue(mismatchedValue, out _)); // mismatched value added to available values

            // test

            var e = Assert_ThrowsExceptionBase<InvalidValueException>(() => reader.ThrowUnmatchedValueException());
            CollectionAssert.AreEquivalent(new[] { mismatchedValue }, e.ValueTypeDescription?.AvailableValues?.ToArray());
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void ThrowUnmatchedValueException_WithValuePrecededByNonVisibleValueMismatch_ThrowsInvalidValueExceptionWithoutAvailableValues(bool isNamedValue)
        {
            // setup

            const string value = "value";
            const string mismatchedValue = "mismatched-value";

            var reader = new ParametersReader(CreateValueArgs(isNamedValue, value));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            Assert.IsFalse(reader.TryMatchValue(new ValuePredicate(mismatchedValue, isVisible: false), out _)); // mismatched value is not added to available values

            // test

            var e = Assert_ThrowsExceptionBase<InvalidValueException>(() => reader.ThrowUnmatchedValueException());
            Assert.AreEqual(0, e.ValueTypeDescription?.AvailableValues?.Count());
        }
    }
}
