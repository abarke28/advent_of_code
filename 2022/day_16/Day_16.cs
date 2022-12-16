using aoc.common;
using aoc.utils;
using aoc.utils.extensions;
using System.Collections.Generic;

namespace aoc.y2022.day_16
{
    // https://adventofcode.com/2022/day/16
    public class Day_16 : ISolver
    {
        private const int StartingTime = 30;
        private const int ActionTimeCost = 1;

        private class Cave
        {
            public int Index { get; set; }
            public string Id { get; set; } = string.Empty;
            public bool ValveOpen { get; set; }
            public int FlowRate { get; set; }
            public List<string> AdjacentCaves { get; set; } = new List<string>();
        }


        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_16/input.txt");

            var caves = ParseCaves(lines).ToList();
            var cavesWithFlow = caves.Where(c => c.FlowRate > 0);
            var cavesWithFlowIndices = cavesWithFlow.Select(cwf => cwf.Index).ToList();

            var caveIdIndexMap = caves.ToDictionary(c => c.Id, c => c.Index);

            var startingCave = caves.Where(c => c.Id == "AA").Single().Index;

            var graph = BuildGraph(caves, caveIdIndexMap);
            var relevantNodes = cavesWithFlow.Append(caves[startingCave]);

            var minDistancesForFlowAndStartingNodes = GetMinDistancesMap(graph, relevantNodes.Select(cvf => cvf.Index).ToList());

            var flowGraph = BuildFlowGraph(relevantNodes, minDistancesForFlowAndStartingNodes);

        }

        private static int CalculateCaveValueScore(int currentPosition,
                                                   int desiredPosition,
                                                   int timeLeft,
                                                   Graph graph,
                                                   Dictionary<int, Cave> indexedCaves,
                                                   HashSet<int> caveValvesOpened)
        {
            var candidateCave = indexedCaves[desiredPosition];
            var candidateCaveFlow = candidateCave.FlowRate;

            if (candidateCaveFlow == 0 || caveValvesOpened.Contains(desiredPosition))
            {
                return 0;
            }

            var distance = graph.GetMinDistances(currentPosition)[desiredPosition];

            var score = (timeLeft - distance) * candidateCaveFlow;

            return score;
        }

        private static Graph BuildFlowGraph(IEnumerable<Cave> cavesWithFlowAndStart, Dictionary<(int, int), int> minDistancesMap)
        {
            var graph = new Graph(cavesWithFlowAndStart.Count());

            var cavesWithFlowAndStartWithIndex = cavesWithFlowAndStart.WithIndex();

            foreach (var (cave, index) in cavesWithFlowAndStartWithIndex)
            {
                var adjacentCaves = cave.AdjacentCaves;

                foreach (var adjacentCave in adjacentCaves)
                {
                    // TODO
                    var adjacentCaveIndex = minDistancesMap[(1,1)];

                    graph.AddEdge(cave.Index, adjacentCaveIndex, 1);
                }
            }

            return graph;
        }

        private static IEnumerable<IEnumerable<int>> GetPermutations(IList<int> list, int length)
        {
            if (length == 1) return list.Select(t => new int[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new int[] { t2 }));
        }

        private static Dictionary<(int, int), int> GetMinDistancesMap(Graph graph, IList<int> nodesWithFlow)
        {
            var map = new Dictionary<(int, int), int>();

            for (var i = 0; i < nodesWithFlow.Count; i++)
            {
                var djikstras = graph.GetMinDistances(nodesWithFlow[i]);

                for (var j = 0; j < nodesWithFlow.Count; j++)
                {
                    if (i != j)
                    {
                        map.Add((nodesWithFlow[i], nodesWithFlow[j]), djikstras[nodesWithFlow[j]]);
                    }
                }
            }

            return map;
        }

        private static Graph BuildGraph(IEnumerable<Cave> caves, Dictionary<string, int> caveIdIndexMap)
        {
            var graph = new Graph(caves.Count());

            foreach (var cave in caves)
            {
                var adjacentCaves = cave.AdjacentCaves;

                foreach (var adjacentCave in adjacentCaves)
                {
                    var adjacentCaveIndex = caveIdIndexMap[adjacentCave];

                    graph.AddEdge(cave.Index, adjacentCaveIndex, 1);
                }
            }

            return graph;
        }

        private static IEnumerable<Cave> ParseCaves(IEnumerable<string> lines)
        {
            var words = lines.Select(l => l.Split(' ', '=', ';', ','))
                             .Select(ws => ws.Select(w => w.Trim()).ToList());

            var caves = words.Select(ws => new Cave
            {
                Id = ws[1],
                ValveOpen = false,
                FlowRate = int.Parse(ws[5]),
                AdjacentCaves = new List<string>(ws.GetRange(11, ws.Count - 11).Where(w => !string.IsNullOrEmpty(w)))
            }).ToList();

            var cavesWithIndex = caves.WithIndex().ToList();

            cavesWithIndex.ForEach(ci => ci.item.Index = ci.index);

            return cavesWithIndex.Select(ci => ci.item);
        }
    }
}
