using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;
using System.Collections;

namespace Aoc.y2021.day_03
{
    // https://adventofcode.com/2021/day/03
    public class Day_03
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_03/input.txt");

            Grid<int> input = Grid<int>.FromStrings(lines, c => c - '0');

            var mcbs = FindMostCommonBits(input);
            var lcbs = mcbs.Duplicate().Not();

            var gamma = mcbs.ConvertToInt();
            var epsilon = lcbs.ConvertToInt();

            Console.WriteLine($"gamma: {gamma}, epsilon: {epsilon}, power: {gamma * epsilon}");

            Func<int, int, bool> oxygenReducer = (num, mcv) => num == mcv;
            Func<int, bool> oxygenTiebreaker = (num) => num == 1;
            Func<int, int, bool> co2Reducer = (num, mcv) => num != mcv;
            Func<int, bool> co2Tiebreaker = (num) => num == 0;

            var oxygen = FilterInputByBitReducer(input, oxygenReducer, oxygenTiebreaker);
            var co2 = FilterInputByBitReducer(input, co2Reducer, co2Tiebreaker);

            var oxygenInt = oxygen.ReadAsBinary();
            var co2Int = co2.ReadAsBinary();

            Console.WriteLine($"oxygen: {oxygenInt}, co2: {co2Int}, life: {oxygenInt * co2Int}");
        }

        public static BitArray FindMostCommonBits(Grid<int> input)
        {
            var mcbs = new BitArray(input.Width);

            for (var i = 0; i < input.Width; i++)
            {
                var column = input.GetColumn(i);

                mcbs[i] = column.Sum() > input.Height / 2;
            }

            return mcbs;
        }

        public static List<int> FilterInputByBitReducer(Grid<int> input,
                                                        Func<int, int, bool> reducer,
                                                        Func<int, bool> tieBreaker)
        {
            List<int> resultNum = new List<int>(input.Width);

            for (var i = 0; i < input.Width; i++)
            {
                var column = input.GetColumn(i);
                var mcv = column.Sum() > column.Count / 2 ? 1 : 0;

                var nums = input.GetRows();
                var remainingNums = nums.Where(nums => reducer(nums[i], mcv)).ToList();
                
                if (remainingNums.Count == nums.Count / 2 && nums.Count % 2 == 0)
                {
                    remainingNums = nums.Where(nums => tieBreaker(nums[i])).ToList();
                }

                if (remainingNums.Count == 1)
                {
                    resultNum = remainingNums[0];

                    return resultNum;
                }
                else
                {
                    input = new Grid<int>(remainingNums);
                }
            }

            throw new Exception("Filtering unsuccesful");
        }
    }
}
