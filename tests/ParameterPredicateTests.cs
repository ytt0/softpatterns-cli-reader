namespace SoftPatterns.Cli.Reader.Tests
{
    [TestClass]
    public class NamePredicateTests
    {
        [TestMethod]
        public void GetAvailableNames_WithVisibleName_AvailableNamesContainsName()
        {
            // setup
            var name = "name";
            var namePredicate = new NamePredicate(name, isVisible: true);

            // test
            CollectionAssert.AreEquivalent(new[] { name }.ToArray(), namePredicate.AvailableNames?.ToArray());
        }

        [TestMethod]
        public void GetAvailableNames_WithInvisibleName_AvailableNamesIsEmpty()
        {
            // setup
            var name = "name";
            var namePredicate = new NamePredicate(name, isVisible: false);

            // test
            Assert.IsNull(namePredicate.AvailableNames);
        }

        [TestMethod]
        public void GetAvailableNames_WithShortName_AvailableNamesDoesNotContainShortName()
        {
            // setup
            var name = "name";
            var shortName = "n";
            var namePredicate = new NamePredicate(name, shortName, isVisible: true);

            // test
            CollectionAssert.AreEquivalent(new[] { name }.ToArray(), namePredicate.AvailableNames?.ToArray());
        }

        [TestMethod]
        public void GetAvailableNames_WithInvalidShortName_ThrowsArgumentException()
        {
            // setup
            var name = "name";
            var invalidShortName = "invalid-short-name"; // longer than one character

            // test

            Assert.ThrowsException<ArgumentException>(() => new NamePredicate(name, invalidShortName));
        }

        [TestMethod]
        public void Match_WithNameAndNonDefaultComparison_ComparisonIsUsed()
        {
            // setup
            var name = "name";
            var nameComparison = StringComparison.InvariantCulture; // do not ignore case for this name
            var defaultComparison = StringComparison.InvariantCultureIgnoreCase;

            var matchingToken = new ParameterToken(ParameterTokenType.Name, 0, "name", 0, null);
            var nonMatchingToken = new ParameterToken(ParameterTokenType.Name, 0, "NAME", 0, null); // can only match with default comparison

            var namePredicate = new NamePredicate(name, nameComparison: nameComparison);

            // test
            Assert.IsTrue(namePredicate.Match(matchingToken, defaultComparison));
            Assert.IsFalse(namePredicate.Match(nonMatchingToken, defaultComparison));
        }

        [TestMethod]
        public void Match_WithNameAndWithoutComparison_DefaultComparisonIsUsed()
        {
            // setup
            var name = "name";
            var defaultComparison = StringComparison.InvariantCulture; // do not ignore case by default

            var matchingToken = new ParameterToken(ParameterTokenType.Name, 0, "name", 0, null);
            var nonMatchingToken = new ParameterToken(ParameterTokenType.Name, 0, "NAME", 0, null); // can not match with default comparison

            var namePredicate = new NamePredicate(name, nameComparison: null); // no name comparison override

            // test
            Assert.IsTrue(namePredicate.Match(matchingToken, defaultComparison));
            Assert.IsFalse(namePredicate.Match(nonMatchingToken, defaultComparison));
        }

        [TestMethod]
        public void Match_WithShortNameAndNonDefaultComparison_ComparisonIsUsed()
        {
            // setup
            var name = "name";
            var shortName = "n";
            var nameComparison = StringComparison.InvariantCulture; // do not ignore case for this name
            var defaultComparison = StringComparison.InvariantCultureIgnoreCase;

            var groupToken = new ParameterToken(ParameterTokenType.NameGroup, 0, shortName, 0, null);
            var matchingToken = new ParameterToken(ParameterTokenType.ShortName, 0, "n", 0, groupToken);
            var nonMatchingToken = new ParameterToken(ParameterTokenType.ShortName, 0, "N", 0, groupToken); // can only match with default comparison

            var namePredicate = new NamePredicate(name, shortName, nameComparison: nameComparison);

            // test
            Assert.IsTrue(namePredicate.Match(matchingToken, defaultComparison));
            Assert.IsFalse(namePredicate.Match(nonMatchingToken, defaultComparison));
        }

        [TestMethod]
        public void Match_WithShortNameAndWithoutComparison_DefaultComparisonIsUsed()
        {
            // setup
            var name = "name";
            var shortName = "n";
            var defaultComparison = StringComparison.InvariantCulture; // do not ignore case by default

            var groupToken = new ParameterToken(ParameterTokenType.NameGroup, 0, shortName, 0, null);
            var matchingToken = new ParameterToken(ParameterTokenType.ShortName, 0, "n", 0, groupToken);
            var nonMatchingToken = new ParameterToken(ParameterTokenType.ShortName, 0, "N", 0, groupToken); // can not match with default comparison

            var namePredicate = new NamePredicate(name, shortName, nameComparison: null); // no name comparison override

            // test
            Assert.IsTrue(namePredicate.Match(matchingToken, defaultComparison));
            Assert.IsFalse(namePredicate.Match(nonMatchingToken, defaultComparison));
        }
    }

    [TestClass]
    public class ValuePredicateTests
    {
        [TestMethod]
        public void GetAvailableValues_WithVisibleValue_AvailableValuesContainsValue()
        {
            // setup
            var value = "value";
            var valuePredicate = new ValuePredicate(value, isVisible: true);

            // test
            CollectionAssert.AreEquivalent(new[] { value }.ToArray(), valuePredicate.ValueTypeDescription?.AvailableValues?.ToArray());
        }

        [TestMethod]
        public void GetAvailableValues_WithInvisibleValue_AvailableValuesIsEmpty()
        {
            // setup
            var value = "value";
            var valuePredicate = new ValuePredicate(value, isVisible: false);

            // test
            Assert.IsNull(valuePredicate.ValueTypeDescription?.AvailableValues);
        }

        [TestMethod]
        public void Match_WithValueAndNonDefaultComparison_ComparisonIsUsed()
        {
            // setup
            var value = "value";
            var valueComparison = StringComparison.InvariantCulture; // do not ignore case for this value
            var defaultComparison = StringComparison.InvariantCultureIgnoreCase;

            var matchingToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, null);
            var nonMatchingToken = new ParameterToken(ParameterTokenType.Value, 0, "VALUE", 0, null); // can only match with default comparison

            var valuePredicate = new ValuePredicate(value, valueComparison: valueComparison);

            // test
            Assert.IsTrue(valuePredicate.Match(matchingToken, defaultComparison));
            Assert.IsFalse(valuePredicate.Match(nonMatchingToken, defaultComparison));
        }

        [TestMethod]
        public void Match_WithValueAndWithoutComparison_DefaultComparisonIsUsed()
        {
            // setup
            var value = "value";
            var defaultComparison = StringComparison.InvariantCulture; // do not ignore case by default

            var matchingToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, null);
            var nonMatchingToken = new ParameterToken(ParameterTokenType.Value, 0, "VALUE", 0, null); // can not match with default comparison

            var valuePredicate = new ValuePredicate(value, valueComparison: null); // no value comparison override

            // test
            Assert.IsTrue(valuePredicate.Match(matchingToken, defaultComparison));
            Assert.IsFalse(valuePredicate.Match(nonMatchingToken, defaultComparison));
        }
    }
}
