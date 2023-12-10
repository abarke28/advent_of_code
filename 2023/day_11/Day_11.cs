using aoc.common;
using aoc.utils;

namespace aoc.y2023.day_11
{
    // https://adventofcode.com/2023/day/11
    public class Day_11 : ISolver
    {
        public object Part1(IList<string> lines)
        {
            var l = new List<string>()
            {
                "abc",
                "def",
                "ghi"
            };

            var g1 = Grid<Char>.FromStrings(l, c => c);

            g1.PrintGrid();

            var zero = g1.GetValue(0, 0);
            var max = g1.GetValue(2, 2);
            Console.WriteLine($"(0,0) = {zero}");
            Console.WriteLine($"(2,2) = {max}");

            return 0;
        }

        public object Part2(IList<string> lines)
        {
            return 0;
        }
    }
}
