using aoc.common;

namespace aoc.y2021.day_16
{
    // https://adventofcode.com/2021/day/16
    public class Day_16 : ISolver
    {
        public object Part1(IList<string> lines)
        {
            var packet = lines.Single();
            var bits = ConvertHexToBits(packet);

            return 0;
        }

        public object Part2(IList<string> lines)
        {
            return 0;
        }

        private static string ConvertHexToBits(string hex)
        {
            long longVal = Convert.ToInt64(hex, fromBase: 16);

            var bitString = Convert.ToString(longVal, toBase: 2);

            return bitString;
        }
    }
}
