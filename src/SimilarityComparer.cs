namespace SoftPatterns.Cli.Reader
{
    public interface ISimilarityComparer
    {
        IEnumerable<string> GetSimilarValues(string value, IEnumerable<string> values);
    }

    // Longest Common Substring
    public class LcsSimilarityComparer : ISimilarityComparer
    {
        private const int AddCost = 1;
        private const int RemoveCost = 1;
        private const int ReplaceCost = 1;

        private readonly int maxDistance;
        private readonly StringComparison stringComparison;

        public LcsSimilarityComparer(int maxDistance, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            this.maxDistance = maxDistance;
            this.stringComparison = stringComparison;
        }

        public IEnumerable<string> GetSimilarValues(string value, IEnumerable<string> values)
        {
            return values.
                Select(availableValue => (Value: availableValue, Distance: GetDistance(value, availableValue, this.stringComparison))).
                Where(pair => pair.Distance <= this.maxDistance).
                OrderBy(pair => pair.Distance).
                Select(pair => pair.Value).ToArray();
        }

        private static int GetDistance(string value1, string value2, StringComparison stringComparison)
        {
            var distance = new int[value1.Length + 1, value2.Length + 1];

            for (var i = 0; i <= value1.Length; i++)
            {
                distance[i, 0] = i * RemoveCost;
            }

            for (var j = 0; j <= value2.Length; j++)
            {
                distance[0, j] = j * AddCost;
            }

            for (var i = 1; i <= value1.Length; i++)
            {
                for (var j = 1; j <= value2.Length; j++)
                {
                    distance[i, j] = Math.Min(distance[i - 1, j - 1] + (Equals(value1[i - 1], value2[j - 1], stringComparison) ? 0 : ReplaceCost), Math.Min(distance[i - 1, j] + RemoveCost, distance[i, j - 1] + AddCost));
                }
            }

            return distance[value1.Length, value2.Length];
        }

        private static bool Equals(char char1, char char2, StringComparison stringComparison)
        {
            return char1.ToString().Equals(char2.ToString(), stringComparison);
        }
    }

    public class NgramSimilarityComparer : ISimilarityComparer
    {
        private class EqualityComparer : IEqualityComparer<string>
        {
            private readonly StringComparison stringComparison;

            public EqualityComparer(StringComparison stringComparison)
            {
                this.stringComparison = stringComparison;
            }

            public bool Equals(string? x, string? y)
            {
                return x == null && y == null || x?.Equals(y, this.stringComparison) == true;
            }

            public int GetHashCode([DisallowNull] string obj)
            {
                return obj.GetHashCode();
            }
        }

        private readonly double minSimilarity;
        private readonly EqualityComparer comparer;

        public NgramSimilarityComparer(double minSimilarity, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            this.minSimilarity = minSimilarity;
            this.comparer = new EqualityComparer(stringComparison);
        }

        public IEnumerable<string> GetSimilarValues(string value, IEnumerable<string> values)
        {
            var valueNgrams = GetNgrams(value);

            return values.
                Select(availableValue => (Value: availableValue, Distance: GetDistance(valueNgrams, GetNgrams(availableValue), this.comparer))).
                Where(pair => pair.Distance > this.minSimilarity).
                OrderByDescending(pair => pair.Distance).
                Select(pair => pair.Value).ToArray();
        }

        private static string[] GetNgrams(string value)
        {
            return GetNgrams(value, 2).Union(GetNgrams(value, 3)).ToArray();
        }

        private static string[] GetNgrams(string value, int size)
        {
            value = '^' + value + '$';
            size = Math.Min(size, value.Length);

            var values = new string[value.Length + size - 1];
            for (var i = 1 - size; i < value.Length; i++)
            {
                values[i + size - 1] = new string(value.Skip(i).Take(i < 0 ? size + i : size).ToArray());
            }

            return values;
        }

        private static double GetDistance(IEnumerable<string> ngrams1, IEnumerable<string> ngrams2, IEqualityComparer<string> comparer)
        {
            return (double)ngrams1.Intersect(ngrams2, comparer).Count() / ngrams1.Union(ngrams2, comparer).Count();
        }
    }
}
