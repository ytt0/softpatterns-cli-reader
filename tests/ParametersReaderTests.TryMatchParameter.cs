namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        public void TryMatchParameter_WithParameterName_ReturnsParameterTokenOnce()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            var reader = new ParametersReader([nameArg]); // args with parameter name

            // test

            Assert.IsTrue(reader.TryMatchParameter(name, out var nameToken)); // match name
            Assert.AreEqual(name, nameToken.Value); // name was matched

            reader.MatchParameterEnd();

            Assert.IsFalse(reader.TryPeekParameter(name, out _)); // name was not matched again
        }

        [TestMethod]
        public void TryMatchParameter_WithoutParameterName_DoesNotReturnParameterToken()
        {
            // setup

            const string name = "parameter";
            var reader = new ParametersReader(["value"]); // args without parameter name

            // test

            Assert.IsFalse(reader.TryMatchParameter(name, out _)); // name was not found
        }

        [TestMethod]
        public void TryMatchParameter_WithPrecedingValue_ReturnsParameterToken()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            var reader = new ParametersReader(["value", nameArg]); // args with parameter name and preceding value

            // test

            Assert.IsTrue(reader.TryMatchParameter(name, out var nameToken1)); // match name
            Assert.AreEqual(name, nameToken1.Value); // name was matched
        }

        [TestMethod]
        public void TryMatchParameter_WithMatchingDescription_ReturnsParameterToken()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--PARAMETER"; // arg name with non matching case

            var reader = new ParametersReader([nameArg], defaultNameComparison: StringComparison.InvariantCulture); // upper case parameter without ignoring case by default
            var namePredicate = new NamePredicate(name, nameComparison: StringComparison.InvariantCultureIgnoreCase); // ignore case for this parameter

            // test

            Assert.IsTrue(reader.TryMatchParameter(namePredicate, out var nameToken1)); // case was ignored
            Assert.AreEqual(nameArg, nameToken1.SubstitutedToken?.Value); // name was matched
        }

        [TestMethod]
        public void TryMatchParameter_WithNonMatchingDescription_DoesNotReturnParameterToken()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--PARAMETER"; // arg name with non matching case

            var reader = new ParametersReader([nameArg], defaultNameComparison: StringComparison.InvariantCultureIgnoreCase); // upper case parameter with ignoring case by default
            var namePredicate = new NamePredicate(name, nameComparison: StringComparison.InvariantCulture); // do not ignore case for this parameter

            // test

            Assert.IsFalse(reader.TryMatchParameter(namePredicate, out _)); // case was not ignored
        }

        [TestMethod]
        public void TryMatchParameter_WithCurrentlyMatchedParameter_ThrowsInvalidOperationException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            var reader = new ParametersReader([nameArg, "--currently-matched-parameter"]); // args with parameter name

            reader.MatchParameter("currently-matched-parameter"); // match parameter

            // test

            Assert.ThrowsException<InvalidOperationException>(() => reader.TryMatchParameter(name, out _)); // match without calling current match parameter end
        }
    }
}
