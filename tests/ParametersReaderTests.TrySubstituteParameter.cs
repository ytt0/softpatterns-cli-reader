namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        public void TrySubstituteParameter_WithMatchingDescription_ParameterNameIsSubstituted()
        {
            // setup

            const string oldName = "old-parameter";
            const string newName = "new-parameter";
            var oldNamePredicate = new NamePredicate(oldName, nameComparison: StringComparison.InvariantCultureIgnoreCase); // ignore case for this parameter

            var reader = new ParametersReader(["--OLD-PARAMETER"], defaultNameComparison: StringComparison.InvariantCulture); // upper case parameter without ignoring case by default


            // test

            Assert.IsTrue(reader.TrySubstituteParameter(oldNamePredicate, newName)); // old name was substituted
            Assert.IsFalse(reader.TryPeekParameter(oldNamePredicate, out _)); // old name cannot be peeked
            Assert.IsTrue(reader.TryPeekParameter(newName, out _)); // new name can be peeked
        }

        [TestMethod]
        public void TrySubstituteParameter_WithNonMatchingDescription_ParameterNameIsNotSubstituted()
        {
            // setup

            const string oldName = "old-parameter";
            const string newName = "new-parameter";
            var oldNamePredicate = new NamePredicate(oldName, nameComparison: StringComparison.InvariantCulture); // do not ignore case for this parameter

            var reader = new ParametersReader(["--OLD-PARAMETER"], defaultNameComparison: StringComparison.InvariantCultureIgnoreCase); // upper case parameter without ignoring case by default

            // test

            Assert.IsFalse(reader.TrySubstituteParameter(oldNamePredicate, newName)); // old name was not substituted
            Assert.IsTrue(reader.TryPeekParameter(oldName, out _)); // old name can still be peeked
        }
    }
}
