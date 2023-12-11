using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.y2021.day_04
{
    // https://adventofcode.com/2021/day/04
    public class Day_04
    {
        private const int BingoDimension = 5;
        private const int Mark = -1; 

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_04/input.txt");

            var bingoNums = lines[0].ReadAllNumbers<int>(',');

            var bingoBoards = GenerateBoards(lines.TakeLast(lines.Count - 1));

            var (board, winningNum) = FindWinningBoard(bingoBoards, bingoNums);

            var score = ComputeWinningScore(board, winningNum);

            Console.WriteLine(score);

            var bingoBoards2 = GenerateBoards(lines.TakeLast(lines.Count - 1));

            var (finalBoard, finalWiningNum) = PlayBoardsTillFinalWinner(bingoBoards2, bingoNums);

            var finalScore = ComputeWinningScore(finalBoard, finalWiningNum);

            Console.WriteLine(finalScore);
        }

        private static (Grid<int> board, int winningNum) FindWinningBoard(IList<Grid<int>> boards, IEnumerable<int> bingoNums)
        {
            foreach (var bingoNum in bingoNums)
            {
                foreach (var bingoBoard in boards)
                {
                    MarkNumIfPresent(bingoBoard, bingoNum);

                    if (IsBoardWinning(bingoBoard))
                    {
                        return (bingoBoard, bingoNum);
                    }
                }
            }

            throw new Exception("No winning boards");
        }

        private static (Grid<int> board, int winningNum) PlayBoardsTillFinalWinner(IList<Grid<int>> boards, IEnumerable<int> bingoNums)
        {
            var numBoards = boards.Count;
            var winningBoardsCount = 0;

            foreach (var bingoNum in bingoNums)
            {
                foreach (var bingoBoard in boards)
                {
                    if (IsBoardWinning(bingoBoard))
                    {
                        continue;
                    }

                    MarkNumIfPresent(bingoBoard, bingoNum);

                    if (IsBoardWinning(bingoBoard))
                    {
                        winningBoardsCount++;

                        if (winningBoardsCount == numBoards)
                        {
                            return (bingoBoard, bingoNum);
                        }
                    }
                }
            }

            throw new Exception("Failed to find final board");
        }

        private static List<Grid<int>> GenerateBoards(IEnumerable<string> input)
        {
            var boards = new List<Grid<int>>();

            var trimmedLines = input.Where(l => !string.IsNullOrEmpty(l));

            for (var i = 0; i < trimmedLines.Count();)
            {
                var boardLines = trimmedLines.Skip(i)
                                             .Take(BingoDimension)
                                             .Select(l => l.ReadAllNumbers<int>().ToList())
                                             .ToList()!;

                var board = new Grid<int>(boardLines)!;

                boards.Add(board);

                i += BingoDimension;
            }

            return boards;
        }

        private static int ComputeWinningScore(Grid<int> board, int num)
        {
            var score = 0;

            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    var value = board.GetValue(x, y);

                    if (value != Mark)
                    {
                        score += value;
                    }
                }
            }

            return score * num;
        }

        private static void MarkNumIfPresent(Grid<int> board, int num)
        {
            if (board.TryFind(num, out var location))
            {
                board.SetValue(location.X, location.Y, Mark);
            }
        }

        private static bool IsBoardWinning(Grid<int> board)
        {
            for (var i = 0; i < BingoDimension; i++)
            {
                if (board.GetRow(i).All(n => n == Mark)) return true;
                if (board.GetColumn(i).All(n => n == Mark)) return true;
            }

            return false;
        }
    }
}
