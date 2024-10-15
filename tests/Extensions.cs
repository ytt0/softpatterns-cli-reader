namespace SoftPatterns.Cli.Reader.Tests
{
    public static class Extensions
    {
        public static bool TryGetAllItems<T>(this IEnumerable<T> items, [MaybeNullWhen(false)] out T item)
        {
            if (items.Count() != 1)
            {
                item = default;
                return false;
            }

            item = items.ElementAt(0);
            return true;
        }

        public static bool TryGetAllItems<T>(this IEnumerable<T> items, [MaybeNullWhen(false)] out T item1, [MaybeNullWhen(false)] out T item2)
        {
            if (items.Count() != 2)
            {
                item1 = default;
                item2 = default;
                return false;
            }

            item1 = items.ElementAt(0);
            item2 = items.ElementAt(1);
            return true;
        }

        public static bool TryGetAllItems<T>(this IEnumerable<T> items, [MaybeNullWhen(false)] out T item1, [MaybeNullWhen(false)] out T item2, [MaybeNullWhen(false)] out T item3)
        {
            if (items.Count() != 3)
            {
                item1 = default;
                item2 = default;
                item3 = default;
                return false;
            }

            item1 = items.ElementAt(0);
            item2 = items.ElementAt(1);
            item3 = items.ElementAt(2);
            return true;
        }
    }
}
