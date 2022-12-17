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

            var caveGraph = BuildGraphOfRelevantCaves(caves, startingCave: "AA");
            var distances = caveGraph.GetMinDistances();

        }

        private static NodeGraph<string> BuildGraphOfRelevantCaves(IEnumerable<Cave> caves, string startingCave)
        {
            var baseGraph = new NodeGraph<string>(caves.Count());

            foreach (var cave in caves)
            {
                baseGraph.AddNode(cave.Id);

                foreach (var adjacentCaveId in cave.AdjacentCaves)
                {
                    baseGraph.AddEdge(cave.Id, adjacentCaveId);
                }
            }

            var baseDistances = baseGraph.GetMinDistances();

            var relevantCaves = caves.Where(c => c.FlowRate > 0 || c.Id == startingCave).ToList();

            var graph = new NodeGraph<string>(relevantCaves.Count);

            foreach (var relevantCave in relevantCaves)
            {
                foreach (var otherRelevantCave in relevantCaves)
                {
                    var distance = relevantCave == otherRelevantCave
                        ? 0
                        : baseDistances[(relevantCave.Id, otherRelevantCave.Id)];

                    graph.AddEdge(relevantCave.Id, otherRelevantCave.Id, distance);
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

            return caves;
        }
    }
}
