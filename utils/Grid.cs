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

        public Grid(List<List<T>> inputLists)
        {
            if (inputLists.Select(l => l.Count).Distinct().Count() > 1)
            {
                throw new ArgumentException("All input lists must be of the same length.");
            }

            var height = inputLists.Count;
            var width = inputLists.First().Count;

            _height = height;
            _width = width;
            _grid = new T?[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    SetValue(x, y, inputLists[y][x]);
                }
            }
        }

        public T? this[int x, int y]
        {
            get { return GetValue(x, y); }
            set { SetValue(x, y, value); }
        }

        public T? this[Vector2D v]
        {
            get { return GetValue(v); }
            set { SetValue(v, value); }
        }

        public T? GetValue(int x, int y)
        {
            if (x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y >= Height) throw new ArgumentOutOfRangeException(nameof(y));

            return _grid[x, y];
        }

        public T? GetValue(Vector2D v)
        {
            ValidateCoords(v);

            return _grid[v.X, v.Y];
        }

        public void SetValue(int x, int y, T? item)
        {
            ValidateCoords(x, y);

            _grid[x, y] = item;
        }

        public void SetValue(Vector2D v, T? item)
        {
            ValidateCoords(v);

            _grid[v.X, v.Y] = item;
        }

        public List<T?> Get4Neighbours(Vector2D v)
        {
            return Get4Neighbours(v.X, v.Y);
        }

        public List<T?> Get4Neighbours(int x, int y)
        {
            return Get4NeighboursWithCoords(x, y).Select(n => n.Item).ToList();
        }

        public List<T?> Get4Neighbours(Vector2D v, Func<T?, bool> predicate)
        {
            return Get4Neighbours(v.X, v.Y, predicate);
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

        public List<T?> Get8Neighbours(Vector2D v)
        {
            return Get8Neighbours(v.X, v.Y);
        }

        public List<T?> Get8Neighbours(int x, int y)
        {
            return Get8NeighboursWithCoords(x, y).Select(n => n.Item).ToList();
        }

        public List<T?> Get8Neighbours(Vector2D v, Func<T?, bool> predicate)
        {
            return Get8Neighbours(v.X, v.Y, predicate);
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

        public List<T?> GetColumn(int columnIndex)
        {
            if (!IsInBounds(columnIndex, 0))
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }

            var column = new List<T?>(Height);

            for (var i = 0; i < Height; i++)
            {
                column.Add(GetValue(columnIndex, i));
            }

            return column;
        }

        public List<List<T?>> GetColumns()
        {
            var cols = new List<List<T?>>();

            for (var x = 0; x < Width; x++)
            {
                cols.Add(GetColumn(x));
            }

            return cols;
        }

        public List<T?> GetRow(int rowIndex)
        {
            if (!IsInBounds(0, rowIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            var row = new List<T?>(Width);

            for (var i = 0; i < Width; i++)
            {
                row.Add(GetValue(i, rowIndex));
            }

            return row;
        }

        public List<List<T?>> GetRows()
        {
            var rows = new List<List<T?>>(Height);

            for (var i = 0; i < Height; i++)
            {
                rows.Add(GetRow(i));
            }

            return rows;
        }

        public IEnumerable<Vector2D> GetAllPoints()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    yield return new Vector2D(x, y);
                }
            }
        }

        public bool IsInBounds(int x, int y)
        {
            return ((x >= 0 && x < Width) && (y >= 0 && y < Height));
        }

        public bool IsInBounds(Vector2D v)
        {
            return ((v.X >= 0 && v.X < Width) && (v.Y >= 0 && v.Y < Height));
        }

        public void PrintGrid(Func<T?, string>? printer = null)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var value = GetValue(x, Height - y - 1);

                    var printValue = printer == null
                        ? value?.ToString() ?? "NULL"
                        : printer(value);

                    Console.Write(printValue);
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
        /// </summary>
        /// <param name="input">Source input strings.</param>
        /// <param name="mapper">Mandatory function to transform the chars to the desired type when filling the grid.</param>
        /// <returns>A grid of the desired type.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Grid<T> FromStrings(IList<string> input, Func<char, T> mapper)
        {
            if (input.Select(s => s.Length).ToHashSet().Count != 1)
            {
                throw new ArgumentException("Input strings not of uniform length.");
            }

            var width = input.First().Length;
            var height = input.Count;

            var grid = new Grid<T>(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var inputChar = input[height - 1 - y][x];

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
                for (int x = 0; x < Width; x++)
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
        /// <param name="generator">Function to compute grid values from Vector2Ds (x, y).</param>
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
        /// Transforms all spaces in the grid by way of a generator function.
        /// </summary>
        /// <param name="generator">Function to compute grid values from current values.</param>
        public void Transform(Func<T?, T?> generator)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x <= Width; x++)
                {
                    var value = GetValue(x, y);
                    SetValue(x, y, generator(value));
                }
            }
        }

        /// <summary>
        /// Finds the Vector2Ds of grid spaces the fulfill a predicate.
        /// </summary>
        /// <param name="predicate">Predicate which takes coordinates as input (x, y)</param>
        /// <returns>Enumerable of Vector2Ds.</returns>
        public IEnumerable<Vector2D> FindAll(Func<int, int, bool> predicate)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (predicate(x, y))
                    {
                        yield return new Vector2D(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the Vector2Ds of grid spaces the fulfill a predicate.
        /// </summary>
        /// <param name="predicate">Predicate which takes Vector2Ds as input (x, y)</param>
        /// <returns>Enumerable of Vector2Ds.</returns>
        public IEnumerable<Vector2D> FindAll(Func<Vector2D, bool> predicate)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (predicate(new Vector2D(x, y)))
                    {
                        yield return new Vector2D(x, y);
                    }
                }
            }
        }

        public bool TryFind(T value, out Vector2D location)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (GetValue(x, y)?.Equals(value) == true)
                    {
                        location = new Vector2D(x, y);
                        return true;
                    }
                }
            }

            location = Vector2D.Zero;
            return false;
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

        private void ValidateCoords(Vector2D v)
        {
            if (!IsInBounds(v))
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
