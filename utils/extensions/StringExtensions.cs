using System.Text.RegularExpressions;

namespace aoc.utils.extensions
{
    public static class StringUtils
    {
        public const char Space = ' ';

        public static IEnumerable<int> ReadAllNumbers(this string s, char delimiter = Space)
        {
            var words = s.Split(delimiter);

            foreach (var word in words)
            {
                if (int.TryParse(word, out var num))
                {
                    yield return num;
                }
            }
        }

        public static IEnumerable<long> ReadAllNumbersLong(this string s, char delimiter = Space)
        {
            var words = s.Split(delimiter);

            foreach (var word in words)
            {
                if (long.TryParse(word, out var num))
                {
                    yield return num;
                }
            }
        }

        public static IEnumerable<string> GetWords(this string s, char delimiter = Space)
        {
            return s.Split(delimiter);
        }

        public static IEnumerable<int> AllIndexesOf(this string source, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            var index = 0;
            while (true)
            {
                index = source.IndexOf(value, index);

                if (index == -1)
                {
                    break;
                }

                yield return index++;
            }
        }

        public static IEnumerable<int> AllIndexesOf(this string source, Regex expression)
        {
            var matches = expression.Matches(source);

            for (var i = 0; i < matches.Count; i++)
            {
                yield return matches[i].Index;
            }
        }
    }
}
