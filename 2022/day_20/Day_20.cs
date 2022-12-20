using aoc.common;
using aoc.utils;
using aoc.utils.extensions;
using System.Text;
using System.Xml.Linq;

namespace aoc.y2022.day_20
{
    // https://adventofcode.com/2022/day/20
    public class Day_20 : ISolver
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_20/input2.txt");
            var nums = lines.Select(l => int.Parse(l)).ToList();

            var result = MixNumbers(nums);
            Console.WriteLine(string.Join(',', result.Select(i => i.ToString())));
        }

        private static IList<int> MixNumbers(IList<int> numbers)
        {
            var length = numbers.Count;
            var indexedNums = numbers.WithIndex().ToList();
            var indexTracker = indexedNums.ToDictionary(n => n.index, n => n.index);

            var mixedNums = new LinkedList<int>(numbers);
            Console.WriteLine(string.Join(',', mixedNums.Select(i => i.ToString())));

            foreach (var (num, originalIndex) in indexedNums)
            {
                if (num == 0) continue;

                var currentIndex = indexTracker[originalIndex];

                var (targetIndex, wrapped) = ComputeTargetIndex(num, currentIndex, length);

                UpdateAffectedIndices(indexTracker, currentIndex, targetIndex, wrapped);

                var targetNode = GetNode(mixedNums, targetIndex + 1);
                var removalNode = GetNode(mixedNums, currentIndex);

                mixedNums.AddBefore(targetNode, num);
                mixedNums.Remove(removalNode);

                Console.WriteLine(string.Join(',', mixedNums.Select(i => i.ToString())));
            }

            return mixedNums.ToList();
        }

        private static (int index, bool wrapped) ComputeTargetIndex(int num, int currentIndex, int numsLength)
        {
            var nominalTargetIndex = currentIndex + num;

            if (nominalTargetIndex < 0)
            {
                var wrappedIndex = nominalTargetIndex + numsLength;

                return (wrappedIndex, true);
            }
            else if (nominalTargetIndex >= numsLength)
            {
                var wrappedIndex = nominalTargetIndex % numsLength;

                return (wrappedIndex, true);
            }
            else
            {
                return (nominalTargetIndex, false);
            }
        }

        private static void UpdateAffectedIndices(Dictionary<int,int> indexTracker, int currentIndex, int targetIndex, bool wrapped)
        {
            // Shifting right
            if (currentIndex < targetIndex)
            {
                for (var i = currentIndex + 1; i < targetIndex; i++)
                {
                    indexTracker[i]--;
                }
            }
            // Shifting left
            else
            {
                for (var i = targetIndex + 1; i < currentIndex; i++)
                {
                    indexTracker[i]++;
                }
            }
        }

        private static LinkedListNode<int> GetNode(LinkedList<int> list, int targetIndex)
        {
            if (targetIndex > list.Count / 2)
            {
                var node = list.Last;
                var traversals = list.Count - 1 - targetIndex;

                while (traversals-- > 0)
                {
                    node = node!.Previous;
                }

                return node!;
            }
            else
            {
                var node = list.First;

                while (targetIndex-- > 0)
                {
                    node = node!.Next;
                }

                return node!;
            }
        }

        private static int ComputeScore(IList<int> numbers)
        {
            const int num1Index = 1000;
            const int num2Index = 2000;
            const int num3Index = 3000;

            return numbers[num1Index - 1] + numbers[num2Index - 1] + numbers[num3Index- 1];
        }
    }
}
