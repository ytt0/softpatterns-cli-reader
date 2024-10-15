namespace SoftPatterns.Cli.Reader.Tests
{
    [TestClass]
    public class SimilarityComparerTests
    {
        [TestMethod]
        public void GetSimilarValues_WithAddSimilarValues_SimilarValuesAreReturnedInOrder()
        {
            // setup

            var comparer = new LcsSimilarityComparer(1);

            const string value = "aa";
            var values = new[] { "a", "abb", "aa" };
            var similarValues = new[] { "aa", "a" }; // require 1 or less add operation to get to "aa"

            // test

            CollectionAssert.AreEqual(similarValues, comparer.GetSimilarValues(value, values).ToArray());
        }

        [TestMethod]
        public void GetSimilarValues_WithRemoveSimilarValues_SimilarValuesAreReturnedInOrder()
        {
            // setup

            var comparer = new LcsSimilarityComparer(1);

            const string value = "aa";
            var values = new[] { "aab", "aac", "aabc", "aa" };
            var similarValues = new[] { "aa", "aab", "aac" }; // require 1 or less remove operation to get to "aa"

            // test

            CollectionAssert.AreEqual(similarValues, comparer.GetSimilarValues(value, values).ToArray());
        }

        [TestMethod]
        public void GetSimilarValues_WithReplaceSimilarValues_SimilarValuesAreReturnedInOrder()
        {
            // setup

            var comparer = new LcsSimilarityComparer(1);

            const string value = "aa";
            var values = new[] { "bb", "ab", "ac", "aa" };
            var similarValues = new[] { "aa", "ab", "ac" }; // require 1 or less replace operation to get to "aa"

            // test

            CollectionAssert.AreEqual(similarValues, comparer.GetSimilarValues(value, values).ToArray());
        }
    }
}
