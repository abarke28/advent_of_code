using Aoc.Common;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_09
{
    // https://adventofcode.com/2023/day/09
    public class Day_09 : ISolver
    {
        public object Part1(IList<string> lines)
        {
            var lists = ParseLines(lines);

            var differenceListsList = lists.Select(l => ComputeDifferenceLists(l));

            var extrapolatedNums = differenceListsList.Select(dll => ComputeDifferenceListMissingNumber(dll));

            return extrapolatedNums.Sum();
        }

        public object Part2(IList<string> lines)
        {
            var lists = ParseLines(lines);

            lists.ForEach(l => l.Reverse());    

            var differenceListsList = lists.Select(l => ComputeDifferenceLists(l));

            var extrapolatedNums = differenceListsList.Select(dll => ComputeDifferenceListMissingNumber(dll));

            return extrapolatedNums.Sum();
        }

        private static int ComputeDifferenceListMissingNumber(List<List<int>> diffs)
        {
            var nextExtrapolation = 0;

            for (int i = diffs.Count; i > 0; i--)
            {
                nextExtrapolation = diffs[i - 1].Last() + nextExtrapolation;
                diffs[i - 1].Add(nextExtrapolation);
            }

            return diffs.First().Last();
        }

        private static List<List<int>> ComputeDifferenceLists(List<int> nums)
        {
            var resultLists = new List<List<int>>
            {
                nums
            };

            var currentList = nums;

            while (!currentList.All(n => n == 0))
            {
                var newList = new List<int>(currentList.Count - 1);

                for (int i = 1; i < currentList.Count; i++)
                {
                    newList.Add(currentList[i] - currentList[i - 1]);
                }

                resultLists.Add(newList);
                currentList = newList;
            }

            return resultLists;
        }

        private static List<List<int>> ParseLines(IList<string> lines)
        {
            return lines
                .Select(l => l.ReadAllNumbers<int>().ToList())
                .ToList();
        }
    }
}
