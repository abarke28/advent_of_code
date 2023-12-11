using Aoc.Common;
using Aoc.Utils;

namespace Aoc.y2022.day_06
{
    // https://adventofcode.com/2022/day/6
    public class Day_06
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_06/input.txt");

            var input = lines[0];

            Console.WriteLine(FindIndexOfFirstNUniqueCharsV2(input, 4));
            Console.WriteLine(FindIndexOfFirstNUniqueCharsV2(input, 14));
        }

        private static int FindIndexOfFirstNUniqueChars(string s, int numOfChars)
        {
            var lastN = new Queue<char>(numOfChars);

            for(int i=0; i<= s.Length; i++)
            {
                if (lastN.Count < numOfChars)
                {
                    lastN.Enqueue(s[i]);
                    continue;
                }
                else
                {
                    if (IsQueueUnique(lastN))
                    {
                        return i;
                    }
                    else
                    {
                        lastN.Dequeue();
                        lastN.Enqueue(s[i]);
                    }
                }
            }

            return -1;
        }

        private static int FindIndexOfFirstNUniqueCharsV2(string s, int numOfChars)
        {
            for (int i = 0; i < (s.Length - numOfChars); i++)
            {
                var window = s.Skip(i).Take(numOfChars);

                if (window.Count() == window.ToHashSet().Count)
                {
                    return i + numOfChars;
                }
            }

            return -1;
        }

        private static bool IsQueueUnique(Queue<char> queue)
        {
            return (queue.Count == queue.ToHashSet().Count);
        }
    }
}
