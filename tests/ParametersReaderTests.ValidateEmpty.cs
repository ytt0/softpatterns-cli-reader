namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        public void ValidateEmpty_WithParameterMatchingStarted_ThrowsInvalidOperationException()
        {
            // setup

            var reader = new ParametersReader(["--parameter"]);
            reader.MatchParameter("parameter"); // start parameter match

            // test

            Assert.ThrowsException<InvalidOperationException>(reader.ValidateEmpty); // parameter is currently being matched
        }

        [TestMethod]
        public void ValidateEmpty_WithRepeatingParameterNameFollowedByValue_ThrowsUnexpectedRepeatingParameterException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";

            var reader = new ParametersReader([nameArg, nameArg, "value"]); // repeating parameter with value
            reader.MatchParameter(name); // match first occurrence
            reader.MatchParameterEnd();

            // test

            var e = Assert.ThrowsException<UnexpectedRepeatingParameterException>(reader.ValidateEmpty); // parameter followed by a value has not been matched yet
            Assert.AreEqual(name, e.Name);
            Assert.AreEqual(name, e.NameToken.Value);
        }

        [TestMethod]
        public void ValidateEmpty_WithRepeatingParameterName_ThrowsUnexpectedRepeatingSwitchException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";

            var reader = new ParametersReader([nameArg, nameArg]); // repeating parameter without value
            reader.MatchParameter(name); // match first occurrence
            reader.MatchParameterEnd();

            // test

            var e = Assert.ThrowsException<UnexpectedRepeatingSwitchException>(reader.ValidateEmpty); // second parameter is considered as a switch
            Assert.AreEqual(name, e.Name);
            Assert.AreEqual(name, e.NameToken.Value);
        }

        [TestMethod]
        public void ValidateEmpty_WithRepeatingParameterNameFollowedByParameterName_ThrowsUnexpectedRepeatingSwitchException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";

            var reader = new ParametersReader([nameArg, nameArg, "--following-parameter"]); // repeating parameter followed by another parameter
            reader.MatchParameter(name); // match first occurrence
            reader.MatchParameterEnd();

            // test

            var e = Assert.ThrowsException<UnexpectedRepeatingSwitchException>(reader.ValidateEmpty); // second parameter is considered as a switch
            Assert.AreEqual(name, e.Name);
            Assert.AreEqual(name, e.NameToken.Value);
        }

        [TestMethod]
        public void ValidateEmpty_WithUnmatchedParameterNameFollowedByValue_ThrowsUnexpectedParameterException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";

            var reader = new ParametersReader([nameArg, "value"]); // parameter with value

            // test

            var e = Assert.ThrowsException<UnexpectedParameterException>(reader.ValidateEmpty); // parameter has not been matched yet
            Assert.AreEqual(name, e.NameToken.Value);
        }

        [TestMethod]
        public void ValidateEmpty_WithUnmatchedParameterAndMatchedPrecedingParameter_ThrowsUnexpectedParameterExceptionWithPrecedingParameterName()
        {
            // setup

            const string precedingName = "preceding-parameter";
            const string precedingNameArg = "--preceding-parameter";
            const string name = "parameter";
            const string nameArg = "--parameter";

            var precedingNamePredicate = new NamePredicate(precedingName);

            var reader = new ParametersReader([precedingNameArg, nameArg, "value"]); // parameter with value
            reader.MatchParameter(precedingNamePredicate);
            reader.MatchParameterEnd();

            // test

            var e = Assert.ThrowsException<UnexpectedParameterException>(reader.ValidateEmpty); // parameter has not been matched yet
            Assert.AreEqual(name, e.NameToken.Value);
            CollectionAssert.AreEquivalent(new[] { precedingNamePredicate }, e.MatchedNames.ToArray()); // preceding matched parameter name is included
        }

        [TestMethod]
        public void ValidateEmpty_WithUnmatchedParameterName_ThrowsUnexpectedSwitchException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";

            var reader = new ParametersReader([nameArg]); // parameter without a value

            // test

            var e = Assert.ThrowsException<UnexpectedSwitchException>(reader.ValidateEmpty); // parameter is considered as a switch
            Assert.AreEqual(name, e.NameToken.Value);
        }

        [TestMethod]
        public void ValidateEmpty_WithUnmatchedParameterNameFollowedByParameterName_ThrowsUnexpectedSwitchException()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";

            var reader = new ParametersReader([nameArg, "--following-parameter"]); // parameter without a value

            // test

            var e = Assert.ThrowsException<UnexpectedSwitchException>(reader.ValidateEmpty); // parameter is considered as a switch
            Assert.AreEqual(name, e.NameToken.Value);
        }

        [TestMethod]
        public void ValidateEmpty_WithUnmatchedValue_ThrowsUnexpectedValueException()
        {
            // setup

            const string value = "value";

            var reader = new ParametersReader([value]);

            // test

            var e = Assert.ThrowsException<UnexpectedValueException>(reader.ValidateEmpty); // value has not been matched yet
            Assert.AreEqual(value, e.ValueToken.Value);
        }

        [TestMethod]
        public void ValidateEmpty_WithoutTokens_CompletesSuccessfully()
        {
            // setup

            var reader = new ParametersReader([]);

            // test

            reader.ValidateEmpty(); // no unmatched tokens
        }

        [TestMethod]
        public void ValidateEmpty_WithMatchedTokens_CompletesSuccessfully()
        {
            // setup

            const string value = "value";
            const string name = "parameter";
            const string nameArg = "--parameter";

            var reader = new ParametersReader([value, nameArg]);

            reader.MatchParameter(name); // match parameter
            reader.MatchParameterEnd();
            reader.MatchValue(); // match value

            // test

            reader.ValidateEmpty(); // all tokens have been matched
        }
    }
}
