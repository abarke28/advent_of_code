using aoc.common;
using aoc.utils;

namespace aoc.y2021.day_06
{
    // https://adventofcode.com/2021/day/06
    public class Day_06 : ISolver
    {
        private const int CycleTime = 6;
        private const int FirstCycleTime = 8;

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_06/input.txt");

            var fish = lines.Single()
                            .ReadAllNumbers(',')
                            .ToList();

            var newPop = SimulateGrowth(fish, 80);
            Console.WriteLine(newPop.Select(fp => fp.Value).Sum());

            var newPop2 = SimulateGrowth(fish, 256);
            Console.WriteLine(newPop2.Select(fp => fp.Value).Sum());
        }

        private static Dictionary<int, long> SimulateGrowth(List<int> fish, int numDays)
        {
            var fishPop = new Dictionary<int, long>(FirstCycleTime);

            for (var i = 0; i <= FirstCycleTime; i++)
            {
                fishPop.Add(i, 0);
            }

            foreach (var f in fish)
            {
                fishPop[f] += 1;
            }

            while (numDays > 0)
            {
                var newFish = fishPop[0];

                for (var i = 0; i < FirstCycleTime; i++)
                {
                    fishPop[i] = fishPop[i + 1];
                }

                fishPop[FirstCycleTime] = newFish;
                fishPop[CycleTime] += newFish;

                numDays--;
            }

            return fishPop;
        }
    }
}
