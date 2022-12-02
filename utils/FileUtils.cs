namespace aoc.utils
{
    public static class FileUtils
    {
        public static List<string> ReadAllLines(string path)
        {
            return File.ReadAllLines(path).ToList();
        }
    }
}
