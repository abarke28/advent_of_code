using System.Numerics;
using System.Text.RegularExpressions;

namespace Aoc.Utils.Extensions
{
    public static class StringUtils
    {
        public const char Space = ' ';

        public static IEnumerable<T> ReadAllNumbers<T>(this string s, char delimiter = Space) where T : INumber<T>
        {
            var words = s.Split(delimiter);

            foreach (var word in words)
            {
                if (T.TryParse(word.AsSpan(), provider: null, out var num))
                {
                    yield return num;
                }
            }
        }

        public static IEnumerable<T> ReadAllNumbers<T>(this string s, char[] delimiters) where T : INumber<T>
        {
            var words = s.Split(delimiters);

            foreach (var word in words)
            {
                if (T.TryParse(word.AsSpan(), provider: null, out var num))
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
