namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        public void TryPeekParameter_WithParameterName_ReturnsParameterTokenMoreThanOnce()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            var reader = new ParametersReader([nameArg]); // args with parameter name

            // test

            Assert.IsTrue(reader.TryPeekParameter(name, out var nameToken1)); // peek name
            Assert.AreEqual(nameArg, nameToken1.SubstitutedToken?.Value); // name was peeked

            Assert.IsTrue(reader.TryPeekParameter(name, out var nameToken2)); // peek again
            Assert.AreEqual(nameToken1, nameToken2); // same token was returned
        }

        [TestMethod]
        public void TryPeekParameter_WithParameterShortName_ReturnsParameterTokenMoreThanOnce()
        {
            // setup

            const string name = "parameter";
            const string shortName = "p";
            const string shortNameArg = "-p";
            var predicate = new NamePredicate(name, shortName);
            var reader = new ParametersReader([shortNameArg]); // args with parameter name

            // test

            Assert.IsTrue(reader.TryPeekParameter(predicate, out var nameToken1)); // peek short name
            Assert.AreEqual(shortName, nameToken1.Value); // short name was peeked

            Assert.IsTrue(reader.TryPeekParameter(predicate, out var nameToken2)); // peek again
            Assert.AreEqual(nameToken1, nameToken2); // same token was returned
        }

        [TestMethod]
        public void TryPeekParameter_WithoutParameterName_DoesNotReturnParameterToken()
        {
            // setup

            const string name = "parameter";
            var reader = new ParametersReader(["value"]); // args without parameter name

            // test

            Assert.IsFalse(reader.TryPeekParameter(name, out _)); // name was not found
        }

        [TestMethod]
        public void TryPeekParameter_WithPrecedingValue_ReturnsParameterToken()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            var reader = new ParametersReader(["value", nameArg]); // args with parameter name and preceding value

            // test

            Assert.IsTrue(reader.TryPeekParameter(name, out var nameToken1)); // peek name
            Assert.AreEqual(name, nameToken1.Value); // name was peeked
        }

        [TestMethod]
        public void TryPeekParameter_WithMatchingDescription_ReturnsParameterToken()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--PARAMETER"; // arg name with non matching case

            var reader = new ParametersReader([nameArg], defaultNameComparison: StringComparison.InvariantCulture); // upper case parameter without ignoring case by default
            var namePredicate = new NamePredicate(name, nameComparison: StringComparison.InvariantCultureIgnoreCase); // ignore case for this parameter

            // test

            Assert.IsTrue(reader.TryPeekParameter(namePredicate, out var nameToken1)); // case was ignored
            Assert.AreEqual(nameArg, nameToken1.SubstitutedToken?.Value); // name was peeked
        }

        [TestMethod]
        public void TryPeekParameter_WithNonMatchingDescription_DoesNotReturnParameterToken()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--PARAMETER"; // arg name with non matching case

            var reader = new ParametersReader([nameArg], defaultNameComparison: StringComparison.InvariantCultureIgnoreCase); // upper case parameter with ignoring case by default
            var namePredicate = new NamePredicate(name, nameComparison: StringComparison.InvariantCulture); // do not ignore case for this parameter

            // test

            Assert.IsFalse(reader.TryPeekParameter(namePredicate, out _)); // case was not ignored
        }

        [TestMethod]
        public void TryPeekParameter_WithCurrentlyMatchedParameter_ThrowsInvalidOperationException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            var reader = new ParametersReader([nameArg, "--currently-matched-parameter"]); // args with parameter name

            reader.MatchParameter("currently-matched-parameter"); // match parameter

            // test

            Assert.ThrowsException<InvalidOperationException>(() => reader.TryPeekParameter(name, out _)); // peek without calling current match parameter end
        }

    }
}
