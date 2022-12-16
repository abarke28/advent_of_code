using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2021.day_09
{
    // https://adventofcode.com/2021/day/09
    public class Day_09 : ISolver
    {
        private const int MaxHeight = 9;

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_09/input.txt");

            var caveMap = Grid<int>.FromStrings(lines, c => c - '0');

            var lowPoints = FindLowPoints(caveMap);
            var riskScore = ComputeRiskLevel(lowPoints, caveMap);
            Console.WriteLine(riskScore);

            var basinSizes = FindBasinSizes(lowPoints, caveMap);
            var top3BasinScore = basinSizes.OrderByDescending(bs => bs)
                                           .Take(3)
                                           .Aggregate((l, r) => l * r);

            Console.WriteLine(top3BasinScore);
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

        private static List<int> FindBasinSizes(IEnumerable<Vector2D> lowPoints, Grid<int> grid)
        {
            var basinSizes = new List<int>();

            foreach (var lowPoint in lowPoints)
            {
                var basin = GetBasinNeighbours(lowPoint, new HashSet<Vector2D>(), grid);
                basinSizes.Add(basin.Count());
            }

            return basinSizes;
        }

        private static IEnumerable<Vector2D> GetBasinNeighbours(Vector2D node, HashSet<Vector2D> nodesAlreadyInBasin, Grid<int> grid)
        {
            nodesAlreadyInBasin.Add(node);
            var currentHeight = grid.GetValue(node);

            var basinNeighbours = grid.Get4NeighboursWithCoords(node.X, node.Y, v => v > currentHeight && v < MaxHeight)
                                      .Select(bn => new Vector2D(bn.X, bn.Y))
                                      .Where(n => !nodesAlreadyInBasin.Contains(n));

            foreach (var neighbour in basinNeighbours)
            {
                nodesAlreadyInBasin.AddRange(GetBasinNeighbours(neighbour, nodesAlreadyInBasin, grid));
            }

            return nodesAlreadyInBasin;
        }
    }
}
