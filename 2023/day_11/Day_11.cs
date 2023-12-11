using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_11
{
    // https://adventofcode.com/2023/day/11
    public class Day_11 : ISolver
    {
        private const char Galaxy = '#';
        private const char Space = '.';

        private const int StretchFactor1 = 2;
        private const int StretchFactor2 = 1_000_000;

        public object Part1(IList<string> lines)
        {
            var (galaxyPairs, emptyRows, emptyColumns) = GetGalaxyPairsAndEmptyIndexes(lines);

            var sum = 0L;

            foreach (var pair in galaxyPairs)
            {
                sum += GetGalaxyDistance(pair[0], pair[1], emptyRows, emptyColumns, StretchFactor1);
            }

            sum /= 2;

            return sum;
        }

        public object Part2(IList<string> lines)
        {
            var (galaxyPairs, emptyRows, emptyColumns) = GetGalaxyPairsAndEmptyIndexes(lines);

            var sum = 0L;

            foreach (var pair in galaxyPairs)
            {
                sum += GetGalaxyDistance(pair[0], pair[1], emptyRows, emptyColumns, StretchFactor2);
            }

            sum /= 2;

            return sum;
        }

        private static long GetGalaxyDistance(Vector2D g1, Vector2D g2, IList<int> stretchedRowIndexes, IList<int> stretchecColumnIndexes, long stretchFactor)
        {
            var nominalDistance = g1.ManhattanDistance(g2);

            var minRowIndex = Math.Min(g1.Y, g2.Y);
            var maxRowIndex = Math.Max(g1.Y, g2.Y);

            var minColumnIndex = Math.Min(g1.X, g2.X);
            var maxColumnIndex = Math.Max(g1.X, g2.X);

            var extraRowStretchedDistance = stretchedRowIndexes.Count(r => r > minRowIndex && r < maxRowIndex) * (stretchFactor - 1);
            var extraColumnStretchedDistance = stretchecColumnIndexes.Count(c => c > minColumnIndex && c < maxColumnIndex) * (stretchFactor - 1);

            return nominalDistance + extraRowStretchedDistance + extraColumnStretchedDistance;
        }

        private static (IList<Vector2D[]> Pairs, IList<int> Rows, IList<int> Columns) GetGalaxyPairsAndEmptyIndexes(IList<string> lines)
        {
            var grid = Grid<char>.FromStrings(lines, c => c);

            var emptyColumns = grid
                .GetColumns()
                .WithIndex()
                .Where(c => c.Item.All(c => c == Space))
                .Select(c => c.Index)
                .ToList();

            var emptyRows = grid
                .GetRows()
                .WithIndex()
                .Where(c => c.Item.All(c => c == Space))
                .Select(c => c.Index)
                .ToList();

            var galaxies = grid.FindAll(c => grid.GetValue(c) == Galaxy).ToList();

            var galaxyPairs = 
                MathUtils.CartesianProduct(Enumerable.Repeat(galaxies, 2))
                .Select(pair => pair.ToArray())
                .ToList();

            return (galaxyPairs, emptyRows, emptyColumns);
        }
    }
}
