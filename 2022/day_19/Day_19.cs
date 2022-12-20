using aoc.common;
using aoc.utils;
using aoc.utils.extensions;
using System.Text;

namespace aoc.y2022.day_19
{
    // https://adventofcode.com/2022/day/19
    public class Day_19 : ISolver
    {
        private enum Resource
        {
            Ore,
            Clay,
            Obsidian,
            Geode
        }

        private class Inventory
        {
            public Dictionary<Resource, int> Resources { get; set; } = new()
            {
                { Resource.Ore, 0 },
                { Resource.Clay, 0 },
                { Resource.Obsidian, 0 },
                { Resource.Geode, 0 }
            };

            public Dictionary<Resource, int> Robots { get; set; } = new()
            {
                { Resource.Ore, 0 },
                { Resource.Clay, 0 },
                { Resource.Obsidian, 0 },
                { Resource.Geode, 0 }
            };

            public bool CanAfford(Cost cost)
            {
                foreach (var resource in cost.Resources)
                {
                    if (this.Resources[resource.Key] < resource.Value)
                    {
                        return false;
                    }
                }

                return true;
            }

            public void BuyRobot(Resource robotType, Cost cost)
            {
                foreach (var resource in cost.Resources)
                {
                    Resources[resource.Key] -= resource.Value;
                }

                Robots[robotType]++;
            }

            public void GetResources()
            {
                foreach (var robot in Robots)
                {
                    Resources[robot.Key] += robot.Value;
                }
            }

            public override string ToString()
            {
                var sb = new StringBuilder();

                foreach (var resource in Resources)
                {
                    sb.Append($"{resource.Key}: {resource.Value}, {resource.Key}Robots: {Robots[resource.Key]}, ");
                }

                sb.Remove(sb.Length - 2, 2);

                return sb.ToString();
            }

            public Inventory Clone()
            {
                var newInventory = new Inventory();

                foreach (var resource in Enum.GetValues<Resource>())
                {
                    newInventory.Resources[resource] = this.Resources[resource];
                    newInventory.Robots[resource] = this.Robots[resource];
                }

                return newInventory;
            }
        }

        private class Cost
        {
            public Dictionary<Resource, int> Resources { get; set; } = new();

            public Cost(int ore = 0, int clay = 0, int obsidian = 0)
            {
                Resources.Add(Resource.Ore, ore);
                Resources.Add(Resource.Clay, clay);
                Resources.Add(Resource.Obsidian, obsidian);
                Resources.Add(Resource.Geode, 0);
            }
        }

        private class BluePrint
        {
            public int Id { get; set; }
            public Dictionary<Resource, Cost> RobotCosts { get; set; } = new();
        }

        private readonly Dictionary<string, int> _resourceBreakEvenPointMemo = new();
        private readonly Dictionary<string, List<int>> _maxGeodesMemo = new();
        private readonly Stack<string> _stack = new();

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_19/input2.txt");
            var bluePrints = ParseInput(lines);

            var startingInventory = new Inventory();
            startingInventory.Robots[Resource.Ore] = 1;

            var scores = FindGeodeScores(bluePrints[0], startingInventory, 24);
            Console.WriteLine(scores.Max());

        }

        private List<int> FindGeodeScores(BluePrint bluePrint, Inventory inventory, int time)
        {
            var key = $"{time}-{inventory}";
            _stack.Push(key);

            if (_maxGeodesMemo.TryGetValue(key, out var result))
            {
                _stack.Pop();
                return result;
            }

            var scores = new List<int>();

            while (time-- > 0)
            {
                Console.WriteLine($"Time left: {time + 1}");
                Console.WriteLine(inventory.ToString());

                if (WaitingIsReasonableOption(inventory, bluePrint))
                {
                    var noBuyInventory = inventory.Clone();
                    noBuyInventory.GetResources();

                    scores.AddRange(FindGeodeScores(bluePrint, noBuyInventory, time));
                }

                foreach (var robot in bluePrint.RobotCosts)
                {
                    var resourceType = robot.Key;
                    var resourceBreakEvenPoint = ResourceBreakEvenPoint(resourceType, bluePrint);
                    var resourceRobotCurrentCount = inventory.Robots[resourceType];

                    if (inventory.CanAfford(robot.Value) &&
                        (resourceType == Resource.Geode || resourceBreakEvenPoint > resourceRobotCurrentCount))
                    {
                        Console.WriteLine($"Can afford to buy a {resourceType} robot, exploring banch");

                        var newInventory = inventory.Clone();
                        newInventory.BuyRobot(robot.Key, robot.Value);
                        inventory.GetResources();

                        scores.AddRange(FindGeodeScores(bluePrint, newInventory, time));
                    }
                }
            }

            scores.Add(inventory.Resources[Resource.Geode]);

            _stack.Pop();
            _maxGeodesMemo.Add(key, scores);
            return scores;
        }

        private int ResourceBreakEvenPoint(Resource resourceType, BluePrint blueprint)
        {
            var lookupKey = $"{resourceType}-{blueprint.Id}";

            if (_resourceBreakEvenPointMemo.TryGetValue(lookupKey, out var result))
            {
                return result;
            }

            var maxCost = blueprint.RobotCosts.Select(rc => rc.Value)
                                              .Select(c => c.Resources[resourceType])
                                              .Max();

            _resourceBreakEvenPointMemo.Add(lookupKey, maxCost);

            return maxCost;
        }

        private static bool WaitingIsReasonableOption(Inventory inventory, BluePrint blueprint)
        {
            // If there are any where you have n of resource where 1 < n < cost for any cost.
            var costs = blueprint.RobotCosts.Select(rc => rc.Value);

            if (costs.All(c => !inventory.CanAfford(c)))
            {
                return true;
            }

            foreach (var cost in costs)
            {
                if (cost.Resources.Any(rc => inventory.Resources[rc.Key] > 0 && inventory.Resources[rc.Key] < rc.Value))
                {
                    return true;
                }
            }

            return false;
        }

        private static IList<BluePrint> ParseInput(IList<string> lines)
        {
            var blueprints = lines.Select(l => l.ReadAllNumbers().ToList())
                                  .Select(b => new BluePrint
                                  {
                                      Id = b[0],
                                      RobotCosts = new Dictionary<Resource, Cost>
                                      {
                                          { Resource.Ore, new Cost(ore: b[1]) },
                                          { Resource.Clay, new Cost(ore: b[2]) },
                                          { Resource.Obsidian, new Cost(ore: b[3], clay: b[4]) },
                                          { Resource.Geode, new Cost(ore: b[5], obsidian: b[6]) }
                                      }
                                  }).ToList();

            return blueprints;
        }
    }
}
