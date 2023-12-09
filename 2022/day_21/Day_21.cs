using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_21
{
    // https://adventofcode.com/2022/day/21
    public class Day_21
    {
        private enum Operation
        {
            Add,
            Minus,
            Multiply,
            Divide,
            Shout,
        }

        private class Monkey
        {
            public string Name { get; set; } = string.Empty;
            public List<string> PredicateMonkeys { get; set; } = new List<string>();
            public long? Number { get; set; }
            public Operation Operation { get; set; } = Operation.Shout;

            public bool TryPerformAction(out long result, IDictionary<string, long> monkeyActions)
            {
                if (Operation == Operation.Shout)
                {
                    result = Number!.Value;

                    monkeyActions.TryAdd(Name, result);

                    return true;
                }

                if (PredicateMonkeys.All(pm => monkeyActions.ContainsKey(pm)))
                {
                    result = Operation switch
                    {
                        Operation.Add => monkeyActions[PredicateMonkeys[0]] + monkeyActions[PredicateMonkeys[1]],
                        Operation.Minus => monkeyActions[PredicateMonkeys[0]] - monkeyActions[PredicateMonkeys[1]],
                        Operation.Multiply => monkeyActions[PredicateMonkeys[0]] * monkeyActions[PredicateMonkeys[1]],
                        Operation.Divide => monkeyActions[PredicateMonkeys[0]] / monkeyActions[PredicateMonkeys[1]],
                        _ => throw new Exception("Unexpected monkey operation")
                    };

                    monkeyActions.TryAdd(Name, result);

                    return true;
                }
                else
                {
                    result = 0;
                    return false;
                }

            }
        }

        private const string Root = "root";
        private const string Human = "humn";

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_21/input.txt");
            var (numberMonkeys, operationMonkeys) = ParseInput(lines);

            var result = FindMonkeyNumber(Root, numberMonkeys, operationMonkeys);
            Console.WriteLine(result);

            var (numberMonkeys2, operationMonkeys2) = ParseInput(lines);
            var human = numberMonkeys2.Where(n => n.Name == Human);
            var root = operationMonkeys2.Where(n => n.Name == Root);
            var operationMonkeyMap = operationMonkeys2.ToDictionary(m => m.Name, m => m);

            var monkeyActions = new Dictionary<string, long>();

            foreach (var numMonkey in numberMonkeys2)
            {
                if (numMonkey.Name == Human)
                {
                    continue;
                }

                monkeyActions.Add(numMonkey.Name, numMonkey.Number!.Value);
            }

            var (rootMonkeyNumber, predicateMonkeyName) = FindMonkeyComputeableBranchValues(Root, monkeyActions, operationMonkeyMap);
            var predicateMonkey = operationMonkeys2.Single(m => m.Name == predicateMonkeyName);

            var humanNumber = FindNecessaryHumanNumber(rootMonkeyNumber,
                                                       predicateMonkey,
                                                       monkeyActions,
                                                       operationMonkeys2.ToDictionary(m => m.Name, m => m));

            Console.WriteLine(humanNumber);
        }

        private static long ComputeChildNeededValue(long neededValue,
                                                    long otherMonkeyScore,
                                                    Monkey monkey,
                                                    string unknownMonkeyName)
        {
            // We want the other monkey - the one where we know the value of it (or it's branch).
            // Then we can use that to compute the needed score of the unknown child, where the humn monkey is.
            var unknownMonkey = monkey.PredicateMonkeys.WithIndex().Single((mi) => mi.Item == unknownMonkeyName);
            var unknownMonkeyIndex = unknownMonkey.Index;

            switch (monkey.Operation)
            {
                case Operation.Add:
                    return neededValue - otherMonkeyScore;

                case Operation.Multiply:
                    return neededValue / otherMonkeyScore;

                case Operation.Minus:
                    // If (Needed = This - Other) => (This = Needed + Other). If (Needed = Other - This) => (This = Other - Needed)
                    return unknownMonkeyIndex == 0 ? neededValue + otherMonkeyScore : otherMonkeyScore - neededValue;

                case Operation.Divide:
                    // If (Needed = This / Other) => (This = Needed * Other). If (Needed = Other / This) => (This = Other / Needed)
                    return unknownMonkeyIndex == 0 ? otherMonkeyScore * neededValue : otherMonkeyScore / neededValue;

                default:
                    throw new Exception("Unexpcted operation");
            }
        }

        private static long FindNecessaryHumanNumber(long neededValue,
                                                     Monkey monkey,
                                                     IDictionary<string, long> monkeyActions,
                                                     IDictionary<string, Monkey> operationMonkeys)
        {
            var (knownChildScore, unknownChildName) = FindMonkeyComputeableBranchValues(monkey.Name,
                                                                                        monkeyActions,
                                                                                        operationMonkeys);

            var neededChildMonkeyValue = ComputeChildNeededValue(neededValue, knownChildScore, monkey, unknownChildName);

            if (unknownChildName == Human)
            {
                return neededChildMonkeyValue;
            }

            return FindNecessaryHumanNumber(neededChildMonkeyValue, operationMonkeys[unknownChildName], monkeyActions, operationMonkeys);
        }

        private static (long Number, string PredicateMonkey) FindMonkeyComputeableBranchValues(string monkeyName,
                                                                                               IDictionary<string, long> monkeyActions,
                                                                                               IDictionary<string, Monkey> operationMonkeys)
        {
            var root = operationMonkeys.Single(n => n.Value.Name == monkeyName).Value;

            long rootNum = default;
            string predicateMonkey = string.Empty;

            foreach (var monkey in root.PredicateMonkeys)
            {
                try
                {
                    rootNum = ComputeMonkeyNumber(monkeyActions, operationMonkeys, monkey);
                }
                catch (Exception)
                {
                    predicateMonkey = monkey;
                }
            }

            return (rootNum, predicateMonkey);
        }

        private static long FindMonkeyNumber(string monkeyName, IList<Monkey> numberMonkeys, IList<Monkey> operationMonkeys)
        {
            var totalMonkeyCount = numberMonkeys.Count + operationMonkeys.Count;

            var monkeyActions = new Dictionary<string, long>(totalMonkeyCount);

            foreach (var numMonkey in numberMonkeys)
            {
                monkeyActions.Add(numMonkey.Name, numMonkey.Number!.Value);
            }

            var operationMonkeysSet = operationMonkeys.ToDictionary(om => om.Name, om => om);

            var result = ComputeMonkeyNumber(monkeyActions, operationMonkeysSet, monkeyName);

            return result;
        }

        private static long ComputeMonkeyNumber(IDictionary<string, long> monkeyActions, IDictionary<string, Monkey> operationMonkeys, string monkeyName)
        {
            if (monkeyActions.TryGetValue(monkeyName, out var result))
            {
                return result;
            }

            var targetMonkey = operationMonkeys[monkeyName];

            if (targetMonkey.TryPerformAction(out var result2, monkeyActions))
            {
                return result2;
            }
            else
            {
                targetMonkey.PredicateMonkeys.ForEach(pm => ComputeMonkeyNumber(monkeyActions, operationMonkeys, pm));

                targetMonkey.TryPerformAction(out var computedResult, monkeyActions);

                return computedResult;
            }
        }

        private static (List<Monkey> NumberMonkeys, List<Monkey> ActionMonkeys) ParseInput(IList<string> lines)
        {
            const int numberMonkeyLineLengthMax = 12;

            var numberMonkeys = lines.Where(l => l.Length < numberMonkeyLineLengthMax)
                                     .Select(l => l.Split(':', ' ').ToList())
                                     .Select(m => new Monkey
                                     {
                                         Name = m[0],
                                         Number = int.Parse(m[2]),
                                         Operation = Operation.Shout
                                     })
                                     .ToList();

            var operationMonkeys = lines.Where(l => l.Length >= numberMonkeyLineLengthMax)
                                        .Select(l => l.Split(':', ' ').ToList())
                                        .Select(l => new Monkey
                                        {
                                            Name = l[0],
                                            PredicateMonkeys = new List<string>
                                            {
                                                l[2],
                                                l[4]
                                            },
                                            Operation = ParseOperation(l[3])
                                        })
                                        .ToList();

            return (numberMonkeys, operationMonkeys);
        }

        private static Operation ParseOperation(string s)
        {
            return s switch
            {
                "+" => Operation.Add,
                "-" => Operation.Minus,
                "*" => Operation.Multiply,
                "/" => Operation.Divide,
                _ => throw new Exception($"Unknown operation time: {s}")
            };
        }
    }
}
