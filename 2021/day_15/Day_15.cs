using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2021.day_15
{
    // https://adventofcode.com/2021/day/15
    public class Day_15 : ISolver
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_15/input.txt");

            var graph = GenerateGraph(lines);
            var minDistances = graph.GetMinDistances(0);
            Console.WriteLine(minDistances[graph.VerticesCount - 1]);

            var expandedGrid = GenerateExpandedGrid(lines);
            var expandedMinDistances = expandedGrid.GetMinDistances(Vector2D.Zero, (_, n2) => n2);
            Console.WriteLine(expandedMinDistances[new Vector2D(expandedGrid.Width - 1, expandedGrid.Height - 1)]);
        }

        private static Graph GenerateGraph(IList<string> lines)
        {
            var grid = Grid<int>.FromStrings(lines, c => c - '0');

            var graph = Graph.FromGrid(grid, GetNeighbors, (_, n2) => n2);

            return graph;
        }

        private static Grid<int> GenerateExpandedGrid(IList<string> lines)
        {
            const int expansionFactor = 5;
            const int maxValue = 9;

            var height = lines.Count;
            var width = lines.First().Length;

            var grid = new Grid<int>(width * expansionFactor, height * expansionFactor, 0);

            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x <grid.Width; x++)
                {
                    if (x < width && y < height)
                    {
                        grid.SetValue(x, y, lines[y][x] - '0');
                    }
                    else
                    {
                        var sourceX = x % width;
                        var sourceY = y % height;

                        var nominalValue = grid.GetValue(sourceX, sourceY);

                        var overflowCountX = x / width;
                        var overflowCountY = y / height;

                        var wrappedValue = ((nominalValue + overflowCountX + overflowCountY - 1) % maxValue) + 1;

                        grid.SetValue(x, y, wrappedValue);
                    }
                }
            }

            return grid;
        }

        private static IEnumerable<Vector2D> GetNeighbors(Vector2D v)
        {
            var neighbors = new List<Vector2D>
            {
                v + Vector2D.Up,
                v + Vector2D.Down,
                v + Vector2D.Left,
                v + Vector2D.Right
            };

            return neighbors;
        }
    }
}
