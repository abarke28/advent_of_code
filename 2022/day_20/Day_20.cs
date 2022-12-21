using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_20
{
    // https://adventofcode.com/2022/day/20
    public class Day_20 : ISolver
    {
        private const int DecryptionKey = 811_589_153;
        private const int Repititions = 10;

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_20/input2.txt");
            var nums = lines.Select(l => long.Parse(l)).ToList();

            var result = MixNumbers(nums, nums);
            var score = ComputeScore(result);
            Console.WriteLine(score);

            //var nums2 = lines.Select(l => long.Parse(l) * DecryptionKey).ToList();
            //var mixedNums = new List<long>(nums2);

            //for (var i = 0; i < Repititions; i++)
            //{
            //    mixedNums = MixNumbers(nums2, mixedNums);
            //}

            //var score2 = ComputeScore(mixedNums);
            //Console.WriteLine(score2);
        }

        private static List<long> MixNumbers(IList<long> numbers, IList<long> startingSequence)
        {
            var length = numbers.Count;
            var indexedNums = startingSequence.WithIndex().ToList();
            var indexTracker = indexedNums.ToDictionary(n => n.index, n => n.index);

            var mixedNums = new LinkedList<long>(startingSequence);
            
            Console.WriteLine(string.Join(',', mixedNums.Select(i => i.ToString())));

            foreach (var (num, originalIndex) in indexedNums)
            {
                    if (num == 0) continue;

                var currentIndex = indexTracker[originalIndex];

                var (targetIndex, headingLeft) = ComputeTargetIndex(num, currentIndex, length);
                Console.WriteLine($"{num} is currently at {currentIndex} and moving to {targetIndex}");

                UpdateAffectedIndices(indexTracker, currentIndex, targetIndex);

                var targetNode = GetNode(mixedNums, targetIndex);
                Console.WriteLine($"Target node has value {targetNode.Value}\n");
                var removalNode = GetNode(mixedNums, currentIndex);

                if (headingLeft)
                {
                    mixedNums.AddBefore(targetNode, num);
                }
                else
                {
                    mixedNums.AddAfter(targetNode, num);
                }

                mixedNums.Remove(removalNode);

                Console.WriteLine(string.Join(',', mixedNums.Select(i => i.ToString())));
            }

            return mixedNums.ToList();
        }

        private static (int index, bool headingLeft) ComputeTargetIndex(long num, long currentIndex, int numsLength)
        {
            var nominalTargetIndex = currentIndex + num;
            var headingLeft = nominalTargetIndex < currentIndex;

            var wrappedTargetIndex = nominalTargetIndex.MathMod(numsLength);

            return ((int)wrappedTargetIndex, nominalTargetIndex < currentIndex);
        }

        private static void UpdateAffectedIndices(Dictionary<int,int> indexTracker, int currentIndex, int targetIndex)
        {
            // Shifting right
            if (currentIndex < targetIndex)
            {
                for (var i = currentIndex + 1; i <= targetIndex; i++)
                {
                    indexTracker[i]--;
                }
            }
            // Shifting left
            else
            {
                for (var i = targetIndex; i < currentIndex; i++)
                {
                    indexTracker[i]++;
                }
            }
        }

        private static LinkedListNode<long> GetNode(LinkedList<long> list, int targetIndex)
        {
            var node = list.First;

            while (targetIndex > 0)
            {
                node = node!.Next;
                targetIndex--;
            }

            return node!;
        }

        private static long ComputeScore(IList<long> numbers)
        {
            const int num1Index = 1000;
            const int num2Index = 2000;
            const int num3Index = 3000;

            var baseIndex = numbers.IndexOf(0);

            var num1 = numbers[(baseIndex + num1Index) % numbers.Count];
            var num2 = numbers[(baseIndex + num2Index) % numbers.Count];
            var num3 = numbers[(baseIndex + num3Index) % numbers.Count];


            return num1 + num2 + num3;
        }
    }
}
