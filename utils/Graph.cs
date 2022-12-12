namespace aoc.utils
{
    public class Graph
    {
        protected readonly int _numberOfVertices;
        protected readonly Grid<int> _vertexMatrix;

        public int VerticesCount => _numberOfVertices;

        public Graph(int numOfVertices)
        {
            if (numOfVertices <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numOfVertices));
            }

            _numberOfVertices = numOfVertices;

            _vertexMatrix = new Grid<int>(numOfVertices, numOfVertices, 0);
        }

        public void AddEdge(int v1, int v2, int weight)
        {
            _vertexMatrix.SetValue(v1, v2, weight);
        }

        public IEnumerable<int> GetAdjacentVertices(int v)
        {
            var vertices = GetAdjacentVerticesWithWeights(v).Select(v => v.Vertex);

            return vertices;
        }

        public IEnumerable<(int Vertex, int Weight)> GetAdjacentVerticesWithWeights(int v)
        {
            if (v < 0 || v >= _numberOfVertices)
            {
                throw new ArgumentOutOfRangeException(nameof(v));
            }

            var vertices = new List<(int Vertex, int Weight)>(_numberOfVertices);

            for (var i = 0; i < _numberOfVertices; i++)
            {
                var weight = _vertexMatrix.GetValue(v, i);

                if (weight > 0)
                {
                    vertices.Add((Vertex: i, Weight: weight));
                }
            }

            return vertices;
        }

        public int GetEdgeWeight(int v1, int v2)
        {
            var edgeValue = _vertexMatrix.GetValue(v1, v2);

            return edgeValue;
        }

        public void Print()
        {
            _vertexMatrix.PrintGrid();
        }

        public Dictionary<int, int> GetMinDistances(int start)
        {
            var allVertices = Enumerable.Range(0, VerticesCount).ToList();
            var unvisited = new HashSet<int>(allVertices);
            var distances = allVertices.ToDictionary(v => v, v => int.MaxValue);

            distances[start] = 0;

            for (var i = 0; i < allVertices.Count; i++)
            {
                var current = GetMinUnvisited(distances, unvisited);
                unvisited.Remove(current);

                var adjacentVertices = GetAdjacentVertices(current);

                foreach (var adjacentVertex in adjacentVertices)
                {
                    var newDistanceToAdjacentVertex = distances[current] + GetEdgeWeight(current, adjacentVertex);

                    if (newDistanceToAdjacentVertex < distances[adjacentVertex])
                    {
                        distances[adjacentVertex] = newDistanceToAdjacentVertex;
                    }
                }
            }

            return distances;
        }

        public static Graph FromGrid<T>(Grid<T> grid,Func<Vector2D, IEnumerable<Vector2D>> adjacentNodeSelctor, Func<T, T, int> edgeWeightSelector)
        {
            var graph = new Graph(grid.Width * grid.Height);

            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var nodeNumber = GenerateNodeNumber(x, y, grid.Width);
                    var nodePosition = new Vector2D(x, y);
                    var nodeVal = grid.GetValue(nodePosition);

                    var adjacentNodePositions = adjacentNodeSelctor(nodePosition).Where(an => grid.IsInBounds(an));

                    foreach (var adjacentNodePosition in adjacentNodePositions)
                    {
                        var adjacentNodeVal = grid.GetValue(adjacentNodePosition);
                        var adjacentNodeNumber = GenerateNodeNumber(adjacentNodePosition.X,
                                                                    adjacentNodePosition.Y,
                                                                    grid.Width);

                        var edgeWeight = edgeWeightSelector(nodeVal!, adjacentNodeVal!);

                        graph.AddEdge(nodeNumber, adjacentNodeNumber, edgeWeight);
                    }
                }
            }

            return graph;
        }

        private static int GenerateNodeNumber(int x, int y, int gridWidth)
        {
            return x + (y * gridWidth);
        }

        private static int GetMinUnvisited(Dictionary<int, int> distances, IReadOnlySet<int> unvisited)
        {
            var min = int.MaxValue;
            var closest = -1;

            foreach (var (vertex, distance) in distances)
            {
                if (unvisited.Contains(vertex) && distance <= min)
                {
                    min = distance;
                    closest = vertex;
                }
            }

            return closest;
        }
    }
}
