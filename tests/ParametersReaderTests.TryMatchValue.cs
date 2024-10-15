namespace SoftPatterns.Cli.Reader.Tests
{
    public partial class ParametersReaderTests
    {
        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryMatchValue_WithValue_ReturnsValueTokenOnce(bool isNamedValue)
        {
            // setup

            const string value = "value";
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, value));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            Assert.IsTrue(reader.TryMatchValue(null, out var valueToken1)); // match value
            Assert.AreEqual(value, valueToken1.Value); // value was matched

            Assert.IsFalse(reader.TryMatchValue(null, out _)); // value was not matched again
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryMatchValue_WithoutValue_DoesNotReturnValueToken(bool isNamedValue)
        {
            // setup

            var reader = new ParametersReader(CreateValueArgs(isNamedValue)); // empty value

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            Assert.IsFalse(reader.TryMatchValue(null, out _)); // value does not exist
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryMatchValue_WithFollowingMatchedParameterValue_DoesNotReturnValueToken(bool isNamedValue)
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

            Assert.IsFalse(reader.TryMatchValue(null, out _)); // value does not exist
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryMatchValue_WithPrecedingParameterName_DoesNotReturnValueToken(bool isNamedValue)
        {
            // setup

            const string parameterValue = "value";
            var reader = new ParametersReader(CreateValueArgs(isNamedValue, "--preceding-parameter", parameterValue));

            if (isNamedValue)
            {
                reader.MatchParameter("parameter");
            }

            // test

            Assert.IsFalse(reader.TryMatchValue(null, out _)); // parameter value was not matched
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryMatchValue_WithMatchingDescription_ReturnsValueTokenMoreThanOnce(bool isNamedValue)
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

            Assert.IsTrue(reader.TryMatchValue(predicate, out var valueToken1)); // match value
            Assert.AreEqual(value, valueToken1.Value); // value was matched

            Assert.IsFalse(reader.TryMatchValue(predicate, out _)); // value was not matched again
        }

        [TestMethod]
        [DataRow(false, DisplayName = "Unnamed Value")]
        [DataRow(true, DisplayName = "Named Value")]
        public void TryMatchValue_WithNonMatchingDescription_DoesNotReturnValueToken(bool isNamedValue)
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

            Assert.IsFalse(reader.TryMatchValue(predicate, out _)); // value with non matching predicate was not matched
        }

        [TestMethod]
        public void TryMatchValue_WithPrecedingMatchedParameter_ReturnsValueToken()
        {
            // setup

            const string value = "value";
            var reader = new ParametersReader(["--preceding-parameter", value]);

            reader.MatchParameter("preceding-parameter"); // match preceding parameter
            reader.MatchParameterEnd();

            // test

            Assert.IsTrue(reader.TryMatchValue(null, out var valueToken)); // value was matched
            Assert.AreEqual(value, valueToken.Value);
        }

        [TestMethod]
        public void TryMatchParameterValue_WithPrecedingMatchedParameter_DoesNotReturnValueToken()
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

            Assert.IsFalse(reader.TryMatchValue(null, out _)); // parameter value was not matched
        }
    }
}