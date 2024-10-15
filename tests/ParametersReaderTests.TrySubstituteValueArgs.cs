namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        public void TrySubstituteValueArgs_WithMatchingDescription_ValueIsSubstitutedWithParsedArgs()
        {
            // setup

            const string oldValue = "old-value";
            const string newValue = "new-value";
            const string newParameter = "new-parameter";
            const string newParameterArg = "--new-parameter";
            const string newParameterValue = "new-parameter-value";

            var newValueArgs = new[] { newValue, newParameterArg, newParameterValue };
            var oldValuePredicate = new ValuePredicate(oldValue, valueComparison: StringComparison.InvariantCultureIgnoreCase); // ignore case for this value

            var reader = new ParametersReader(["OLD-VALUE"], defaultValueComparison: StringComparison.InvariantCulture); // upper case value without ignoring case by default

            // test

            Assert.IsTrue(reader.TrySubstituteValueArgs(oldValuePredicate, newValueArgs)); // old name was substituted
            Assert.IsFalse(reader.TryPeekValue(oldValuePredicate, out _)); // old value cannot be peeked

            Assert.IsTrue(reader.TryMatchParameter(newParameter, out _)); // new parameter was matched
            Assert.IsTrue(reader.TryMatchValue(newParameterValue, out _)); // new parameter value was matched
            reader.MatchParameterEnd();

            Assert.IsTrue(reader.TryMatchValue(newValue, out _)); // new value was matched
        }

        [TestMethod]
        public void TrySubstituteValueArgs_WithNonMatchingDescription_ValueIsNotSubstituted()
        {
            // setup

            const string oldValue = "old-value";
            var newValues = new[] { "new-value" };
            var oldValuePredicate = new ValuePredicate(oldValue, valueComparison: StringComparison.InvariantCulture); // do not ignore case for this value

            var reader = new ParametersReader(["OLD-VALUE"], defaultValueComparison: StringComparison.InvariantCultureIgnoreCase); // upper case value and ignore case by default

            // test

            Assert.IsFalse(reader.TrySubstituteValueArgs(oldValuePredicate, newValues)); // old value was not substituted
            Assert.IsTrue(reader.TryPeekValue(oldValue, out _)); // old value can still be peeked
        }
    }
}
