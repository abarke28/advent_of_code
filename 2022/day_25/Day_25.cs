using aoc.common;
using aoc.utils;
using aoc.utils.extensions;
using System.Text;

namespace aoc.y2022.day_25
{
    // https://adventofcode.com/2022/day/25
    public class Day_25
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

        private static readonly Dictionary<int, char> digitSnafuMap = charScoreMap.Invert();

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
            const int radixOffset = -2;

            var snafu = new StringBuilder();

            while (n != 0)
            {
                var radixValue = n.ModWithOffset(Radix, radixOffset);

                snafu.Append(digitSnafuMap[(int)radixValue]);
                n -= radixValue;

                n /= Radix;
            }

            return new string(snafu.ToString().Reverse().ToArray());
        }
    }
}
