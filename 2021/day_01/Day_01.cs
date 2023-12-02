using aoc.common;
using aoc.utils;

namespace aoc.y2021.day_01
{
    // https://adventofcode.com/2021/day/01
    public class Day_01
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_01/input.txt");

            var ints = lines.Select(l => int.Parse(l)).ToList();

            Console.WriteLine(CountIntervalIncreases(ints));
            Console.WriteLine(CountIntervalIncreases(ints, 3));
        }

        private static int CountIntervalIncreases(IList<int> lines, int intervalSize = 1)
        {
            var count = 0;

            for (var i = 0; i < lines.Count - intervalSize; i++)
            {
                var interval1 = lines.Skip(i).Take(intervalSize);
                var interval2 = lines.Skip(i + 1).Take(intervalSize);

                if (interval2.Sum() > interval1.Sum())
                {
                    count++;
                }
            }

            return count;
        }
    }
}
