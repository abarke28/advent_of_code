using aoc.common;
using aoc.utils;
using System.Text;

namespace aoc.y2022.day_25
{
    // https://adventofcode.com/2022/day/25
    public class Day_25 : ISolver
    {
        private const int Radix = 5;

        private static readonly Dictionary<char, int> charScoreMap = new()
        {
            { '=', -2 },
            { '-', -1 },
            { '0', 0 },
            { '1', 1 },
            { '2', 2 },
        };

        private static readonly Dictionary<int, char> digitSnafuMap = new()
        {
            { 0, '0' },
            { 1, '1' },
            { 2, '2' },
            { 3, '=' },
            { 4, '-' },
        };

        private static readonly Dictionary<int, int> digitSnafuOffsetMap = new()
        {
            { 0, 0 },
            { 1, 1 },
            { 2, 2 },
            { 3, -2 },
            { 4, -1 }
        };

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_25/input.txt");

            var nums = lines.Select(l => ConvertSnafuToDecimal(l)).ToList();
            var sum = nums.Sum();
            Console.WriteLine(sum);

            var snafu = ConvertDecimalToSnafu(sum);
            Console.WriteLine(snafu);
        }

        private static long ConvertSnafuToDecimal(string s)
        {
            var value = 0L;

            var rev = s.Reverse().ToArray();

            for (var i = 0; i < s.Length; i++)
            {
                value += charScoreMap[rev[i]] * (long)Math.Pow(Radix, i);
            }

            return value;
        }

        private static string ConvertDecimalToSnafu(long n)
        {
            var workingValue = n;
            var snafu = new StringBuilder();

            while (workingValue != 0)
            {
                var radixValue = ((workingValue % Radix) + Radix) % Radix;

                snafu.Append(digitSnafuMap[(int)radixValue]);
                workingValue -= digitSnafuOffsetMap[(int)radixValue];

                workingValue /= Radix;
            }

            return new string(snafu.ToString().Reverse().ToArray());
        }
    }
}
