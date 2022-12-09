namespace aoc.utils
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

        public static IEnumerable<string> GetWords(this string s, char delimiter = Space)
        {
            return s.Split(delimiter);
        }
    }
}
