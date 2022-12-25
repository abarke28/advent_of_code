using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_23
{
    // https://adventofcode.com/2022/day/23
    public class Day_23 : ISolver
    {
        private static readonly Dictionary<Vector2D, IEnumerable<Vector2D>> _preferenceSequences = new()
        {
            { Vector2D.Up, new List<Vector2D> { Vector2D.Up, Vector2D.Down, Vector2D.Left, Vector2D.Right } },
            { Vector2D.Down, new List<Vector2D> { Vector2D.Down, Vector2D.Left, Vector2D.Right, Vector2D.Up } },
            { Vector2D.Left, new List<Vector2D> { Vector2D.Left, Vector2D.Right, Vector2D.Up, Vector2D.Down } },
            { Vector2D.Right, new List<Vector2D> { Vector2D.Right, Vector2D.Up, Vector2D.Down, Vector2D.Left } }
        };

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_23/input.txt");

            var elvesOriginalPositions = ParseInput(lines);

            var positions = new Queue<Vector2D>(4);
            positions.Enqueue(Vector2D.Up);
            positions.Enqueue(Vector2D.Down);
            positions.Enqueue(Vector2D.Left);
            positions.Enqueue(Vector2D.Right);

            var elfPositions = ExecuteNRounds(elvesOriginalPositions, positions, 10);
            var score = CalculateScore(elfPositions);
            Console.WriteLine(score);

            var positions2 = new Queue<Vector2D>(4);
            positions2.Enqueue(Vector2D.Up);
            positions2.Enqueue(Vector2D.Down);
            positions2.Enqueue(Vector2D.Left);
            positions2.Enqueue(Vector2D.Right);

            var roundNum = FindFirstRoundWithNoMoves(elvesOriginalPositions, positions2);
            Console.WriteLine(roundNum);
        }

        private static int FindFirstRoundWithNoMoves(ISet<Vector2D> originalPositions, Queue<Vector2D> directionPreferences)
        {
            var positions = new HashSet<Vector2D>(originalPositions);
            var roundCount = 0;

            while (true)
            {
                roundCount++;

                var directionPreference = directionPreferences.Dequeue();

                var newPositions = ExecuteRound(positions, directionPreference);

                if (newPositions.All(np => positions.Contains(np)))
                {
                    return roundCount;
                }

                directionPreferences.Enqueue(directionPreference);
                positions = new HashSet<Vector2D>(newPositions);
            }
        }

        private static ISet<Vector2D> ExecuteNRounds(ISet<Vector2D> originalPositions, Queue<Vector2D> directionPreferences, int numRounds)
        {
            var positions = new HashSet<Vector2D>(originalPositions);

            for (var i = 0; i < numRounds; i++)
            {
                var directionPreference = directionPreferences.Dequeue();

                var newPositions = ExecuteRound(positions, directionPreference);

                directionPreferences.Enqueue(directionPreference);
                positions = new HashSet<Vector2D>(newPositions);
            }

            return positions;
        }

        private static ISet<Vector2D> ExecuteRound(ISet<Vector2D> elfPositions, Vector2D directionPreference)
        {
            var proposedMoves = GetProposedMoves(elfPositions, directionPreference);
            var proposedTargets = proposedMoves.Select(pm => pm.Value);

            var proposedMovesCount = proposedTargets.Distinct().ToDictionary(pt => pt, pt => 0);

            foreach (var proposedTarget in proposedTargets)
            {
                proposedMovesCount[proposedTarget]++;
            }

            var newPositions = new HashSet<Vector2D>(elfPositions.Count);

            foreach (var proposedMove in proposedMoves)
            {
                if (proposedMovesCount[proposedMove.Value] == 1)
                {
                    newPositions.Add(proposedMove.Value);
                }
                else
                {
                    newPositions.Add(proposedMove.Key);
                }
            }

            return newPositions;
        }

        private static IDictionary<Vector2D, Vector2D> GetProposedMoves(ISet<Vector2D> elfPositions, Vector2D directionPreference)
        {
            var proposedMoves = new Dictionary<Vector2D, Vector2D>(elfPositions.Count());

            foreach (var elfPosition in elfPositions)
            {
                var foundMove = false;

                foreach (var direction in _preferenceSequences[directionPreference])
                {
                    var allNeighbors = elfPosition.Get8Neighbors();

                    if (allNeighbors.All(n => !elfPositions.Contains(n)))
                    {
                        foundMove = true;
                        proposedMoves.Add(elfPosition, elfPosition);
                        break;
                    }

                    var neighbors = GetNeighborsForDirection(elfPosition, direction);

                    if (neighbors.All(n => !elfPositions.Contains(n)))
                    {
                        foundMove = true;
                        proposedMoves.Add(elfPosition, elfPosition + direction);
                        break;
                    }
                }

                if (!foundMove)
                {
                    proposedMoves.Add(elfPosition, elfPosition);
                }
            }

            return proposedMoves;
        }

        private static IEnumerable<Vector2D> GetNeighborsForDirection(Vector2D elfPosition, Vector2D direction)
        {
            var neighbors = elfPosition.Get8Neighbors();

            if (direction == Vector2D.Up)
            {
                return neighbors.Where(n => n.Y == elfPosition.Y + 1);
            }
            else if (direction == Vector2D.Down)
            {
                return neighbors.Where(n => n.Y == elfPosition.Y - 1);
            }
            else if (direction == Vector2D.Left)
            {
                return neighbors.Where(n => n.X== elfPosition.X - 1);
            }
            else if (direction == Vector2D.Right)
            {
                return neighbors.Where(n => n.X == elfPosition.X + 1);
            }

            throw new Exception($"Unexpected direction: {direction}");
        }

        private static int CalculateScore(ISet<Vector2D> positions)
        {
            var minX = positions.Min(p => p.X);
            var maxX = positions.Max(p => p.X);

            var minY = positions.Min(p => p.Y);
            var maxY = positions.Max(p => p.Y);

            var score = 0;

            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    if (!positions.Contains(new Vector2D(x, y)))
                    {
                        score++;
                    }
                }
            }

            return score;
        }

        private static ISet<Vector2D> ParseInput(IList<string> lines)
        {
            const char Elf = '#';

            var elves = new HashSet<Vector2D>();

            for (var i = 0; i < lines.Count; i++)
            {
                for (var j = 0; j <lines.First().Count(); j++)
                {
                    if (lines[i][j] == Elf)
                    {
                        elves.Add(new Vector2D(j, i * -1));
                    }
                }
            }

            return elves;
        }
    }
}
