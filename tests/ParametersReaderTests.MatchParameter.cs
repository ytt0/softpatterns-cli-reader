namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        public void MatchParameter_WithParameterName_ReturnsParameterTokenOnce()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            var reader = new ParametersReader([nameArg]); // args with parameter name

            // test

            var nameToken = reader.MatchParameter(name); // match name
            Assert.AreEqual(name, nameToken.Value); // name was matched

            reader.MatchParameterEnd();

            Assert.IsFalse(reader.TryPeekParameter(name, out _)); // name was not matched again
        }

        [TestMethod]
        public void MatchParameter_WithParameterShortName_ReturnsParameterTokenOnce()
        {
            // setup

            const string name = "parameter";
            const string shortName = "p";
            const string shortNameArg = "-p";
            var reader = new ParametersReader([shortNameArg]); // args with parameter short name

            // test

            var nameToken = reader.MatchParameter(new NamePredicate(name, shortName)); // match short name
            Assert.AreEqual(shortName, nameToken.Value); // short name was matched

            reader.MatchParameterEnd();

            Assert.IsFalse(reader.TryPeekParameter(new NamePredicate(name, shortName), out _)); // parameter was not matched again
        }

        [TestMethod]
        public void MatchParameter_WithoutParameterName_ThrowsMissingParameterException()
        {
            // setup

            const string name = "parameter";
            var reader = new ParametersReader(["value"]); // args without parameter name

            // test

            var e = Assert.ThrowsException<MissingParameterException>(() => reader.MatchParameter(name)); // name is missing
            Assert.AreEqual(name, e.Name);
        }

        [TestMethod]
        public void MatchParameter_WithCurrentlyMatchedParameter_ThrowsInvalidOperationException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            var reader = new ParametersReader([nameArg, "--currently-matched-parameter"]); // args with parameter name

            reader.MatchParameter("currently-matched-parameter"); // match parameter

            // test

            Assert.ThrowsException<InvalidOperationException>(() => reader.MatchParameter(name)); // match without calling current match parameter end
        }

        [TestMethod]
        public void MatchParameterEnd_WithMatchedParameter_CompletesSuccessfully()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";

            var reader = new ParametersReader([nameArg]); // args with parameter name and value

            reader.MatchParameter(name); // match parameter

            // test

            reader.MatchParameterEnd(); // match parameter end without matching value
        }

        [TestMethod]
        public void MatchParameterEnd_WithFollowingUnmatchedValue_CompletesSuccessfully()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            const string value = "value";

            var reader = new ParametersReader([nameArg, value]); // args with parameter name and value

            reader.MatchParameter(name); // match parameter

            // test

            reader.MatchParameterEnd(); // match parameter end without matching value
            Assert.IsTrue(reader.TryPeekValue(value, out _)); // value was not matched
        }

        [TestMethod]
        public void MatchParameterEnd_WithFollowingUnmatchedAttachedValue_ThrowsUnexpectedSwitchValueException()
        {
            // setup

            const string name = "parameter";
            const string value = "value";

            var reader = new ParametersReader([$"--{name}={value}"]); // args with parameter name and attached value

            reader.MatchParameter(name); // match parameter

            // test

            var e = Assert.ThrowsException<UnexpectedSwitchValueException>(reader.MatchParameterEnd); // match parameter end without matching attached value
            Assert.AreEqual(name, e.Name);
            Assert.AreEqual(name, e.NameToken.Value);
            Assert.AreEqual(value, e.ValueToken.Value);
        }

        [TestMethod]
        public void MatchParameterEnd_WithoutCurrentlyMatchedParameter_ThrowsInvalidOperationException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            var reader = new ParametersReader([nameArg]); // empty args

            reader.MatchParameter(name); // match parameter
            reader.MatchParameterEnd(); // match parameter end

            // test

            Assert.ThrowsException<InvalidOperationException>(reader.MatchParameterEnd); // no parameter is currently being matched
        }
    }
}
