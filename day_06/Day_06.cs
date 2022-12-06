using aoc.utils;

namespace aoc.day_06
{
    // https://adventofcode.com/2022/day/6
    public class Day_06
    {
        public static void GetResult()
        {
            var lines = FileUtils.ReadAllLines("day_06/input.txt");

            var input = lines[0];

            var num1 = FindIndexOfFirstFourUniqueChars(input, 4);
            var num2 = FindIndexOfFirstFourUniqueChars(input, 14);

            Console.WriteLine(num1);
            Console.WriteLine(num2);
        }

        private static int FindIndexOfFirstFourUniqueChars(string s, int numOfChars)
        {
            var lastN = new Queue<char>(numOfChars);

            for(int i=0;  i<= s.Length; i++)
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

        private static bool IsQueueUnique(Queue<char> queue)
        {
            return (queue.Count == queue.ToHashSet().Count);
        }
    }
}
