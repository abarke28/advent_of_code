using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_17
{
    // https://adventofcode.com/2023/day/17
    public class Day_17 : ISolver
    {
        private const int Max = int.MaxValue / 2;
        private const int MaxStraight = 3;

        private struct StateNode : IEquatable<StateNode>
        {
            public Pose Pose { get; set; }
            public int HeadingCount { get; set; }

            public StateNode(Pose pose, int headingCount)
            {
                Pose = pose;
                HeadingCount = headingCount;
            }

            public IEnumerable<StateNode> GetAdjacent(ISet<Vector2D> validPoints, int maxStraight)
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

            var min = GetLowestHeat(startingPoint, goal, map);

            return min;
        }

        public object Part2(IList<string> lines)
        {
            var map = Grid<int>.FromStrings(lines, c => int.Parse(c.ToString()));

            return 0;
        }

        private static int GetLowestHeat(Vector2D startingPoint, Vector2D goal, Grid<int> map)
        {
            var start = new StateNode(new Pose(startingPoint, Vector2D.Down), 1);

            var validPoints = map.GetAllPoints().Except(new[] { startingPoint }).ToHashSet();
            var nodes = new List<StateNode>();
                
            foreach (var v in map.GetAllPoints())
            {
                foreach (var face in new[] { Vector2D.Up, Vector2D.Left, Vector2D.Down, Vector2D.Right })
                {
                    foreach (var headingCount in new[] { 1, 2, 3 })
                    {
                        //if (v == startingPoint && face != Vector2D.Right && headingCount != 1) continue;

                        var stateNode = new StateNode(new Pose(v, face), headingCount);
                        nodes.Add(stateNode);
                    }
                }
            }

            var nodeDistances = nodes.ToDictionary(n => n, _ => Max);
            nodeDistances[start] = 0;

            var toProcess = new PriorityQueue<StateNode, int>(items: new List<(StateNode, int)> { new (start, 0) });

            while (toProcess.Count > 0)
            {
                var current = toProcess.Dequeue();

                if (current.Pose.Pos == goal)
                {
                    return nodeDistances[current];
                }

                foreach (var adjacentNode in current.GetAdjacent(validPoints, MaxStraight))
                {
                    var distance = nodeDistances[current] + map.GetValue(adjacentNode.Pose.Pos);

                    if (distance < nodeDistances[adjacentNode])
                    {
                        nodeDistances[adjacentNode] = distance;
                        toProcess.Enqueue(adjacentNode, distance);
                    }
                }
            }

            throw new Exception("Could not find path");
        }
    }
}
