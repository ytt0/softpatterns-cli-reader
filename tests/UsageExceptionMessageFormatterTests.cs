using System;
using System.Text;

namespace SoftPatterns.Cli.Reader.Tests
{
    [TestClass]
    public class UsageExceptionMessageFormatterTests
    {
        [TestMethod]
        public void FormatMissingValue_WithPreviousToken_PreviousTokenIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var previousValueToken = new ParameterToken(ParameterTokenType.Value, 0, "previous-value", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "previous-value", 0, null));
            var nextToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 1, "--parameter", 0, null));
            var valueTypeName = new ValueTypeName("value-type");
            const string expectedMessage = "value-type is missing after 'previous-value'";

            // test

            var message = formatter.FormatMissingValue(previousValueToken, nextToken, valueTypeName); // previous token takes precedence over next token

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatMissingValue_WithNextToken_NextTokenIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nextToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var valueTypeName = new ValueTypeName("value-type");
            const string expectedMessage = "value-type is missing before '--parameter'";

            // test

            var message = formatter.FormatMissingValue(null, nextToken, valueTypeName);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatMissingValue_WithValueTypeName_TypeNameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var valueTypeName = new ValueTypeName("value-type");
            const string expectedMessage = "value-type is missing";

            // test

            var message = formatter.FormatMissingValue(null, null, valueTypeName);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatMissingValue_WithoutValueTypeName_DefaultTypeNameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            const string expectedMessage = "value is missing";

            // test

            var message = formatter.FormatMissingValue(null, null, null);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatMissingValue_WithAvailableValues_AvailableValuesListIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var description = new ValueTypeDescription("value-type", ["available-value1", "available-value2"]);

            var expectedMessage = JoinLines(
                "value-type is missing",
                "",
                "Available value-types are:",
                "    available-value1",
                "    available-value2");

            // test

            var message = formatter.FormatMissingValue(null, null, description);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatMissingParameter_WithName_NameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            const string expectedMessage = "--parameter parameter is missing";

            // test

            var message = formatter.FormatMissingParameter("parameter");

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatMissingParameterValue_WithPreviousToken_PreviousTokenIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var previousValueToken = new ParameterToken(ParameterTokenType.Value, 0, "previous-value", 0, new ParameterToken(ParameterTokenType.RawArg, 1, "previous-value", 0, null));
            var nextToken = new ParameterToken(ParameterTokenType.Name, 0, "next-parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 2, "--next-parameter", 0, null));
            var valueTypeName = new ValueTypeName("value-type");

            const string expectedMessage = "--parameter parameter additional value-type is missing after 'previous-value'";

            // test

            var message = formatter.FormatMissingParameterValue(nameToken.Value, nameToken, previousValueToken, nextToken, valueTypeName); // previous token takes precedence over next token

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatMissingParameterValue_WithNextToken_NextTokenIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var nextToken = new ParameterToken(ParameterTokenType.Name, 0, "next-parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 1, "--next-parameter", 0, null));
            const string expectedMessage = "--parameter parameter value is missing before '--next-parameter'";

            // test

            var message = formatter.FormatMissingParameterValue(nameToken.Value, nameToken, null, nextToken, null);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatMissingParameterValue_WithValueTypeName_TypeNameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var valueTypeName = new ValueTypeName("value-type");
            const string expectedMessage = "--parameter parameter value-type is missing";

            // test

            var message = formatter.FormatMissingParameterValue(nameToken.Value, nameToken, null, null, valueTypeName);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatMissingParameterValue_WithoutValueTypeName_DefaultTypeNameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            const string expectedMessage = "--parameter parameter value is missing";

            // test

            var message = formatter.FormatMissingParameterValue(nameToken.Value, nameToken, null, null, null);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatMissingParameterValue_WithAvailableValues_AvailableValuesListIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));

            var expectedMessage = JoinLines(
                "--parameter parameter value-type is missing",
                "",
                "Available value-types are:",
                "    available-value1",
                "    available-value2");

            var description = new ValueTypeDescription("value-type", ["available-value1", "available-value2"]);

            // test

            var message = formatter.FormatMissingParameterValue(nameToken.Value, nameToken, null, null, description);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatUnexpectedValue_WithValue_ValueIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "unexpected-value", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "unexpected-value", 0, null));

            const string expectedMessage = "value 'unexpected-value' is unexpected";

            // test

            var message = formatter.FormatUnexpectedValue(valueToken);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatUnexpectedParameter_WithValue_ValueIsIncludedInMessage()
        {
            // setup

            var similarityComparer = TestSimilarityComparer.None; // do not match any value
            var formatter = new UsageExceptionMessageFormatter(similarityComparer: similarityComparer);
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 1, "previous-value", 0, null));
            var availableNames = CreateNamePredicates("available-parameter1", "available-parameter2");

            const string expectedMessage = "Unexpected --parameter parameter 'value'";

            // test

            var message = formatter.FormatUnexpectedParameter(nameToken, valueToken, availableNames);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatUnexpectedParameter_WithMatchingSimilarNames_SimilarNamesListIsIncludedInMessage()
        {
            // setup

            var similarityComparer = TestSimilarityComparer.All; // match all values
            var formatter = new UsageExceptionMessageFormatter(similarityComparer: similarityComparer);
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 1, "previous-value", 0, null));
            var availableNames = CreateNamePredicates("available-parameter1", "available-parameter2");

            var expectedMessage = JoinLines(
                "Unexpected --parameter parameter 'value'",
                "",
                "The most similar parameters are:",
                "    --available-parameter1",
                "    --available-parameter2");

            // test

            var message = formatter.FormatUnexpectedParameter(nameToken, valueToken, availableNames);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatUnexpectedSwitch_WithName_NameIsIncludedInMessage()
        {
            // setup

            var similarityComparer = TestSimilarityComparer.None; // do not match any value
            var formatter = new UsageExceptionMessageFormatter(similarityComparer: similarityComparer);
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "switch", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--switch", 0, null));
            var availableNames = CreateNamePredicates("available-parameter1", "available-parameter2");

            const string expectedMessage = "Unexpected --switch switch";

            // test

            var message = formatter.FormatUnexpectedSwitch(nameToken, availableNames);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatUnexpectedSwitch_WithShortNameGroupAndMatchingName_MatchingNameSuggestionIsIncludedInMessage()
        {
            // setup

            var similarityComparer = TestSimilarityComparer.None; // do not match any value
            var formatter = new UsageExceptionMessageFormatter(similarityComparer: similarityComparer);
            var nameToken = new ParameterToken(ParameterTokenType.ShortName, 0, "s", 1, new ParameterToken(ParameterTokenType.NameGroup, 0, "switch", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "-switch", 0, null)));
            var availableNames = CreateNamePredicates("switch");

            var expectedMessage = JoinLines(
                "Unexpected 's' switch (at '-switch' parameter group)",
                "Did you mean '--switch'?");

            // test

            var message = formatter.FormatUnexpectedSwitch(nameToken, availableNames);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatUnexpectedSwitch_WithMatchingSimilarNames_SimilarNamesListIsIncludedInMessage()
        {
            // setup

            var similarityComparer = TestSimilarityComparer.All; // match all values
            var formatter = new UsageExceptionMessageFormatter(similarityComparer: similarityComparer);
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "switch", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--switch", 0, null));
            var availableNames = CreateNamePredicates("available-parameter1", "available-parameter2");

            var expectedMessage = JoinLines(
                "Unexpected --switch switch",
                "",
                "The most similar parameters are:",
                "    --available-parameter1",
                "    --available-parameter2");

            // test

            var message = formatter.FormatUnexpectedSwitch(nameToken, availableNames);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatUnexpectedSwitchValue_WithValue_ValueIsIncludedInMessage()
        {
            // setup

            var similarityComparer = TestSimilarityComparer.None; // do not match any value
            var formatter = new UsageExceptionMessageFormatter(similarityComparer: similarityComparer);
            var argToken = new ParameterToken(ParameterTokenType.RawArg, 0, "--switch:value", 0, null);
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "switch", 2, argToken);
            var valueToken = new ParameterToken(ParameterTokenType.AttachedValue, 0, "value", 12, argToken);

            const string expectedMessage = "Unexpected --switch switch value 'value'";

            // test

            var message = formatter.FormatUnexpectedSwitchValue(nameToken.Value, nameToken, valueToken);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatUnexpectedRepeatingParameter_WithName_NameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));

            const string expectedMessage = "Unexpected repeating --parameter parameter";

            // test

            var message = formatter.FormatUnexpectedRepeatingParameter(nameToken.Value, nameToken);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatUnexpectedRepeatingSwitch_WithName_NameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "switch", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--switch", 0, null));

            const string expectedMessage = "Unexpected repeating --switch switch";

            // test

            var message = formatter.FormatUnexpectedRepeatingSwitch(nameToken.Value, nameToken);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatInvalidValue_WithValueTypeName_TypeNameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "value", 0, null));
            var valueTypeName = new ValueTypeName("value-type");

            const string expectedMessage = "value-type 'value' is invalid";

            // test

            var message = formatter.FormatInvalidValue(valueToken, valueTypeName, null);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatInvalidValue_WithoutValueTypeName_DefaultTypeNameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "value", 0, null));

            const string expectedMessage = "value 'value' is invalid";

            // test

            var message = formatter.FormatInvalidValue(valueToken, null, null);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatInvalidValue_WithInnerException_InnerExceptionMessageIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "value", 0, null));
            var valueTypeName = new ValueTypeName("value-type");
            var innerException = new Exception("inner-exception-message");

            var expectedMessage = JoinLines("value-type 'value' is invalid", innerException.Message);

            // test

            var message = formatter.FormatInvalidValue(valueToken, valueTypeName, innerException);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatInvalidValue_WithAvailableValues_AvailableValuesListIsIncludedInMessage()
        {
            // setup

            var similarityComparer = TestSimilarityComparer.None; // do not match any value
            var formatter = new UsageExceptionMessageFormatter(similarityComparer: similarityComparer);
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "value", 0, null));
            var description = new ValueTypeDescription("value-type", ["available-value1", "available-value2"]);

            var expectedMessage = JoinLines(
                "value-type 'value' is invalid",
                "",
                "Available value-types are:",
                "    available-value1",
                "    available-value2");

            // test

            var message = formatter.FormatInvalidValue(valueToken, description, null);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatInvalidValue_WithMatchingSimilarValues_SimilarValuesListIsIncludedInMessage()
        {
            // setup

            var similarityComparer = TestSimilarityComparer.All; // match all values
            var formatter = new UsageExceptionMessageFormatter(similarityComparer: similarityComparer);
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 0, "value", 0, null));
            var description = new ValueTypeDescription("value-type", ["available-value1", "available-value2"]);

            var expectedMessage = JoinLines(
                "value-type 'value' is invalid",
                "",
                "The most similar value-types are:",
                "    available-value1",
                "    available-value2");

            // test

            var message = formatter.FormatInvalidValue(valueToken, description, null);

            Assert.AreEqual(expectedMessage, message);

        }

        [TestMethod]
        public void FormatInvalidParameterValue_WithValueTypeName_TypeNameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 1, "value", 0, null));
            var valueTypeName = new ValueTypeName("value-type");

            const string expectedMessage = "--parameter parameter value-type 'value' is invalid";

            // test

            var message = formatter.FormatInvalidParameterValue(nameToken.Value, nameToken, valueToken, valueTypeName, null);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatInvalidParameterValue_WithoutValueTypeName_DefaultTypeNameIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 1, "value", 0, null));

            const string expectedMessage = "--parameter parameter value 'value' is invalid";

            // test

            var message = formatter.FormatInvalidParameterValue(nameToken.Value, nameToken, valueToken, null, null);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatInvalidParameterValue_WithInnerException_InnerExceptionMessageIsIncludedInMessage()
        {
            // setup

            var formatter = new UsageExceptionMessageFormatter();
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 1, "value", 0, null));
            var valueTypeName = new ValueTypeName("value-type");
            var innerException = new Exception("inner-exception-message");

            var expectedMessage = JoinLines("--parameter parameter value-type 'value' is invalid", innerException.Message);

            // test

            var message = formatter.FormatInvalidParameterValue(nameToken.Value, nameToken, valueToken, valueTypeName, innerException);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatInvalidParameterValue_WithAvailableValues_AvailableValuesListIsIncludedInMessage()
        {
            // setup

            var similarityComparer = TestSimilarityComparer.None; // do not match any value
            var formatter = new UsageExceptionMessageFormatter(similarityComparer: similarityComparer);
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 1, "value", 0, null));
            var description = new ValueTypeDescription("value-type", ["available-value1", "available-value2"]);

            var expectedMessage = JoinLines(
                "--parameter parameter value-type 'value' is invalid",
                "",
                "Available value-types are:",
                "    available-value1",
                "    available-value2");

            // test

            var message = formatter.FormatInvalidParameterValue(nameToken.Value, nameToken, valueToken, description, null);

            Assert.AreEqual(expectedMessage, message);
        }

        [TestMethod]
        public void FormatInvalidParameterValue_WithMatchingSimilarValues_SimilarValuesListIsIncludedInMessage()
        {
            // setup

            var similarityComparer = TestSimilarityComparer.All; // match all values
            var formatter = new UsageExceptionMessageFormatter(similarityComparer: similarityComparer);
            var nameToken = new ParameterToken(ParameterTokenType.Name, 0, "parameter", 2, new ParameterToken(ParameterTokenType.RawArg, 0, "--parameter", 0, null));
            var valueToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, new ParameterToken(ParameterTokenType.RawArg, 1, "value", 0, null));
            var description = new ValueTypeDescription("value-type", ["available-value1", "available-value2"]);

            var expectedMessage = JoinLines(
                "--parameter parameter value-type 'value' is invalid",
                "",
                "The most similar value-types are:",
                "    available-value1",
                "    available-value2");

            // test

            var message = formatter.FormatInvalidParameterValue(nameToken.Value, nameToken, valueToken, description, null);

            Assert.AreEqual(expectedMessage, message);
        }

        private static IEnumerable<NamePredicate> CreateNamePredicates(params string[] names)
        {
            return names.Select(name => new NamePredicate(name));
        }

        private static string JoinLines(params string[] lines)
        {
            return String.Join(Environment.NewLine, lines);
        }
    }
}
