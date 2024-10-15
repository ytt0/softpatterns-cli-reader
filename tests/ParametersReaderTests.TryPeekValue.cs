namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryPeekValue_WithValue_ReturnsValueTokenMoreThanOnce(bool isNamedValue)
        {
            // setup

            const string value = "value";
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, value));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            Assert.IsTrue(reader.TryPeekValue(null, out var valueToken1)); // peek value
            Assert.AreEqual(value, valueToken1.Value); // value was peeked

            Assert.IsTrue(reader.TryPeekValue(null, out var valueToken2)); // peek again
            Assert.AreEqual(valueToken1, valueToken2); // same token was returned
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryPeekValue_WithoutValue_DoesNotReturnValueToken(bool isNamedValue)
        {
            // setup

            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty value

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            Assert.IsFalse(reader.TryPeekValue(null, out _)); // value does not exist
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryPeekValue_WithFollowingMatchedParameterValue_DoesNotReturnValueToken(bool isNamedValue)
        {
            // setup

            var reader = new ParametersReader(CreateValueArgs(isNamedValue, "--following-parameter", "following-value")); // empty value

            reader.MatchParameter("following-parameter");
            reader.MatchValue();
            reader.MatchParameterEnd();

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            Assert.IsFalse(reader.TryPeekValue(null, out _)); // value does not exist
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryPeekValue_WithPrecedingParameterName_DoesNotReturnValueToken(bool isNamedValue)
        {
            // setup

            const string parameterValue = "value";
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, "--preceding-parameter", parameterValue));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            Assert.IsFalse(reader.TryPeekValue(null, out _)); // parameter value was not peeked
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryPeekValue_WithMatchingDescription_ReturnsValueTokenMoreThanOnce(bool isNamedValue)
        {
            // setup

            const string value = "value";
            var predicate = new ValuePredicate(value); // matching predicate
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, value));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            Assert.IsTrue(reader.TryPeekValue(predicate, out var valueToken1)); // peek value
            Assert.AreEqual(value, valueToken1.Value); // value was peeked

            Assert.IsTrue(reader.TryPeekValue(predicate, out var valueToken2)); // peek again
            Assert.AreEqual(valueToken1, valueToken2); // same token was returned
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryPeekValue_WithNonMatchingDescription_DoesNotReturnValueToken(bool isNamedValue)
        {
            // setup

            const string value = "value";
            var predicate = new ValuePredicate("different-value"); // non matching predicate
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, value));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            Assert.IsFalse(reader.TryPeekValue(predicate, out _)); // non matching value was not peaked
        }

        [TestMethod]
        public void TryPeekValue_WithPrecedingMatchedParameter_ReturnsValueToken()
        {
            // setup

            const string value = "value";
            var reader = new ParametersReader(["--preceding-parameter", value]);

            reader.MatchParameter("preceding-parameter"); // match preceding parameter
            reader.MatchParameterEnd();

            // test

            Assert.IsTrue(reader.TryPeekValue(null, out var valueToken)); // value was peeked
            Assert.AreEqual(value, valueToken.Value);
        }

        [TestMethod]
        public void TryPeekParameterValue_WithPrecedingMatchedParameter_DoesNotReturnValueToken()
        {
            // setup

            const string name = "parameter";
            const string nameArg = "--parameter";
            const string value = "value";
            var reader = new ParametersReader([nameArg, "--preceding-parameter", value]);

            reader.MatchParameter("preceding-parameter"); // match preceding parameter
            reader.MatchParameterEnd();

            reader.MatchParameter(name); // match parameter

            // test

            Assert.IsFalse(reader.TryPeekValue(null, out _)); // parameter value was not peeked
        }
    }
}