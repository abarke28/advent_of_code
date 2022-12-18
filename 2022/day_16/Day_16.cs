using aoc.common;
using aoc.utils;
using System.Collections;

namespace aoc.y2022.day_16
{
    // https://adventofcode.com/2022/day/16
    public class Day_16 : ISolver
    {
        private const int ValveTurnTime = 1;

        private class Cave
        {
            public string Id { get; set; } = string.Empty;
            public int FlowRate { get; set; }
            public List<string> AdjacentCaves { get; set; } = new List<string>();
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_16/input.txt");

            var startingCave = "AA";

            var caves = ParseCaves(lines).ToList();
            var caveIdToFlowMap = caves.ToDictionary(c => c.Id, c => c.FlowRate);
            var cavesWithFlow = caves.Where(c => c.FlowRate > 0);

            var caveGraph = BuildGraphOfRelevantCaves(caves, startingCave);
            var distances = caveGraph.GetMinDistances();

            var part1Time = 30;
            var allPaths = FindAllPaths(distances, startingCave, cavesWithFlow.Select(c => c.Id).ToHashSet(), new List<string>(), part1Time);
            var allScores = allPaths.Select(p => ComputePathPressureScore(p, startingCave, distances, caveIdToFlowMap, part1Time));

            Console.WriteLine(allScores.Max());

            var part2Time = 26;
            var allPaths2 = FindAllPaths(distances, startingCave, cavesWithFlow.Select(c => c.Id).ToHashSet(), new List<string>(), part2Time);
            var score = FindBestScoreForTwo(allPaths2, startingCave, distances, caveIdToFlowMap, part2Time);

            Console.WriteLine(score);
        }

        private static int ComputePathPressureScore(IList<string> path,
                                                    string startingCave,
                                                    IDictionary<(string, string), int> distances,
                                                    IDictionary<string, int> flowRates,
                                                    int time)
        {
            var score = 0;
            var current = startingCave;

            foreach (var cave in path)
            {
                time -= distances[(current, cave)] + ValveTurnTime;

                score += flowRates[cave] * time;

                current = cave;
            }

            return score;
        }

        private static int FindBestScoreForTwo(IList<IList<string>> paths,
                                               string startingCave,
                                               IDictionary<(string, string), int> distances,
                                               IDictionary<string, int> flows, int time)
        {
            var pathsWithScores = paths.Select((path, index) => (Score: ComputePathPressureScore(path, startingCave, distances, flows, time),
                                                                 Valves: new HashSet<string>(path),
                                                                 Index: index)).ToList();

            pathsWithScores.Sort((l, r) => r.Score.CompareTo(l.Score));

            var combinedScore = 0;

            foreach (var (score, valves, index) in pathsWithScores)
            {
                foreach ((int score2, HashSet<string> valves2, int index2) in pathsWithScores)
                {
                    if (index % 100 == 0)
                    {
                        Console.WriteLine($"Computing path {index} & {index2}");
                    }

                    if (score + score2 < combinedScore)
                    {
                        continue;
                    }

                    if (!valves.Intersect(valves2).Any())
                    {
                        combinedScore = Math.Max(combinedScore, (score + score2));
                    }
                }
            }

            return combinedScore;
        }

        private static IList<IList<string>> FindAllPaths(IDictionary<(string, string), int> distances,
                                                         string startingCave,
                                                         ISet<string> unvisited,
                                                         IList<string> visited,
                                                         int time)
        {
            var paths = new List<IList<string>>();

            foreach (var nextCave in unvisited)
            {
                var cost = distances[(startingCave, nextCave)] + ValveTurnTime;

                if (cost < time)
                {
                    var newUnvisited = new HashSet<string>(unvisited);
                    newUnvisited.Remove(nextCave);

                    var newVisited = new List<string>(visited)
                    {
                        nextCave
                    };

                    var newTime = time - cost;

                    paths.AddRange(FindAllPaths(distances, nextCave, newUnvisited, newVisited, newTime));
                }
            }

            paths.Add(visited);

            return paths;
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
                FlowRate = int.Parse(ws[5]),
                AdjacentCaves = new List<string>(ws.GetRange(11, ws.Count - 11).Where(w => !string.IsNullOrEmpty(w)))
            }).ToList();

            return caves;
        }
    }
}
