using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_25
{
    // https://adventofcode.com/2023/day/25
    public class Day_25 : ISolver
    {
        public object Part1(IList<string> lines)
        {
            var graph = ParseLines(lines);

            var edgesToSever = FindMinCutEdges(graph);

            foreach (var edgeToSever in edgesToSever)
            {
                graph.RemoveEdge(edgeToSever.Node1, edgeToSever.Node2, directed: false);
            }

            var score = GetGraphScore(edgesToSever.First().Node1, edgesToSever.First().Node2, graph);

            return score;
        }

        public object Part2(IList<string> lines)
        {
            return 0;
        }

        private static int GetGraphScore(string node1, string node2, NodeGraph<string> graph)
        {
            var score = 1;

            foreach (var start in new string[] { node1, node2 })
            {
                var visited = new HashSet<string>();

                var toVisit = new Queue<string>();
                toVisit.Enqueue(start);

                while (toVisit.Count > 0)
                {
                    var current = toVisit.Dequeue();
                    visited.Add(current);

                    var adjacentNodes = graph.GetAdjacentNodes(current);

                    foreach (var adjacentNode in adjacentNodes.Where(an => !visited.Contains(an)))
                    {
                        toVisit.Enqueue(adjacentNode);
                    }
                }

                score *= visited.Count;
            }

            return score;
        }

        private static IEnumerable<NodeGraphEdge<string>> FindMinCutEdges(NodeGraph<string> graph)
        {
            var allNodes = graph.GetAllNodes().ToList();
            var allEdges = graph.GetAllEdges().Distinct();

            var allEdgesFrequency = allEdges.ToDictionary(e => e, _ => 0);

            var rnd = new Random();

            var numPairs = 1000;
            var pairsToCheck = new List<(string Node1, string Node2)>(numPairs);

            while (numPairs-- > 0)
            {
                var n1 = allNodes[rnd.Next(allNodes.Count)];
                var n2 = allNodes[rnd.Next(allNodes.Count)];

                pairsToCheck.Add((n1, n2));
            }

            foreach (var pair in pairsToCheck)
            {
                var path = graph.GetShortestPath(pair.Node1, pair.Node2).ToList();

                var edges = graph.GetPathEdges(path);

                foreach (var edge in edges)
                {
                    allEdgesFrequency[edge]++;
                }
            }

            var edgesToCut = allEdgesFrequency.OrderByDescending(kvp => kvp.Value).Take(3).Select(s => s.Key);

            return edgesToCut;
        }

        private static IEnumerable<NodeGraphEdge<string>> FindNodesToDisjointGraph(NodeGraph<string> graph)
        {
            var nodes = graph.GetAllNodes();
            var edges = graph.GetAllEdges();
            var distinctEdges = edges.Distinct();

            var currentCluster = new HashSet<string>();
            var currentNode = nodes.First();
            currentCluster.Add(currentNode);

            var currentEdgesLeavingCluster = new List<NodeGraphEdge<string>>(graph.GetNodeEdges(currentNode));

            while (currentEdgesLeavingCluster.Count != 3)
            {
                var nextNodesToProcess = new List<NodeGraphEdge<string>>(currentEdgesLeavingCluster.Count);

                //var nodesThatCanAdded

                
            }


            return new List<NodeGraphEdge<string>>();

        }

        private static NodeGraph<string> ParseLines(IList<string> lines)
        {
            var allNodes = new HashSet<string>();
            var nodeConnections = new Dictionary<string, List<string>>();

            foreach (var line in lines)
            {
                var nodes = line.Split(':', ' ');
                var parent = nodes[0];
                var children = nodes[1..].Where(c => !string.IsNullOrWhiteSpace(c));

                allNodes.Add(parent);
                allNodes.AddRange(children);

                if (nodeConnections.ContainsKey(parent))
                {
                    nodeConnections[parent].AddRange(children);
                }
                else
                {
                    nodeConnections.Add(parent, children.ToList());
                }
            }

            var graph = new NodeGraph<string>(allNodes.Count);

            foreach (var (parent, children) in nodeConnections)
            {
                graph.AddNode(parent);

                foreach (var child in children)
                {
                    graph.AddNode(child);
                    graph.AddEdge(parent, child, directed: false);
                }
            }

            return graph;
        }
    }
}
