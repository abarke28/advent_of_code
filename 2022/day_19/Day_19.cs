using aoc.common;
using aoc.utils;
using aoc.utils.extensions;
using System.Text;

namespace aoc.y2022.day_19
{
    // https://adventofcode.com/2022/day/19
    public class Day_19
    {
        private enum Resource
        {
            Ore,
            Clay,
            Obsidian,
            Geode
        }

        private struct Inventory
        {
            public Inventory()
            {
            }

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

            public HashSet<Resource> GetAffordableRobots(BluePrint bluePrint)
            {
                var types = new HashSet<Resource>();

                foreach (var c in bluePrint.RobotCosts)
                {
                    if (CanAfford(c.Value))
                    {
                        types.Add(c.Key);
                    }
                }

                return types;
            }

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

            public Inventory NextWithBuyRobotAndGetResources(Resource robotType, Cost cost)
            {
                var inventory = this.Clone();

                foreach (var resource in cost.Resources)
                {
                    inventory.Resources[resource.Key] -= resource.Value;
                }

                foreach (var robot in inventory.Robots)
                {
                    inventory.Resources[robot.Key] += robot.Value;
                }

                inventory.Robots[robotType]++;

                return inventory;
            }

            public Inventory NextWithGetResources()
            {
                var inventory = this.Clone();

                foreach (var robot in inventory.Robots)
                {
                    inventory.Resources[robot.Key] += robot.Value;
                }

                return inventory;
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
        private readonly Dictionary<string, int> _maxGeodesMemo = new();

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_19/input.txt");
            var bluePrints = ParseInput(lines);

            var startingInventory = new Inventory();
            startingInventory.Robots[Resource.Ore] = 1;

            var scores = GetQualityLevels(bluePrints, 24);
            Console.WriteLine(scores.Sum());

            var bluePrints2 = bluePrints.Take(3).ToList();

            var result = 1;

            foreach (var bp in bluePrints2)
            {
                _maxGeodesMemo.Clear();
                _resourceBreakEvenPointMemo.Clear();

                var score = FindGeodeScore(bp, startingInventory, 32, null);
                Console.WriteLine(score);

                result *= score;
            }

            Console.WriteLine(result);
        }

        private int FindGeodeScore(BluePrint bluePrint, Inventory inventory, int time, HashSet<Resource>? cannotBuild = null)
        {
            var key = $"{time}-{inventory}";
            var maxScore = 0;

            if (_maxGeodesMemo.TryGetValue(key, out var result))
            {
                return result;
            }

            if (time <= 0)
            {
                _maxGeodesMemo.Add(key, inventory.Resources[Resource.Geode]);
                return inventory.Resources[Resource.Geode];
            }
            else if (time == 1)
            {
                _maxGeodesMemo.Add(key, inventory.Resources[Resource.Geode] + inventory.Robots[Resource.Geode]);
                return inventory.Resources[Resource.Geode] + inventory.Robots[Resource.Geode];
            }

            if (WaitingIsReasonableOption(inventory, bluePrint, time))
            {
                maxScore = Math.Max(maxScore, FindGeodeScore(bluePrint, inventory.NextWithGetResources(), time - 1, inventory.GetAffordableRobots(bluePrint)));
            }

            foreach (var robot in bluePrint.RobotCosts)
            {
                var resourceType = robot.Key;
                var resourceBreakEvenPoint = ResourceBreakEvenPoint(resourceType, bluePrint);
                var resourceRobotCurrentCount = inventory.Robots[resourceType];
                var robotIsWorthBuilding = resourceBreakEvenPoint > resourceRobotCurrentCount || resourceType == Resource.Geode;
                var typeIsAllowed = cannotBuild == null || !cannotBuild.Contains(resourceType);

                if (inventory.CanAfford(robot.Value) && robotIsWorthBuilding/* && typeIsAllowed*/)
                {
                    maxScore = Math.Max(maxScore, FindGeodeScore(bluePrint, inventory.NextWithBuyRobotAndGetResources(robot.Key, robot.Value), time - 1, null));
                }
            }

            _maxGeodesMemo.Add(key, maxScore);
            return maxScore;
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

        private static bool WaitingIsReasonableOption(Inventory inventory, BluePrint blueprint, int time)
        {
            if (time <= 2)
            {
                return false;
            }

            var costs = blueprint.RobotCosts.Select(rc => rc.Value);

            if (costs.All(c => !inventory.CanAfford(c)))
            {
                return true;
            }

            return false;
        }

        private IEnumerable<int> GetQualityLevels(IList<BluePrint> bluePrints, int time)
        {
            foreach (var bluePrint in bluePrints)
            {
                _maxGeodesMemo.Clear();
                _resourceBreakEvenPointMemo.Clear();

                var startingInventory = new Inventory();
                startingInventory.Robots[Resource.Ore] = 1;

                var score = FindGeodeScore(bluePrint, startingInventory, time);

                yield return score * bluePrint.Id;
            }
        }

        private static IList<BluePrint> ParseInput(IList<string> lines)
        {
            var blueprints = lines.Select(l => l.ReadAllNumbers<int>().ToList())
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
