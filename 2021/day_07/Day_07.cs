using aoc.common;
using aoc.utils;

namespace aoc.y2021.day_07
{
    // https://adventofcode.com/2021/day/07
    public class Day_07 : ISolver
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_07/input.txt");

            var positions = lines.Single().ReadAllNumbers(',');

            var fuelCosts = CalculateFuelCosts(positions, d => d);
            Console.WriteLine(fuelCosts.Min());

            // Sum of 1, 2, 3, ..., N is the well known N(N - 1)/2
            var complexFuelCosts = CalculateFuelCosts(positions, d => d * (d + 1) / 2);
            Console.WriteLine(complexFuelCosts.Min());
        }

        private static IEnumerable<int> CalculateFuelCosts(IEnumerable<int> positions, Func<int, int> fuelCalculator)
        {
            var maxPosition = positions.Max();
            var minPosition = positions.Min();

            var fuelCosts = Enumerable.Repeat(0, maxPosition - minPosition + 1).ToArray();

            for (var i = 0; i < fuelCosts.Count(); i++)
            {
                foreach (var position in positions)
                {
                    fuelCosts[i] += fuelCalculator(Math.Abs(position - i));
                }
            }

            return fuelCosts;
        }
    }
}
