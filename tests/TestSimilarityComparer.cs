namespace SoftPatterns.Cli.Reader.Tests
{
    public static class TestSimilarityComparer
    {
        private class AllSimilarityComparer : ISimilarityComparer
        {
            public IEnumerable<string> GetSimilarValues(string value, IEnumerable<string> values)
            {
                return values;
            }
        }

        private class NoneSimilarityComparer : ISimilarityComparer
        {
            public IEnumerable<string> GetSimilarValues(string value, IEnumerable<string> values)
            {
                return [];
            }
        }

        public static readonly ISimilarityComparer All = new AllSimilarityComparer();
        public static readonly ISimilarityComparer None = new NoneSimilarityComparer();
    }
}
