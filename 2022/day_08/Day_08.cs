using Aoc.Common;
using Aoc.Utils;

namespace Aoc.y2022.day_08
{
    // https://adventofcode.com/2022/day/8
    public class Day_08
    {
        public void Solve()
        {
            var lines = File.ReadAllLines("2022/day_08/input.txt");

            var grid = Grid<int>.FromStrings(lines, c => int.Parse(c.ToString()));

            var treeCount = 0;
            var maxScore = 0;

            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    if (IsTreeVisible(grid, x, y))
                    {
                        treeCount++;
                    }

                    var treeScore = ComputeViewingScore(grid, x, y);

                    if (treeScore > maxScore)
                    {
                        maxScore = treeScore;
                    }
                }
            }

            Console.WriteLine($"Trees visible from beyond the grid: {treeCount}");
            Console.WriteLine($"Max tree viewing score: {maxScore}");
        }

        private bool IsTreeVisible(Grid<int> grid, int x, int y)
        {
            var canSeeFromLeft = CanSeeFromLeft(grid, x, y);
            var canSeeFromRight = CanSeeFromRight(grid, x, y);
            var canSeeFromUp = CanSeeFromUp(grid, x, y);
            var canSeeFromDown = CanSeeFromDown(grid, x, y);

            return canSeeFromLeft || canSeeFromRight || canSeeFromUp || canSeeFromDown;
        }

        private bool CanSeeFromLeft(Grid<int> grid, int x, int y)
        {
            var tree = grid.GetValue(x, y);

            var leftIndex = x - 1;
            while (true)
            {
                if (!grid.IsInBounds(leftIndex, y))
                {
                    return true;
                }

                var nextTree = grid.GetValue(leftIndex, y);

                if (nextTree >= tree)
                {
                    return false;
                }
                else
                {
                    leftIndex--;
                    continue;
                }
            }
        }

        private bool CanSeeFromRight(Grid<int> grid, int x, int y)
        {
            var tree = grid.GetValue(x, y);

            var rightIndex = x + 1;
            while (true)
            {
                if (!grid.IsInBounds(rightIndex, y))
                {
                    return true;
                }

                var nextTree = grid.GetValue(rightIndex, y);

                if (nextTree >= tree)
                {
                    return false;
                }
                else
                {
                    rightIndex++;
                    continue;
                }
            }
        }

        private bool CanSeeFromUp(Grid<int> grid, int x, int y)
        {
            var tree = grid.GetValue(x, y);

            var upIndex = y + 1;
            while (true)
            {
                if (!grid.IsInBounds(x, upIndex))
                {
                    return true;
                }

                var nextTree = grid.GetValue(x, upIndex);

                if (nextTree >= tree)
                {
                    return false;
                }
                else
                {
                    upIndex++;
                    continue;
                }
            }
        }

        private bool CanSeeFromDown(Grid<int> grid, int x, int y)
        {
            var tree = grid.GetValue(x, y);

            var downIndex = y - 1;
            while (true)
            {
                if (!grid.IsInBounds(x, downIndex))
                {
                    return true;
                }

                var nextTree = grid.GetValue(x, downIndex);

                if (nextTree >= tree)
                {
                    return false;
                }
                else
                {
                    downIndex--;
                    continue;
                }
            }
        }

        private int ComputeViewingScore(Grid<int> grid, int x, int y)
        {
            var tree = grid.GetValue(x, y);

            return 
                GetLeftScore(grid, x, y, tree) * 
                GetRightScore(grid, x, y, tree) * 
                GetDownScore(grid, x, y, tree) * 
                GetUpScore(grid, x, y, tree);
        }

        private int GetLeftScore(Grid<int> grid, int x, int y, int tree)
        {
            var leftScore = 0;

            var leftIndex = x - 1;
            while (true)
            {
                if (!grid.IsInBounds(leftIndex, y))
                {
                    break;
                }

                var nextTree = grid.GetValue(leftIndex, y);

                if (nextTree >= tree)
                {
                    leftScore++;
                    break;
                }
                else
                {
                    leftScore++;
                    leftIndex--;
                    continue;
                }
            }

            return leftScore;
        }

        private int GetRightScore(Grid<int> grid, int x, int y, int tree)
        {
            var rightScore = 0;

            var rightIndex = x + 1;
            while (true)
            {
                if (!grid.IsInBounds(rightIndex, y))
                {
                    break;
                }

                var nextTree = grid.GetValue(rightIndex, y);

                if (nextTree >= tree)
                {
                    rightScore++;
                    break;
                }
                else
                {
                    rightScore++;
                    rightIndex++;
                    continue;
                }
            }

            return rightScore;
        }

        private int GetDownScore(Grid<int> grid, int x, int y, int tree)
        {
            var downScore = 0;

            var downIndex = y - 1;
            while (true)
            {
                if (!grid.IsInBounds(x, downIndex))
                {
                    break;
                }

                var nextTree = grid.GetValue(x, downIndex);

                if (nextTree >= tree)
                {
                    downScore++;
                    break; ;
                }
                else
                {
                    downScore++;
                    downIndex--;
                    continue;
                }
            }

            return downScore;
        }

        private int GetUpScore(Grid<int> grid, int x, int y, int tree)
        {
            var upScore = 0;

            var upIndex = y + 1;
            while (true)
            {
                if (!grid.IsInBounds(x, upIndex))
                {
                    break;
                }

                var nextTree = grid.GetValue(x, upIndex);

                if (nextTree >= tree)
                {
                    upScore++;
                    break;
                }
                else
                {
                    upScore++;
                    upIndex++;
                    continue;
                }
            }

            return upScore;
        }
    }
}
