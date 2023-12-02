using aoc.common;
using aoc.utils;

namespace aoc.y2021.day_16
{
    // https://adventofcode.com/2021/day/16
    public class Day_16
    {
        public void Solve()
        {
            var packet = FileUtils.ReadAllLines("2021/day_16/input.txt").Single();
            var bits = ConvertHexToBits(packet);

        }

        public static string ConvertHexToBits(string hex)
        {
            long longVal = Convert.ToInt64(hex, fromBase: 16);

            var bitString = Convert.ToString(longVal, toBase: 2);

            return bitString;
        }
    }
}
