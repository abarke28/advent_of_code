using aoc.common;
using aoc.utils;
using System.Collections;

namespace aoc.y2021.day_03
{
    // https://adventofcode.com/2021/day/03
    public class Day_03 : ISolver
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_03/input.txt");

            Grid<int> input = Grid<int>.FromStrings(lines, c => c - '0');

            var mcbs = FindMostCommonBits(input);
            var lcbs = (mcbs.Clone() as BitArray)!.Not();

            var gamma = mcbs.ConvertToInt();
            var epsilon = lcbs.ConvertToInt();

            Console.WriteLine($"gamma: {gamma}, epsilon: {epsilon}, power: {gamma * epsilon}");
        }

        public static BitArray FindMostCommonBits(Grid<int> input)
        {
            var mcbs = new BitArray(input.Width);

            for (var i = 0; i < input.Width; i++)
            {
                var column = input.GetColumn(i);

                mcbs[i] = column.Sum() > input.Height / 2 ? true : false;
            }

            return mcbs;
        }
    }
}
