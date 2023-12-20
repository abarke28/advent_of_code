using Aoc.Common;
using Aoc.Utils;

namespace Aoc.Y2023.Day_20
{
    // https://adventofcode.com/2023/day/20
    public class Day_20 : ISolver
    {
        private const char FlipFlop = '%';
        private const char Conjunction = '&';
        private const string Broadcaster = "broadcaster";

        private enum ModuleType
        {
            FlipFlop,
            Conjunction,
            Broadcaster,
            None
        };

        private class Pulse
        {
            public string SourceModule { get; set; }
            public string DestinationModule { get; set; }
            public bool High { get; set; }

            public Pulse(string sourceModule, string destinationModule, bool high)
            {
                SourceModule = sourceModule;
                DestinationModule = destinationModule;
                High = high;
            }
        }

        private class Module
        {
            public string Name { get; set; }
            public ModuleType Type { get; set; }
            public List<string> Connections { get; set; }
            public Dictionary<string, bool> InboundConnectionsState { get; set; } = new Dictionary<string, bool>(); 
            public bool State { get; set; }

            public Module(string name, ModuleType type, IEnumerable<string> connections)
            {
                Name = name;
                Type = type;
                Connections = new List<string>(connections);
            }

            public IEnumerable<Pulse> ProcessPulse(string sourceModule, bool high)
            {
                if (Type == ModuleType.FlipFlop)
                {
                    if (!high)
                    {
                        State = !State;
                        return Connections.Select(c => new Pulse(Name, c, State));
                    }
                    else
                    {
                        return Enumerable.Empty<Pulse>();
                    }
                }
                else if (Type == ModuleType.Conjunction)
                {
                    InboundConnectionsState[sourceModule] = high;

                    return Connections.Select(c => new Pulse(Name, c, !InboundConnectionsState.All(ics => ics.Value)));
                }
                else if (Type == ModuleType.Broadcaster)
                {
                    return Connections.Select(c => new Pulse(Name, c, high));
                }
                else
                {
                    // None - noop
                    if (!high)
                    {

                    }

                    return Enumerable.Empty<Pulse>();
                }
            }
        }

        public object Part1(IList<string> lines)
        {
            var modules = ParseLines(lines);

            var (low, high) = SimulateNButtonPresses(n: 1_000, modules);

            return 1L * low * high;
        }

        public object Part2(IList<string> lines)
        {
            var modules = ParseLines(lines);

            var outputModule = "rx";
            var outputModuleInput = modules.Single(m => m.Connections.Any(c => c.Equals(outputModule)));

            var modulesConnectedToOutputConjuctor = modules
                .Where(m => m.Connections.Any(c => c.Equals(outputModuleInput.Name)))
                .Select(m => m.Name)
                .ToHashSet();

            var cycleWatch = modulesConnectedToOutputConjuctor.ToDictionary(m => m, _ => new List<int>());

            SimulateNButtonPresses(n: 100_000, modules, cycleWatch);

            var cycles = new List<int>();

            foreach (var cycleList in cycleWatch)
            {
                cycles.Add(cycleList.Value[^1] - cycleList.Value[^2]);
            }

            return MathUtils.LCM<long>(cycles.Select(c => (long)c).ToArray());
        }

        private static (int LowPulses, int HighPulses) SimulateNButtonPresses(int n, ISet<Module> modulesSet, Dictionary<string, List<int>>? cycleWatch = null)
        {
            var modules = modulesSet.ToDictionary(m => m.Name, m => m);

            var lowPulseCount = 0;
            var highPulseCount = 0;

            var pulseQueue = new Queue<Pulse>();

            for (int i = 0; i < n; i++)
            {
                pulseQueue.Enqueue(new Pulse("button", Broadcaster, high: false));

                while (pulseQueue.Count > 0)
                {
                    var pulse = pulseQueue.Dequeue();
                    if (pulse.High)
                    {
                        highPulseCount++;
                    }
                    else
                    {
                        lowPulseCount++;
                    }

                    var nextPulses = modules[pulse.DestinationModule].ProcessPulse(pulse.SourceModule, pulse.High);

                    foreach (var nextPulse in nextPulses)
                    {
                        if (cycleWatch is not null)
                        {
                            if (nextPulse.High && cycleWatch.Keys.Contains(nextPulse.SourceModule))
                            {
                                cycleWatch[nextPulse.SourceModule].Add(i);
                            }
                        }

                        pulseQueue.Enqueue(nextPulse);
                    }
                }
            }

            return (lowPulseCount, highPulseCount);
        }

        private static HashSet<Module> ParseLines(IList<string> lines)
        {
            var modules = new HashSet<Module>(lines.Count);

            var broadcaster = lines.Where(l => l.Contains(Broadcaster)).Single();
            var flipFlops = lines.Where(l => l.Contains(FlipFlop));
            var conjunctors = lines.Where(l => l.Contains(Conjunction));

            var broadcastConnections = broadcaster.Split("->", StringSplitOptions.TrimEntries)[1].Split(",", StringSplitOptions.TrimEntries);
            var bc = new Module(Broadcaster, ModuleType.Broadcaster, broadcastConnections);

            modules.Add(bc);

            foreach (var flipFlop in flipFlops)
            {
                var name = flipFlop.Split("->", StringSplitOptions.TrimEntries)[0]!.Substring(1);
                var ffConnections = flipFlop.Split("->", StringSplitOptions.TrimEntries)[1].Split(",", StringSplitOptions.TrimEntries);
                var ff = new Module(name, ModuleType.FlipFlop, ffConnections);

                modules.Add(ff);
            }

            foreach (var conjuction in conjunctors)
            {
                var name = conjuction.Split("->", StringSplitOptions.TrimEntries)[0]!.Substring(1);
                var cConnections = conjuction.Split("->", StringSplitOptions.TrimEntries)[1].Split(",", StringSplitOptions.TrimEntries);
                var c = new Module(name, ModuleType.Conjunction, cConnections);

                modules.Add(c);
            }

            foreach (var conjuctorModule in modules.Where(m => m.Type == ModuleType.Conjunction))
            {
                var inboundConnections = modules.Where(m => m.Connections.Any(c => c == conjuctorModule.Name));

                conjuctorModule.InboundConnectionsState = inboundConnections.ToDictionary(c => c.Name, _ => false);
            }

            var unknownModuleNames = modules.SelectMany(m => m.Connections).Where(um => !modules.Select(m => m.Name).ToHashSet().Contains(um)).ToList();

            foreach (var unknownModuleName in unknownModuleNames)
            {
                modules.Add(new Module(unknownModuleName, ModuleType.None, Enumerable.Empty<string>()));
            }

            return modules;
        }
    }
}
