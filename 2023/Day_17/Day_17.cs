using Aoc.Common;
using Aoc.Utils;

namespace Aoc.Y2023.Day_17
{
    // https://adventofcode.com/2023/day/17
    public class Day_17 : ISolver
    {
        private const int MaxDistance = int.MaxValue / 2;

        private const int MaxStraightP1 = 3;
        private const int MaxStraightP2 = 10;
        private const int MinStraightP1 = 0;
        private const int MinStraightP2 = 4;

        private struct StateNode : IEquatable<StateNode>
        {
            public Pose Pose { get; set; }
            public int HeadingCount { get; set; }

            public StateNode(Pose pose, int headingCount)
            {
                Pose = pose;
                HeadingCount = headingCount;
            }

            public IEnumerable<StateNode> GetAdjacent(ISet<Vector2D> validPoints, int minStraight, int maxStraight)
            {
                var nodes = new List<StateNode>();

                if (HeadingCount < maxStraight)
                {
                    var aheadNode = new StateNode(new Pose(this.Pose.Ahead, this.Pose.Face), HeadingCount + 1);

                    if (validPoints.Contains(aheadNode.Pose.Pos))
                    {
                        nodes.Add(aheadNode);
                    }
                }

                if (HeadingCount >= minStraight)
                {
                    var leftFace = new Vector2D(-1 * this.Pose.Face.Y, this.Pose.Face.X);
                    var leftNode = new StateNode(new Pose(this.Pose.Pos + leftFace, leftFace), 1);

                    var rightFace = new Vector2D(this.Pose.Face.Y, -1 * this.Pose.Face.X);
                    var rightNode = new StateNode(new Pose(this.Pose.Pos + rightFace, rightFace), 1);

                    if (validPoints.Contains(leftNode.Pose.Pos))
                    {
                        nodes.Add(leftNode);
                    }

                    if (validPoints.Contains(rightNode.Pose.Pos))
                    {
                        nodes.Add(rightNode);
                    }
                }

                return nodes;
            }

            public string GetRepresentativeString()
            {
                return $"{this.Pose.GetRepresentativeString()}-{this.HeadingCount}";
            }

            public bool Equals(StateNode other)
            {
                return GetRepresentativeString().Equals(other.GetRepresentativeString());
            }

            public override int GetHashCode()
            {
                return GetRepresentativeString().GetHashCode();
            }
        }

        public object Part1(IList<string> lines)
        {
            var map = Grid<int>.FromStrings(lines, c => int.Parse(c.ToString()));

            var startingPoint = new Vector2D(0, map.Height - 1);
            var goal = new Vector2D(map.Width - 1, 0);

            var min = GetLowestHeat(startingPoint, goal, map, MinStraightP1, MaxStraightP1);

            return min;
        }

        public object Part2(IList<string> lines)
        {
            var map = Grid<int>.FromStrings(lines, c => int.Parse(c.ToString()));

            var startingPoint = new Vector2D(0, map.Height - 1);
            var goal = new Vector2D(map.Width - 1, 0);

            var min = GetLowestHeat(startingPoint, goal, map, MinStraightP2, MaxStraightP2);

            return min;
        }

        private static int GetLowestHeat(Vector2D startingPoint, Vector2D goal, Grid<int> map, int minStraight, int maxStraight)
        {
            var starts = new List<StateNode>
            {
                new StateNode(new Pose(startingPoint, Vector2D.Right), 1),
                new StateNode(new Pose(startingPoint, Vector2D.Down), 1)
            };

            var minHeat = MaxDistance;

            var validPoints = map.GetAllPoints().Except(new[] { startingPoint }).ToHashSet();
            var nodes = new List<StateNode>();

            foreach (var v in map.GetAllPoints())
            {
                foreach (var face in new[] { Vector2D.Up, Vector2D.Left, Vector2D.Down, Vector2D.Right })
                {
                    foreach (var headingCount in Enumerable.Range(1, maxStraight))
                    {
                        var stateNode = new StateNode(new Pose(v, face), headingCount);
                        nodes.Add(stateNode);
                    }
                }
            }

            foreach (var start in starts)
            {
                var nodeDistances = nodes.ToDictionary(n => n, _ => MaxDistance);
                nodeDistances[start] = 0;

                var toProcess = new PriorityQueue<StateNode, int>(items: new List<(StateNode, int)> { new(start, 0) });

                var pathFound = false;
                while (toProcess.Count > 0)
                {
                    var current = toProcess.Dequeue();

                    if (current.Pose.Pos == goal && current.HeadingCount >= minStraight)
                    {
                        minHeat = Math.Min(minHeat, nodeDistances[current]);
                        pathFound = true;
                        break;
                    }

                    foreach (var adjacentNode in current.GetAdjacent(validPoints, minStraight, maxStraight))
                    {
                        var distance = nodeDistances[current] + map.GetValue(adjacentNode.Pose.Pos);

                        if (distance < nodeDistances[adjacentNode])
                        {
                            nodeDistances[adjacentNode] = distance;
                            toProcess.Enqueue(adjacentNode, distance);
                        }
                    }
                }

                if (!pathFound)
                {
                    throw new Exception("Could not find path");
                }
            }

            return minHeat;
        }
    }
}
