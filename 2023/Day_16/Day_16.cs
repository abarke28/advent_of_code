using Aoc.Common;
using Aoc.Utils;

namespace Aoc.Y2023.Day_16
{
    // https://adventofcode.com/2023/day/16
    public class Day_16 : ISolver
    {
        public object Part1(IList<string> lines)
        {
            var map = ParseLines(lines);

            var startPosition = new Vector2D(0, map.Height - 1);
            var startHeading = Vector2D.Right;

            var squares = GetEnergizedSquares(map, new Pose(startPosition, startHeading));

            return squares.Count;
        }

        public object Part2(IList<string> lines)
        {
            var map = ParseLines(lines);

            var poses = GetAllPossibleStarts(map);

            var energizedSquares = poses.Select(p => GetEnergizedSquares(map, p));

            var max = energizedSquares.Max(es => es.Count);

            return max;
        }

        private static List<Pose> GetAllPossibleStarts(Grid<char> map)
        {
            var possibleStartPositions = map.FindAll((x, y) => x == 0 || y == 0 || x == (map.Width - 1) || y == (map.Height - 1));

            var poses = new List<Pose>(possibleStartPositions.Count() + 4);

            foreach (var possibleStart in possibleStartPositions)
            {
                if (possibleStart.X == 0)
                {
                    // Left edge => face right
                    poses.Add(new Pose(possibleStart, Vector2D.Right));
                }
                if (possibleStart.X == map.Width - 1)
                {
                    // Right edge => face left
                    poses.Add(new Pose(possibleStart, Vector2D.Left));
                }
                if (possibleStart.Y == 0)
                {
                    // Bottom edge => face up
                    poses.Add(new Pose(possibleStart, Vector2D.Up));
                }
                if (possibleStart.Y == map.Height - 1)
                {
                    // Top edge => face down
                    poses.Add(new Pose(possibleStart, Vector2D.Down));
                }
            }

            return poses;
        }

        private static List<Vector2D> GetEnergizedSquares(Grid<char> map, Pose startPose)
        {
            var seenSquares = new HashSet<Vector2D>();
            var seenPoses = new HashSet<string>();

            var toProcess = new Queue<Pose>(new List<Pose> { startPose });

            while (toProcess.Count > 0)
            {
                var current = toProcess.Dequeue();

                seenSquares.Add(current.Pos);

                if (seenPoses.Add(current.GetRepresentativeString()))
                {
                    var nextPoses = GetNextPoses(map, current);

                    if (nextPoses.Any())
                    {
                        foreach (var pose in nextPoses)
                        {
                            toProcess.Enqueue(pose);
                        }
                    }
                }
            }

            return seenSquares.ToList();
        }

        private static IEnumerable<Pose> GetNextPoses(Grid<char> map, Pose pose)
        {
            var currentGridElement = map.GetValue(pose.Pos);

            var targets = (currentGridElement, pose.Face.X, pose.Face.Y) switch
            {
                ('.', _, _) => new[] { new Pose(pose.Pos + pose.Face, pose.Face) },
                ('/', 0, 1) => new[] { new Pose(pose.Pos + Vector2D.Right, Vector2D.Right) },
                ('/', -1, 0) => new[] { new Pose(pose.Pos + Vector2D.Down, Vector2D.Down) },
                ('/', 0, -1) => new[] { new Pose(pose.Pos + Vector2D.Left, Vector2D.Left) },
                ('/', 1, 0) => new[] { new Pose(pose.Pos + Vector2D.Up, Vector2D.Up) },
                ('\\', 0, 1) => new[] { new Pose(pose.Pos + Vector2D.Left, Vector2D.Left) },
                ('\\', -1, 0) => new[] { new Pose(pose.Pos + Vector2D.Up, Vector2D.Up) },
                ('\\', 0, -1) => new[] { new Pose(pose.Pos + Vector2D.Right, Vector2D.Right) },
                ('\\', 1, 0) => new[] { new Pose(pose.Pos + Vector2D.Down, Vector2D.Down) },
                ('-', _, 0) => new[] { new Pose(pose.Pos + pose.Face, pose.Face) },
                ('-', 0, _) => new[] { new Pose(pose.Pos + Vector2D.Left, Vector2D.Left), new Pose(pose.Pos + Vector2D.Right, Vector2D.Right) },
                ('|', _, 0) => new[] { new Pose(pose.Pos + Vector2D.Up, Vector2D.Up), new Pose(pose.Pos + Vector2D.Down, Vector2D.Down) },
                ('|', 0, _) => new[] { new Pose(pose.Pos + pose.Face, pose.Face) },
                (_, _, _) => throw new Exception($"Can not compute next pose for position {pose.Pos} with heading {pose.Face} on square {currentGridElement}")
            };

            var validTargets = targets.Where(t => map.IsInBounds(t.Pos));

            return validTargets;
        }

        public static Grid<char> ParseLines(IList<string> lines)
        {
            return Grid<char>.FromStrings(lines, c => c);
        }
    }
}
