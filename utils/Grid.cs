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

            _grid = new T?[_width, _height];
        }

        public Grid(int width, int height, T defaultValue)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            _width = width;
            _height = height;

            _grid = new T?[_width, _height];

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _grid[i, j] = defaultValue;
                }
            }
        }
        
        public T? GetItem(int x, int y)
        {
            if (x >= _width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y >= _height) throw new ArgumentOutOfRangeException(nameof(y));

            return _grid[x, y];
        }

        public void SetItem(int x, int y, T? item)
        {
            ValidateCoords(x, y);

            _grid[x, y] = item;
        }

        public List<T?> GetNeighbours(int x, int y)
        {
            return GetNeighboursWithCoords(x, y).Select(n => n.Item).ToList();
        }

        public List<T?> GetNeighbours(int x, int y, Func<T?, bool> predicate)
        {
            return GetNeighboursWithCoords(x, y, predicate).Select(n => n.Item).ToList();
        }

        public List<GridItem<T?>> GetNeighboursWithCoords(int x, int y)
        {
            return GetNeighboursWithCoordsImplementation(x, y);
        }

        public List<GridItem<T?>> GetNeighboursWithCoords(int x, int y, Func<T?, bool> predicate)
        {
            return GetNeighboursWithCoordsImplementation(x, y, predicate);
        }

        public bool IsCoordValid(int x, int y)
        {
            return ((x >= 0 && x < _width) && (y >= 0 && y < _height));
        }

        public void PrintGrid()
        {
            for (int j = 0; j < _height; j++)
            {
                for (int i = 0; i < _width; i++)
                {
                    Console.Write(_grid[i, j]?.ToString() ?? "NULL");
                    Console.Write(' ');
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        public static Grid<char> FromStrings(string[] input, Func<char, char>? mapper = null)
        {
            if (input.Select(s => s.Trim().Length).ToHashSet().Count != 1)
            {
                throw new ArgumentException("Input strings not of the same length");
            }

            var width = input.First().Length;
            var height = input.Count();

            var grid = new Grid<char>(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var inputChar = input[j][i];

                    grid.SetItem(i, j, mapper is null ? inputChar : mapper(inputChar));
                }
            }

            return grid;
        }

        private List<GridItem<T?>> GetNeighboursWithCoordsImplementation(int x, int y, Func<T?, bool>? predicate = null)
        {
            const int maxNeighbours = 8;

            ValidateCoords(x, y);

            var neighbours = new List<GridItem<T?>>(maxNeighbours);

            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    // Validate x & y in bounds, and exclude self
                    if ((i >= 0 && i < _width) &&
                        (j >= 0 && j < _height) &&
                        !(i == x && j == y))
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
            if (!IsCoordValid(x, y))
            {
                throw new Exception("Invalid co-ords");
            }
        }
    }
}
