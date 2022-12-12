using aoc.common;

namespace aoc.y2022.day_11
{
    // https://adventofcode.com/2022/day/11
    public class Day_11 : ISolver
    {
        private class Monkey
        {
            public long Id { get; set; }

            public long ItemsHandled { get; set; } = 0;

            public Queue<long> Items { get; set; } = new Queue<long>();

            public Func<long, long> MonkeyAction { get; set; } = l => l;

            public Func<long, int> PassTo { get; set; } = l => (int)l;
        }

        private static readonly long Modulo = 5 * 7 * 13 * 11 * 3 * 2 * 17 * 19;

        private static readonly List<Monkey> Monkeys = new()
        {
            new Monkey
            {
                Id = 0,
                Items = new Queue<long>(new List<long> { 52, 78, 79, 63, 51, 94 }),
                MonkeyAction = (long item) => 13 * item,
                PassTo = (long item) => item % 5 == 0 ? 1 : 6
            },
            new Monkey
            {
                Id = 1,
                Items = new Queue<long>(new List<long> { 77, 94, 70, 83, 53 }),
                MonkeyAction = (long item) => 3 + item,
                PassTo = (long item) => item % 7 == 0 ? 5 : 3
            },
            new Monkey
            {
                Id = 2,
                Items = new Queue<long>(new List<long> { 98, 50, 76 }),
                MonkeyAction = (long item) => item * item,
                PassTo = (long item) => item % 13 == 0 ? 0 : 6
            },
            new Monkey
            {
                Id = 3,
                Items = new Queue<long>(new List<long> { 92, 91, 61, 75, 99, 63, 84, 69 }),
                MonkeyAction = (long item) => 5 + item,
                PassTo = (long item) => item % 11 == 0 ? 5 : 7
            },
            new Monkey
            {
                Id = 4,
                Items = new Queue<long>(new List<long> { 51, 53, 83, 52 }),
                MonkeyAction = (long item) => 7 + item,
                PassTo = (long item) => item % 3 == 0 ? 2 : 0
            },
            new Monkey
            {
                Id = 5,
                Items = new Queue<long>(new List<long> { 76, 76 }),
                MonkeyAction = (long item) => 4 + item,
                PassTo = (long item) => item % 2 == 0 ? 4 : 7
            },
            new Monkey
            {
                Id = 6,
                Items = new Queue<long>(new List<long> { 75, 59, 93, 69, 76, 96, 65 }),
                MonkeyAction = (long item) => 19 * item,
                PassTo = (long item) => item % 17 == 0 ? 1 : 3
            },
            new Monkey
            {
                Id = 7,
                Items = new Queue<long>(new List<long> { 89 }),
                MonkeyAction = (long item) => 2 + item,
                PassTo = (long item) => item % 19 == 0 ? 2 : 4
            }
        };

        private static readonly Func<long, long> WorryModifier = (long item) => item / 3;

        public void Solve()
        {
            var monkeys = Monkeys;

            var numOfRounds = 10_000;
            while (numOfRounds-- > 0)
            {
                ExecuteMonkeyRound(monkeys, false);
            }

            var itemsInspected = Monkeys.Select(m => m.ItemsHandled).ToList();
            itemsInspected.Sort();
            var top2 = itemsInspected.TakeLast(2).ToList();
            
            Console.WriteLine(top2[0] * top2[1]);
        }

        private static void ExecuteMonkeyRound(IList<Monkey> monkeys, bool changeWorry)
        {
            foreach (var monkey in monkeys)
            {
                ExecuteMonkeyTurn(monkey, monkeys, changeWorry);
            }
        }

        private static void ExecuteMonkeyTurn(Monkey monkey, IList<Monkey> monkeys, bool changeWorry)
        {
            while (monkey.Items.Count > 0)
            {
                var item = monkey.Items.Dequeue();

                item = monkey.MonkeyAction(item);

                if (!changeWorry)
                {
                    item = item % Modulo;
                }
                else
                {
                    item = WorryModifier(item);
                }

                var targetMonkey = monkey.PassTo(item);

                monkeys[targetMonkey].Items.Enqueue(item);

                monkey.ItemsHandled++;
            }
        }
    }
}
