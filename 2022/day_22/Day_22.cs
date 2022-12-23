using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_22
{
    // https://adventofcode.com/2022/day/22
    public class Day_22 : ISolver
    {
        private const char Valid = '.';
        private const char InValid = ' ';
        private const char Wall = '#';

        private const int RowScore = 1000;
        private const int ColScore = 4;

        private enum Turn
        {
            Left,
            Right,
            None
        }

        private class Instruction
        {
            public Turn Direction { get; set; }
            public int Count { get; set; }
        }

        private static readonly Dictionary<Vector2D, int> _headingScoreMap = new()
        {
            { Vector2D.Right, 0 },
            { Vector2D.Up, 1 },
            { Vector2D.Left, 2 },
            { Vector2D.Down, 3 }
        };

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_22/input.txt");

            var (grid, instructions) = ParseInput(lines);
            grid.PrintGrid();

            var startingPosition = grid.GetRow(0).WithIndex().First(rwi => rwi.item == Valid).index;
            var startingVector = new Vector2D(startingPosition, 0);
            var startingHeading = Vector2D.Right;

            var (finalPosition, finalHeading) = ExecuteInstructions(grid, instructions, startingVector, startingHeading);

            var score = CalculateScore(finalPosition, finalHeading);
            Console.WriteLine(score);
        }

        private static (Vector2D finalPosition, Vector2D finalHeading) ExecuteInstructions(Grid<char> grid, IEnumerable<Instruction> instructions, Vector2D startingPosition, Vector2D startingHeading)
        {
            var currentHeading = startingHeading;
            var currentPosition = startingPosition;

            foreach (var instruction in instructions)
            {
                (currentPosition, currentHeading) = ExecuteInstruction(grid, instruction, currentPosition, currentHeading);
                Console.WriteLine($"Now at {currentPosition}, with heading {currentHeading}");
            }

            return (currentPosition, currentHeading);
        }

        private static (Vector2D newPosition, Vector2D newHeading) ExecuteInstruction(Grid<char> grid, Instruction instruction, Vector2D currentPosition, Vector2D currentHeading)
        {
            Console.WriteLine($"Instruction: Move: {instruction.Count}, Turn: {instruction.Direction}");

            if (instruction.Direction != Turn.None)
            {
                var adjustedHeading = CalculateTurn(currentHeading, instruction.Direction);
                Console.WriteLine($"Heading adjusted to {adjustedHeading}, still at {currentPosition}");

                return (currentPosition, adjustedHeading);
            }

            Vector2D newPosition = currentPosition;
            Vector2D newHeading = currentHeading;

            while (instruction.Count > 0)
            {
                var nextTarget = currentPosition + currentHeading;
                var nextHeading = currentHeading;
           
                DirectionEval:
                
                var isInBounds = grid.IsInBounds(nextTarget);

                Console.WriteLine($"At {currentPosition} with heading {currentHeading}. Next target {nextTarget}");

                if (isInBounds && grid.GetValue(nextTarget) == Valid)
                {
                    currentPosition = nextTarget;
                    currentHeading = nextHeading;
                    newPosition = currentPosition;
                    instruction.Count--;
                    continue;
                }
                else if (isInBounds && grid.GetValue(nextTarget) == Wall)
                {
                    break;
                }
                else if (!isInBounds || grid.GetValue(nextTarget) == InValid)
                {
                    var (canMove, wrappedTarget, wrappedHeading) = ComputeWrap(grid, currentPosition, currentHeading);

                    if (!canMove)
                    {
                        break;
                    }
                    else
                    {
                        nextTarget = wrappedTarget;
                        nextHeading = wrappedHeading;

                        goto DirectionEval;
                    }
                }

                throw new Exception($"Unexpected next target position value: {grid.GetValue(nextTarget)}");
            }

            return (newPosition, newHeading);
        }

        private static (bool canMove, Vector2D wrappedPosition, Vector2D wrappedHeading) ComputeWrap(Grid<char> grid, Vector2D currentPosition, Vector2D currentHeading)
        {
            // Heading left or right
            if (currentHeading.Y == 0)
            {
                var indexedRow = grid.GetRow(currentPosition.Y).WithIndex();

                var nextXCoord = currentHeading ==
                    Vector2D.Right
                        ? indexedRow.Where(ir => ir.item != InValid).Min(ir => ir.index)
                        : indexedRow.Where(ir => ir.item != InValid).Max(ir => ir.index);

                var nextTarget = new Vector2D(nextXCoord, currentPosition.Y);

                if (!grid.IsInBounds(nextTarget) || grid.GetValue(nextTarget) == Wall)
                {
                    return (false, nextTarget, currentHeading);
                }

                return (true, nextTarget, currentHeading);
            }
            // Heading up or down
            else
            {
                var indexedColumn = grid.GetColumn(currentPosition.X).WithIndex();

                var nextYCoord = currentHeading ==
                    Vector2D.Up
                        ? indexedColumn.Where(ic => ic.item != InValid).Min(ic => ic.index)
                        : indexedColumn.Where(ic => ic.item != InValid).Max(ic => ic.index);

                var nextTarget = new Vector2D(currentPosition.X, nextYCoord);

                if (!grid.IsInBounds(nextTarget) || grid.GetValue(nextTarget) == Wall)
                {
                    return (false, nextTarget, currentHeading);
                }

                return (true, nextTarget, currentHeading);
            }
        }

        private static Vector2D CalculateTurn(Vector2D currentHeading, Turn direction)
        {
            if (direction == Turn.Left)
            {
                var newHeading = currentHeading.Transpose() * (currentHeading.Y == 0 ? -1 : 1);

                return newHeading;
            }
            else if (direction == Turn.Right)
            {
                var newHeading = currentHeading.Transpose() * (currentHeading.Y != 0 ? -1 : 1);

                return newHeading;
            }

            throw new Exception($"Unexpeced turn direction: {direction}");
        }

        private static int CalculateScore(Vector2D position, Vector2D heading)
        {
            var score = _headingScoreMap[heading] + ((position.X + 1) * ColScore) + ((position.Y + 1) * RowScore);

            return score;
        }

        private static IDictionary<(Vector2D position, Vector2D heading), (Vector2D position, Vector2D heading)> Compute3DWraps(Grid<char> grid)
        {
            var wraps = new Dictionary<(Vector2D position, Vector2D heading), (Vector2D position, Vector2D heading)>();

            // TODO - Compute wraps.

            return wraps;
        }

        private static (Grid<char> grid, IList<Instruction> instructions) ParseInput(IList<string> lines)
        {
            var gridLines = lines.Take(lines.Count - 2);

            var gridLinesMaxLength = gridLines.Max(l => l.Length);
            var paddedGridLines = gridLines.Select(l => l.PadRight(gridLinesMaxLength))
                                           .ToList();

            var grid = Grid<char>.FromStrings(paddedGridLines, c => c);

            var instructionsString = lines.TakeLast(1).Single();

            var instructions = new List<Instruction>();

            for (var i = 0; i < instructionsString.Length;)
            {
                if (instructionsString[i] == 'L')
                {
                    instructions.Add(new Instruction
                    {
                        Direction = Turn.Left
                    });
                    i++;

                    continue;
                }
                else if (instructionsString[i] == 'R')
                {
                    instructions.Add(new Instruction
                    {
                        Direction = Turn.Right
                    });
                    i++;

                    continue;
                }

                var numChars = instructionsString.Skip(i).TakeWhile(c => c != 'R' && c != 'L');
                var num = int.Parse(new string(numChars.ToArray()));

                instructions.Add(new Instruction { Count = num, Direction = Turn.None });

                i += numChars.Count();
            }

            return (grid, instructions);
        }
    }
}
