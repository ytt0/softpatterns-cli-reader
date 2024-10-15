namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        public void TrySubstituteValue_WithMatchingDescription_ValueIsSubstituted()
        {
            // setup

            const string oldValue = "old-value";
            const string newValue = "new-value";
            var oldValuePredicate = new ValuePredicate(oldValue, valueComparison: StringComparison.InvariantCultureIgnoreCase); // ignore case for this value

            var reader = new ParametersReader(["OLD-VALUE"], defaultValueComparison: StringComparison.InvariantCulture); // upper case value without ignoring case by default

            // test

            Assert.IsTrue(reader.TrySubstituteValue(oldValuePredicate, newValue)); // old name was substituted
            Assert.IsFalse(reader.TryPeekValue(oldValuePredicate, out _)); // old value cannot be peeked
            Assert.IsTrue(reader.TryPeekValue(newValue, out _)); // new value can be peeked
        }

        [TestMethod]
        public void TrySubstituteValue_WithNonMatchingDescription_ValueIsNotSubstituted()
        {
            // setup

            const string oldValue = "old-value";
            const string newValue = "new-value";
            var oldValuePredicate = new ValuePredicate(oldValue, valueComparison: StringComparison.InvariantCulture); // do not ignore case for this value

            var reader = new ParametersReader(["OLD-VALUE"], defaultValueComparison: StringComparison.InvariantCultureIgnoreCase); // upper case value and ignore case by default

            // test

            Assert.IsFalse(reader.TrySubstituteValue(oldValuePredicate, newValue)); // old value was not substituted
            Assert.IsTrue(reader.TryPeekValue(oldValue, out _)); // old value can still be peeked
        }
    }
}
