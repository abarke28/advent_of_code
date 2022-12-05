namespace aoc.utils
{
    public static class FileUtils
    {
        public static List<string> ReadAllLines(string path)
        {
            return File.ReadAllLines(path).ToList();
        }

        public static IEnumerable<int> ReadAllNumbers(string s, char delimiter = ' ')
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
    }
}
