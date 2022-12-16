using aoc.common;
using aoc.utils;

namespace aoc.y2021.day_09
{
    // https://adventofcode.com/2021/day/09
    public class Day_09 : ISolver
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_09/input.txt");

            var caveMap = Grid<int>.FromStrings(lines, c => c - '0');

            var lowPoints = FindLowPoints(caveMap);
            var riskScore = ComputeRiskLevel(lowPoints, caveMap);

            Console.WriteLine(riskScore);

        }

        private static IEnumerable<Vector2D> FindLowPoints(Grid<int> grid)
        {
            Func<int, int, bool> lowPointPredicate = (x, y) =>
            {
                var value = grid.GetValue(x, y);

                var neighbours = grid.Get4Neighbours(x, y, nv => nv <= value);

                return neighbours.Count == 0;
            };

            var lowPoints = grid.FindAll(lowPointPredicate);

            return lowPoints;
        }

        private static int ComputeRiskLevel(IEnumerable<Vector2D> lowPoints, Grid<int> grid)
        {
            var riskLevel = 0;

            foreach (var lowPoint in lowPoints)
            {
                riskLevel += grid.GetValue(lowPoint) + 1;
            }

            return riskLevel;
        }
    }
}
