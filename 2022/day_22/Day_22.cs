using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_22
{
    // https://adventofcode.com/2022/day/22
    public class Day_22
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

        private static readonly Vector2D Up = Vector2D.Down;
        private static readonly Vector2D Down = Vector2D.Up;
        private static readonly Vector2D Left = Vector2D.Left;
        private static readonly Vector2D Right = Vector2D.Right;

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_22/input.txt");

            var (grid, instructions) = ParseInput(lines);

            var startingPosition = grid.GetRow(0).WithIndex().First(rwi => rwi.Item == Valid).Index;
            var startingVector = new Vector2D(startingPosition, 0);
            var startingHeading = Vector2D.Right;

            var (finalPosition, finalHeading) = ExecuteInstructions(grid, instructions, startingVector, startingHeading, wrap3D: false);

            var score = CalculateScore(finalPosition, finalHeading);
            Console.WriteLine($"Final position: {finalPosition}, final heading: {finalHeading}, score: {score}");

            var (_, instructions3D) = ParseInput(lines);
            var (finalPosition3D, finalHeading3D) = ExecuteInstructions(grid, instructions3D, startingVector, startingHeading, wrap3D: true);

            var score3D = CalculateScore(finalPosition3D, finalHeading3D);
            Console.WriteLine($"Final position: {finalPosition3D}, final heading: {finalHeading3D}, score: {score3D}");
        }

        private static (Vector2D finalPosition, Vector2D finalHeading) ExecuteInstructions(
            Grid<char> grid,
            IEnumerable<Instruction> instructions, 
            Vector2D startingPosition, 
            Vector2D startingHeading,
            bool wrap3D)
        {
            var currentHeading = startingHeading;
            var currentPosition = startingPosition;

            foreach (var instruction in instructions)
            {
                (currentPosition, currentHeading) = ExecuteInstruction(grid, instruction, currentPosition, currentHeading, wrap3D);
            }

            return (currentPosition, currentHeading);
        }

        private static (Vector2D newPosition, Vector2D newHeading) ExecuteInstruction(
            Grid<char> grid,
            Instruction instruction,
            Vector2D currentPosition,
            Vector2D currentHeading,
            bool wrap3D)
        {
            if (instruction.Direction != Turn.None)
            {
                var adjustedHeading = CalculateTurn(currentHeading, instruction.Direction);

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

                if (isInBounds && grid.GetValue(nextTarget) == Valid)
                {
                    currentPosition = nextTarget;
                    currentHeading = nextHeading;
                    newPosition = currentPosition;
                    newHeading = nextHeading;

                    instruction.Count--;
                    continue;
                }
                else if (isInBounds && grid.GetValue(nextTarget) == Wall)
                {
                    break;
                }
                else if (!isInBounds || grid.GetValue(nextTarget) == InValid)
                {
                    var (canMove, wrappedTarget, wrappedHeading) = 
                        wrap3D ? ComputeWrap3D(grid, currentPosition, currentHeading) : ComputeWrap2D(grid, currentPosition, currentHeading);

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

        private static (bool canMove, Vector2D wrappedPosition, Vector2D wrappedHeading) ComputeWrap3D(Grid<char> grid, Vector2D currentPosition, Vector2D currentHeading)
        {
            /*
                 ..0 .50 100 150
             ..0 ... .1. .2. ...
             .50 ... .3. ... ...
             100 .4. .5. ... ...
             150 .6. ... ... ...
             200 ... ... ... ...
                                  */

            var (x, y) = currentPosition;

            var wrappedPosition = Vector2D.Zero;
            var wrappedHeading = Vector2D.Zero;

            // 1 > 6
            if (x is >= 50 and <= 99 && y is 0 && currentHeading == Up)
            {
                wrappedPosition = new Vector2D(0, x + 100);
                wrappedHeading = Right;
            }

            // 6 > 1
            if (x is 0 && y is >= 150 and <= 199 && currentHeading == Left)
            {
                wrappedPosition = new Vector2D(y - 100, 0);
                wrappedHeading = Down;
            }

            // 1 > 4
            if (x is 50 && y is >= 0 and <= 49 && currentHeading == Left)
            {
                wrappedPosition = new Vector2D(0, 149 - y);
                wrappedHeading = Right;
            }

            // 4 > 1
            if (x is 0 && y is >= 100 and <= 149 && currentHeading == Left)
            {
                wrappedPosition = new Vector2D(50, 149 - y);
                wrappedHeading = Right;
            }

            // 3 > 4
            if (x is 50 && y is >= 50 and <= 99 && currentHeading == Left)
            {
                wrappedPosition = new Vector2D(y - 50, 100);
                wrappedHeading = Down;
            }

            // 4 > 3
            if (x is >= 0 and <= 49 && y is 100 && currentHeading == Up)
            {
                wrappedPosition = new Vector2D(50, x + 50);
                wrappedHeading = Right;
            }

            // 2 > 3
            if (x is >= 100 and <= 149 && y == 49 && currentHeading == Down)
            {
                wrappedPosition = new Vector2D(99, x - 50);
                wrappedHeading = Left;
            }

            // 3 > 2
            if (x is 99 && y is >= 50 and <= 99 && currentHeading == Right)
            {
                wrappedPosition = new Vector2D(y + 50, 49);
                wrappedHeading = Up;
            }

            // 5 > 6
            if (x is >= 50 and <= 99 && y is 149 && currentHeading == Down)
            {
                wrappedPosition = new Vector2D(49, x + 100);
                wrappedHeading = Left;
            }

            // 6 > 5
            if (x is 49 && y is >= 150 and <= 199 && currentHeading == Right)
            {
                wrappedPosition = new Vector2D(y - 100, 149);
                wrappedHeading = Up;
            }

            // 2 > 6
            if (x is >= 100 and <= 149 && y is 0 && currentHeading == Up)
            {
                wrappedPosition = new Vector2D(x - 100, 199);
                wrappedHeading = Up;
            }

            // 6 > 2
            if (x is >= 0 and <= 49 && y is 199 && currentHeading == Down)
            {
                wrappedPosition = new Vector2D(x + 100, 0);
                wrappedHeading = Down;
            }

            // 2 > 5
            if (x is 149 && y is >= 0 and <= 49 && currentHeading == Right)
            {
                wrappedPosition = new Vector2D(99, 149 - y);
                wrappedHeading = Left;
            }

            // 5 > 2
            if (x is 99 && y is >= 100 and <= 149 && currentHeading == Right)
            {
                wrappedPosition = new Vector2D(149, 149 - y);
                wrappedHeading = Left;
            }

            if (!grid.IsInBounds(wrappedPosition) || grid.GetValue(wrappedPosition) == Wall)
                {
                    return (false, wrappedPosition, wrappedHeading);
                }

            return (true, wrappedPosition, wrappedHeading);
        }

        private static (bool canMove, Vector2D wrappedPosition, Vector2D wrappedHeading) ComputeWrap2D(Grid<char> grid, Vector2D currentPosition, Vector2D currentHeading)
        {
            // Heading left or right
            if (currentHeading.Y == 0)
            {
                var indexedRow = grid.GetRow(currentPosition.Y).WithIndex();

                var nextXCoord = currentHeading ==
                    Vector2D.Right
                        ? indexedRow.Where(ir => ir.Item != InValid).Min(ir => ir.Index)
                        : indexedRow.Where(ir => ir.Item != InValid).Max(ir => ir.Index);

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
                        ? indexedColumn.Where(ic => ic.Item != InValid).Min(ic => ic.Index)
                        : indexedColumn.Where(ic => ic.Item != InValid).Max(ic => ic.Index);

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
