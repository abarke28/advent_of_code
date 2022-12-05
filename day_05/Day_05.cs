using aoc.utils;

namespace aoc.day_05
{
    // https://adventofcode.com/2022/day/5
    public class Day_05
    {
        private static List<Stack<char>> CrateStacks = new List<Stack<char>>
        {
            new Stack<char>( new List<char> { 'F', 'H', 'B', 'V', 'R', 'Q', 'D', 'P' }),
            new Stack<char>( new List<char> { 'L', 'D', 'Z', 'Q', 'W', 'V' }),
            new Stack<char>( new List<char> { 'H', 'L', 'Z', 'Q', 'G', 'R', 'P', 'C' }),
            new Stack<char>( new List<char> { 'R', 'D', 'H', 'F', 'J', 'V', 'B' }),
            new Stack<char>( new List<char> { 'Z', 'W', 'L', 'C' }),
            new Stack<char>( new List<char> { 'J', 'R', 'P', 'N', 'T', 'G', 'V', 'M' }),
            new Stack<char>( new List<char> { 'J', 'R', 'L', 'V', 'M', 'B', 'S' }),
            new Stack<char>( new List<char> { 'D', 'P', 'J' }),
            new Stack<char>( new List<char> { 'D', 'C', 'N', 'W', 'V' })
        };

        public static void GetResult()
        {
            var lines = FileUtils.ReadAllLines("day_05/input_2.txt");

            foreach(var line in lines)
            {
                PerformLineOperation(line);
            }

            foreach(var stack in CrateStacks)
            {
                Console.Write(stack.Peek());
            }
        }

        private static void PerformLineOperation(string l)
        {
            var words = l.Split(' ');

            const int moveIndex = 1;
            const int sourceIndex = 3;
            const int destinationIndex = 5;

            var moveCount = int.Parse(words[moveIndex]);
            var sourceStack = int.Parse(words[sourceIndex]) - 1;
            var destinationStack = int.Parse(words[destinationIndex]) - 1;

            //MoveCratesV1(moveCount, sourceStack, destinationStack);
            MoveCratesV2(moveCount, sourceStack, destinationStack);
        }

        private static void MoveCratesV1(int moveCount, int sourceStack, int destinationStack)
        {
            while (moveCount > 0)
            {
                var crate = CrateStacks[sourceStack].Pop();

                CrateStacks[destinationStack].Push(crate);

                moveCount--;
            }
        }

        private static void MoveCratesV2(int moveCount, int sourceStack, int destinationStack)
        {
            var movingCrates = new List<char>(moveCount);
            
            while (moveCount > 0)
            {
                var crate = CrateStacks[sourceStack].Pop();

                movingCrates.Add(crate);

                moveCount--;
            }

            movingCrates.Reverse();

            foreach (var crate in movingCrates)
            {
                CrateStacks[destinationStack].Push(crate);
            }
        }
    }
}
