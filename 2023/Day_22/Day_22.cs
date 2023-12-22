using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_22
{
    // https://adventofcode.com/2023/day/22
    public class Day_22 : ISolver
    {
        private static readonly Vector3D Down = -1 * Vector3D.ZHat;

        private class LowerBrick : IComparer<HashSet<Vector3D>>
        {
            public int Compare(HashSet<Vector3D>? l, HashSet<Vector3D>? r)
            {
                var lowestL = l!.Min(b => b.Z);
                var lowestR = r!.Min(b => b.Z);

                return lowestL.CompareTo(lowestR);
            }
        }

        public object Part1(IList<string> lines)
        {
            var bricks = ParseBricks(lines);

            var fallenBricks = SimulateBrickFalls(bricks, out var _);

            var bricksToDisintegrate = CountBricksThatCanBeDisintegrated(fallenBricks);

            return bricksToDisintegrate;
        }

        public object Part2(IList<string> lines)
        {
            var bricks = ParseBricks(lines);

            var fallenBricks = SimulateBrickFalls(bricks, out var x);

            var chainReactionSum = CountBricksThatFallIfEachBrickDisintegrated(fallenBricks);

            return chainReactionSum;
        }

        private static int CountBricksThatFallIfEachBrickDisintegrated(List<HashSet<Vector3D>> bricks)
        {
            var totalFallCount = 0;

            for (int i = 0; i < bricks.Count; i++)
            {
                var brickState = bricks.Select(b => b.ToHashSet()).ToList();
                brickState.RemoveAt(i);

                SimulateBrickFalls(brickState, out var brickFallCount);

                totalFallCount += brickFallCount;
            }

            return totalFallCount;
        }

        private static int CountBricksThatCanBeDisintegrated(List<HashSet<Vector3D>> bricks)
        {
            var count = 0;

            var allPoints = bricks.SelectMany(p => p).ToHashSet();
            var pointsPerLayer = bricks.SelectMany(b => b).GroupBy(b => b.Z).ToDictionary(g => g.Key, g => g.Count());

            foreach (var brick in bricks)
            {
                var maxHeight = brick.Max(p => p.Z);

                var bricksAtLayerAboveCurrentBrick = bricks.Where(b => b.Any(p => p.Z == maxHeight + 1));
                var brickIsSupportingAnother = false;

                foreach (var brickAbove in bricksAtLayerAboveCurrentBrick)
                {
                    var balancePoints = brickAbove.Where(p => p.Z == maxHeight + 1).Select(p => p + Down).ToHashSet();

                    if (balancePoints.Intersect(allPoints).Count() == balancePoints.Intersect(brick).Count())
                    {
                        brickIsSupportingAnother = true;
                        break;
                    }
                }

                if (!brickIsSupportingAnother)
                {
                    count++;
                }
            }

            return count;
        }

        private static List<HashSet<Vector3D>> SimulateBrickFalls(IEnumerable<HashSet<Vector3D>> bricks, out int numBricksFell)
        {
            numBricksFell = 0;
            var bricksList = bricks.ToList();

            bricksList.Sort(new LowerBrick());

            var newBricks = new List<HashSet<Vector3D>>();
            var allBricks = new HashSet<Vector3D>();

            foreach (var brick in bricksList)
            {
                var currentBrickPosition = brick.ToList();
                var brickFell = false;

                while (currentBrickPosition.All(p => !allBricks.Contains(p + Down) && p.Z > 1))
                {
                    brickFell = true;
                    currentBrickPosition = currentBrickPosition.Select(p => p + Down).ToList();
                }

                newBricks.Add(currentBrickPosition.ToHashSet());
                allBricks.AddRange(currentBrickPosition);

                if (brickFell)
                {
                    numBricksFell++;
                }
            }

            return newBricks;
        }

        private static IEnumerable<HashSet<Vector3D>> ParseBricks(IList<string> lines)
        {
            var bricks = new List<HashSet<Vector3D>>(lines.Count);

            foreach (var nums in lines.Select(l => l.ReadAllNumbers<int>(new char[] { ',', '~' }).ToArray()))
            {
                var start = new Vector3D(nums[0], nums[1], nums[2]);
                var end = new Vector3D(nums[3], nums[4], nums[5]);

                var point = Vector3D.Normalize(end - start);

                var brick = new HashSet<Vector3D>();

                var current = start;

                while (current != end)
                {
                    brick.Add(current);
                    current += point;
                }

                brick.Add(end);

                bricks.Add(brick);
            }

            return bricks;
        }
    }
}
