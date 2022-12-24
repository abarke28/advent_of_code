using aoc.common;
using aoc.utils;

namespace aoc.y2022.day_24
{
    // https://adventofcode.com/2022/day/24
    public class Day_24 : ISolver
    {
        private const char OpenSpace = '.';

        private static readonly Dictionary<char, Vector2D> _charSnowflakeMap = new()
        {
            { '>', Vector2D.Right },
            { 'v', Vector2D.Down },
            { '<', Vector2D.Left },
            { '^', Vector2D.Up }
        };

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_24/input.txt");

            var (snowflakes, maxX, maxY, start, end) = ParseInput(lines);

            var shortestPath = FindShortestPath(start, end, maxX, maxY, snowflakes);
            Console.WriteLine(shortestPath);

            var returnStartingFlakes = MoveSnowflakesNTimes(snowflakes, maxX, maxY, shortestPath);
            var shortestReturnTrip = FindShortestPath(end, start, maxX, maxY, returnStartingFlakes);

            var finalReturnStartingFlakes = MoveSnowflakesNTimes(snowflakes, maxX, maxY, shortestPath + shortestReturnTrip);
            var shortestFinalReturnTrip = FindShortestPath(start, end, maxX, maxY, finalReturnStartingFlakes);
            Console.WriteLine(shortestPath + shortestReturnTrip + shortestFinalReturnTrip);
        }

        private static int FindShortestPath(Vector2D start,
                                            Vector2D end,
                                            int maxX,
                                            int maxY,
                                            IList<(Vector2D Position, Vector2D Direction)> snowflakes)
        {
            var paths = new Queue<Vector2D>();
            paths.Enqueue(start);

            var time = 0;

            while (true)
            {
                time++;

                var nextSnowflakes = MoveSnowflakes(snowflakes, maxX, maxY);
                var nextSnowflakePositions = nextSnowflakes.Select(s => s.Position).ToHashSet();

                var nextSteps = new HashSet<Vector2D>();

                while (paths.Count > 0)
                {
                    var currentPosition = paths.Dequeue();
                    var options = currentPosition.Get5Neighbors().Where(n => IsInBounds(n, maxX, maxY) || n == end || n == start);

                    var viableOptions = options.Where(o => !nextSnowflakePositions.Contains(o)).ToList();

                    if (viableOptions.Any(vo => vo == end))
                    {
                        return time;
                    }

                    if (viableOptions.Any())
                    {
                        foreach (var viableOption in viableOptions)
                        {
                            nextSteps.Add(viableOption);
                        }
                    }
                }

                foreach (var nextStep in nextSteps)
                {
                    paths.Enqueue(nextStep);
                }

                snowflakes = nextSnowflakes;
            }
        }

        private static List<(Vector2D Position, Vector2D Direction)> MoveSnowflakesNTimes(
            IList<(Vector2D Position, Vector2D Direction)> snowflakes,
            int maxX,
            int maxY,
            int n)
        {
            var currentSnowflakes = new List<(Vector2D Position, Vector2D Direction)>(snowflakes);

            for (var i = 0; i < n; i++)
            {
                currentSnowflakes = MoveSnowflakes(currentSnowflakes, maxX, maxY);
            }

            return currentSnowflakes;
        }

        private static List<(Vector2D Position, Vector2D Direction)> MoveSnowflakes(
            IList<(Vector2D Position, Vector2D Direction)> snowflakes,
            int maxX,
            int maxY)
        {
            var currentSnowflakeLocations = new List<(Vector2D, Vector2D)>(snowflakes.Count);

            for (var i = 0; i < snowflakes.Count; i++)
            {
                var (position, direction) = snowflakes[i];

                var newPosition = MoveSnowflake(position, direction, maxX, maxY);

                currentSnowflakeLocations.Add(new (newPosition, direction));
            }

            return currentSnowflakeLocations;
        }

        private static Vector2D MoveSnowflake(Vector2D snowflakePosition, Vector2D snowflakeDirection, int maxX, int maxY)
        {
            var targetPosition = snowflakePosition + snowflakeDirection;

            if (IsInBounds(targetPosition, maxX, maxY))
            {
                return targetPosition;
            }
            // Snowflake moving up or down
            else if (snowflakeDirection.X == 0)
            {
                var newPosition = snowflakeDirection.Y > 0 
                    ? new Vector2D(snowflakePosition.X, 1)
                    : new Vector2D(snowflakePosition.X, maxY - 1);

                return newPosition;
            }
            // Snowflake moving left or right
            else
            {
                var newPosition = snowflakeDirection.X > 0
                    ? new Vector2D(1, snowflakePosition.Y)
                    : new Vector2D(maxX - 1, snowflakePosition.Y);

                return newPosition;
            }
        }

        private static bool IsInBounds(Vector2D position, int maxX, int maxY)
        {
            return (position.X > 0 &&
                    position.X < maxX &&
                    position.Y > 0 &&
                    position.Y < maxY);
        }

        private static (List<(Vector2D Position, Vector2D Direction)> Snowflakes,
                        int MaxX,
                        int MaxY,
                        Vector2D start,
                        Vector2D end) ParseInput(IList<string> lines)
        {
            var snowflakes = new List<(Vector2D, Vector2D)>();

            var numLines = lines.Count;

            for (var y = 0; y < lines.Count; y++)
            {
                for (var x = 0; x < lines.First().Length; x++)
                {
                    var inputChar = lines[numLines - 1 - y][x];

                    if (_charSnowflakeMap.ContainsKey(inputChar))
                    {
                        var position = new Vector2D(x, y);
                        var snowflakeDirection = _charSnowflakeMap[inputChar];

                        snowflakes.Add(new (position, snowflakeDirection));
                    }
                }
            }

            var maxX = lines.First().Length - 1;
            var maxY = lines.Count - 1;

            var startX = lines.First().IndexOf(OpenSpace);
            var endX = lines.Last().IndexOf(OpenSpace);

            var start = new Vector2D(startX, maxY);
            var end = new Vector2D(endX, 0);

            return (snowflakes, maxX, maxY, start, end);
        }
    }
}
