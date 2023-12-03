using aoc.common;
using aoc.utils;
using aoc.utils.extensions;
using System.Text;

namespace aoc.y2023.day_03
{
    // https://adventofcode.com/2023/day/03
    public class Day_03 : ISolver
    {
        private const char Nada = '.';

        public object Part1(IList<string> lines)
        {
            var input = lines.Select(l => l.ToList()).ToList();
            var grid = new Grid<char>(input)!;

            var partPoints = grid
                .FindAll(v => !grid[v].IsNumber() && grid[v] != Nada)
                .ToHashSet();

            var numsAndPoints = GetNumbersAndPoints(grid);

            var validNums = new List<int>();
            foreach (var (number, points) in numsAndPoints)
            {
                var validatingPoints = points.SelectMany(p => p.Get8Neighbors());

                if (validatingPoints.Any(partPoints.Contains))
                {
                    validNums.Add(number);
                }
            }

            return validNums.Sum();
        }

        public object Part2(IList<string> lines)
        {
            var input = lines.Select(l => l.ToList()).ToList();
            var grid = new Grid<char>(input)!;

            var gears = grid.FindAll((x, y) => grid[x, y].Equals('*'));

            var numsAndPoints = GetNumbersAndPoints(grid);

            var gearRatioSum = 0;

            foreach (var gear in gears)
            {
                var gearPotentialNeighbors = gear.Get8Neighbors().ToHashSet();

                var borderingNums = numsAndPoints
                    .Where(np => np.points.Any(p => gearPotentialNeighbors.Contains(p)))
                    .ToArray();

                if (borderingNums.Length > 1)
                {
                    gearRatioSum += borderingNums
                        .Select(bn => bn.number)
                        .Aggregate(1, (n1, n2) => n1 * n2);
                }
            }

            return gearRatioSum;
        }

        private List<(int number, List<Vector2D> points)> GetNumbersAndPoints(Grid<char> grid)
        {
            var numberStarts = grid.FindAll((x, y) => grid[x, y].IsNumber() && (!grid.IsInBounds(x - 1, y) || !grid[x - 1, y].IsNumber()));

            var numsAndPoints = new List<(int number, List<Vector2D> points)>();

            foreach (var numberStart in numberStarts)
            {
                var points = new List<Vector2D> { numberStart };
                var sb = new StringBuilder(grid.GetValue(numberStart).ToString());

                var nextPoint = numberStart + Vector2D.Right;
                while (grid.IsInBounds(nextPoint) &&
                       grid.GetValue(nextPoint).IsNumber())
                {
                    sb.Append(grid.GetValue(nextPoint));
                    points.Add(nextPoint);
                    nextPoint += Vector2D.Right;
                }

                numsAndPoints.Add((int.Parse(sb.ToString()), points));
            }

            return numsAndPoints;
        }
    }
}
