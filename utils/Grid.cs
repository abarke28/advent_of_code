namespace aoc.utils
{
    public class Grid<T>
    {
        private readonly int _width;
        private readonly int _height;
        private readonly T?[,] _grid;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public Grid(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;

            _grid = new T?[Width, Height];
        }

        public Grid(int width, int height, T defaultValue)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;

            _grid = new T?[Width, Height];

            Fill(defaultValue);
        }

        public Grid(int width, int height, Func<int, int, T?> generatingFunction)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;

            _grid = new T?[Width, Height];

            Fill(generatingFunction);
        }
        
        public T? GetValue(int x, int y)
        {
            if (x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y >= Height) throw new ArgumentOutOfRangeException(nameof(y));

            return _grid[x, y];
        }

        public void SetValue(int x, int y, T? item)
        {
            ValidateCoords(x, y);

            _grid[x, y] = item;
        }

        public List<T?> Get4Neighbours(int x, int y)
        {
            return Get4NeighboursWithCoords(x, y).Select(n => n.Item).ToList();
        }

        public List<T?> Get4Neighbours(int x, int y, Func<T?, bool> predicate)
        {
            return Get4NeighboursWithCoords(x, y, predicate).Select(n => n.Item).ToList();
        }

        public List<GridItem<T?>> Get4NeighboursWithCoords(int x, int y)
        {
            return Get4NeighboursWithCoordsImplementation(x, y);
        }

        public List<GridItem<T?>> Get4NeighboursWithCoords(int x, int y, Func<T?, bool> predicate)
        {
            return Get4NeighboursWithCoordsImplementation(x, y, predicate);
        }

        public List<T?> Get8Neighbours(int x, int y)
        {
            return Get8NeighboursWithCoords(x, y).Select(n => n.Item).ToList();
        }

        public List<T?> Get8Neighbours(int x, int y, Func<T?, bool> predicate)
        {
            return Get8NeighboursWithCoords(x, y, predicate).Select(n => n.Item).ToList();
        }

        public List<GridItem<T?>> Get8NeighboursWithCoords(int x, int y)
        {
            return Get8NeighboursWithCoordsImplementation(x, y);
        }

        public List<GridItem<T?>> Get8NeighboursWithCoords(int x, int y, Func<T?, bool> predicate)
        {
            return Get8NeighboursWithCoordsImplementation(x, y, predicate);
        }

        public bool IsInBounds(int x, int y)
        {
            return ((x >= 0 && x < Width) && (y >= 0 && y < Height));
        }

        public void PrintGrid()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(_grid[x, y]?.ToString() ?? "NULL");
                    Console.Write(' ');
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        public Dictionary<string, int> CountDistinct() 
        {
            var itemsCount = new Dictionary<string, int>();

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    var key = GetValue(i, j)?.ToString() ?? "NULL";

                    if (itemsCount.ContainsKey(key))
                    {
                        itemsCount[key]++;
                    }
                    else
                    {
                        itemsCount.Add(key, 1);
                    }
                }
            }

            return itemsCount;
        }

        /// <summary>
        /// Generate a grid from an array of strings. Each string must have the same length.
        /// Each string will be trimmed.
        /// </summary>
        /// <param name="input">Source input strings.</param>
        /// <param name="mapper">Optional function to transform source characters when filling the grid.</param>
        /// <returns>A grid of chars.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Grid<char> FromStrings(string[] input, Func<char, char>? mapper = null)
        {
            if (input.Select(s => s.Trim().Length).ToHashSet().Count != 1)
            {
                throw new ArgumentException("Input strings not of uniform length.");
            }

            var width = input.First().Length;
            var height = input.Count();

            var grid = new Grid<char>(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var inputChar = input[y][x];

                    grid.SetValue(x, y, mapper is null ? inputChar : mapper(inputChar));
                }
            }

            return grid;
        }

        /// <summary>
        /// Generate a grid from an array of strings. Each string must have the same length.
        /// </summary>
        /// <param name="input">Source input strings.</param>
        /// <param name="mapper">Mandatory function to transform the chars to the desired type when filling the grid.</param>
        /// <returns>A grid of the desired type.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Grid<T> FromStrings(string[] input, Func<char, T> mapper)
        {
            if (input.Select(s => s.Trim().Length).ToHashSet().Count != 1)
            {
                throw new ArgumentException("Input strings not of uniform length.");
            }

            var width = input.First().Length;
            var height = input.Count();

            var grid = new Grid<T>(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var inputChar = input[y][x];

                    grid.SetValue(x, y, mapper(inputChar));
                }
            }

            return grid;
        }

        /// <summary>
        /// Fill all spaces in the grid with the supplied value
        /// </summary>
        /// <param name="value"></param>
        public void Fill(T value)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x <= Width; x++)
                {
                    SetValue(x, y, value);
                }
            }
        }

        /// <summary>
        /// Fill all spaces in the grid with the supplied value, if the current value satisfies a predicate.
        /// </summary>
        /// <param name="value">The value to fill with.</param>
        /// <param name="predicate">The predicate to determine if the grid space should be filled, based on the current value.</param>
        public void Fill(T value, Func<T?, bool> predicate)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x <= Width; x++)
                {
                    var currentValue = GetValue(x, y);

                    if (predicate(currentValue))
                    {
                        SetValue(x, y, value);
                    }
                }
            }
        }

        /// <summary>
        /// Fill all spaces in the grid with the supplied value, if the coordinates satisfy a predicate.
        /// </summary>
        /// <param name="value">The value to fill with.</param>
        /// <param name="predicate">Predicate to determine if the grid space should be filled, based on the space coordinates (x, y)</param>
        public void Fill(T value, Func<int, int, bool> predicate)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x <= Width; x++)
                {
                    if (predicate(x, y))
                    {
                        SetValue(x, y, value);
                    }
                }
            }
        }

        /// <summary>
        /// Fills all spaces in the grid by way of a generator function.
        /// </summary>
        /// <param name="generator">Function to compute grid values from coordinates (x, y).</param>
        public void Fill(Func<int, int, T?> generator)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x <= Width; x++)
                {
                    SetValue(x, y, generator(x, y));
                }
            }
        }

        /// <summary>
        /// Finds the coordinates of grid spaces the fulfill a predicate.
        /// </summary>
        /// <param name="predicate">Predicate which takes coordinates as input (x, y)</param>
        /// <returns>Enumerable of coordinates.</returns>
        public IEnumerable<Coordinate> FindAll(Func<int, int, bool> predicate)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (predicate(x, y))
                    {
                        yield return new Coordinate(x, y);
                    }
                }
            }
        }

        private List<GridItem<T?>> Get4NeighboursWithCoordsImplementation(int x, int y, Func<T?, bool>? predicate = null)
        {
            const int maxNeighbours = 4;

            ValidateCoords(x, y);

            var neighbours = new List<GridItem<T?>>(maxNeighbours);

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (IsInBounds(i, j) && AreCardinal(x, y, i, j))
                    {
                        neighbours.Add(new GridItem<T?>(i, j, _grid[i, j]));
                    }
                }
            }

            return predicate == null
                ? neighbours
                : neighbours.Where(n => predicate(n.Item)).ToList();
        }

        private List<GridItem<T?>> Get8NeighboursWithCoordsImplementation(int x, int y, Func<T?, bool>? predicate = null)
        {
            const int maxNeighbours = 8;

            ValidateCoords(x, y);

            var neighbours = new List<GridItem<T?>>(maxNeighbours);

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (IsInBounds(i, j) && !AreTheSame(x, y, i, j))
                    {
                        neighbours.Add(new GridItem<T?>(i, j, _grid[i, j]));
                    }
                }
            }

            return predicate == null
                ? neighbours
                : neighbours.Where(n => predicate(n.Item)).ToList();
        }

        private void ValidateCoords(int x, int y)
        {
            if (!IsInBounds(x, y))
            {
                throw new Exception("Invalid coordinates");
            }
        }

        private static bool AreCardinal(int x1, int y1, int x2, int y2)
        {
            return (x1 == x2) ^ (y1 == y2);
        }

        private static bool AreTheSame(int x1, int y1, int x2, int y2)
        {
            return (x1 == x2) && (y1 == y2);
        }
    }
}
