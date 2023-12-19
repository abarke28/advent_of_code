using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_17
{
    // https://adventofcode.com/2023/day/17
    public class Day_17 : ISolver
    {
        private const int MaxStraight = 3;

        private class StateNode : IEquatable<StateNode>
        {
            public Pose Pose { get; set; }
            public int HeadingCount { get; set; }

            public StateNode(Pose pose, int headingCount)
            {
                Pose = pose;
                HeadingCount = headingCount;
            }

            public IEnumerable<StateNode> GetAdjacent(ISet<Vector2D> validPoints)
            {
                var nodes = new List<StateNode>();

                if (HeadingCount < MaxStraight)
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

            public bool Equals(StateNode? other)
            {
                if (other is null) return false;

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
            var startingPoints = new List<StateNode>
            {
                new StateNode(new Pose(startingPoint, Vector2D.Right), 1),
                //new StateNode(new Pose(startingPoint, Vector2D.Down), 1)
            };

            var goals = new List<StateNode>
            {
                new StateNode(new Pose(goal, Vector2D.Right), 1),
                new StateNode(new Pose(goal, Vector2D.Right), 2),
                new StateNode(new Pose(goal, Vector2D.Right), 3),
                new StateNode(new Pose(goal, Vector2D.Down), 1),
                new StateNode(new Pose(goal, Vector2D.Down), 2),
                new StateNode(new Pose(goal, Vector2D.Down), 3),
            }.ToHashSet();

            var validPoints = map.GetAllPoints().ToHashSet();
            var nodes = new List<StateNode>();

            foreach (var v in validPoints)
            {
                foreach (var face in new[] { Vector2D.Up, Vector2D.Left, Vector2D.Down, Vector2D.Right })
                {
                    //if (v == startingPoint && (face == Vector2D.Up || face == Vector2D.Left)) continue;

                    foreach (var headingCount in new[] { 1, 2, 3 })
                    {
                        var stateNode = new StateNode(new Pose(v, face), headingCount);
                        nodes.Add(stateNode);
                    }
                }
            }

            var min = int.MaxValue / 2;

            foreach (var possibleStart in startingPoints)
            {
                var start = possibleStart;

                var unvisistedNotes = nodes.Select(n => n.GetRepresentativeString()).ToHashSet();
                var nodeDistances = nodes.ToDictionary(n => n, _ => int.MaxValue);

                nodeDistances[start] = 0;

                while (unvisistedNotes.Any())
                {
                    var current = GetMinUnvisited(nodeDistances, unvisistedNotes);

                    if (current.HeadingCount == 0) break;

                    unvisistedNotes.Remove(current.GetRepresentativeString());

                    var adjacentNodes = current.GetAdjacent(validPoints);

                    foreach (var adjacentNode in adjacentNodes)
                    {
                        //if (nodeDistances[current] == int.MaxValue / 10)
                        //{

                        //}

                        var newDistanceToAdjacentNode = nodeDistances[current] + map.GetValue(adjacentNode.Pose.Pos);

                        nodeDistances[adjacentNode] = Math.Min(nodeDistances[adjacentNode], newDistanceToAdjacentNode);
                    }
                }

                var minDistanceForStart = nodeDistances
                    .Where(n => goals.Contains(n.Key))
                    .Select(kvp => kvp.Value)
                    .Min();

                min = Math.Min(minDistanceForStart, min);

            }

            return min;
        }

        private static StateNode GetMinUnvisited(IDictionary<StateNode, int> distances, HashSet<string> unvisitedNodes)
        {
            var min = int.MaxValue / 2;
            StateNode closest = new StateNode(new Pose(Vector2D.Zero, Vector2D.Zero), 0);

            foreach (var (node, distance) in distances)
            {
                if (unvisitedNodes.Contains(node.GetRepresentativeString()) && distance <= min)
                {
                    min = distance;
                    closest = node;
                }
            }

            return closest!;
        }
    }
}
