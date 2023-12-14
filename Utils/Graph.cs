namespace Aoc.Utils
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

        public void AddEdge(int v1, int v2, int weight = 1, bool directed = true)
        {
            _vertexMatrix.SetValue(v1, v2, weight);

            if (!directed)
            {
                _vertexMatrix.SetValue(v2, v1, weight);
            }
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
            if (!_vertexMatrix.IsInBounds(v1, v2))
            {
                throw new ArgumentOutOfRangeException("Vertices out of range.");
            }

            var edgeValue = _vertexMatrix.GetValue(v1, v2);

            return edgeValue;
        }

        public void Print()
        {
            _vertexMatrix.Print();
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

        /// <summary>
        /// Returns a path from start to end via DFS
        /// </summary>
        public IEnumerable<int> GetPath(int start, int end)
        {
            var path = new Stack<int>();
            var visited = new HashSet<int>(VerticesCount);

            var currentVertex = start;
            path.Push(currentVertex);
            visited.Add(currentVertex);

            while (currentVertex != end)
            {
                var adjacentVertices = GetAdjacentVertices(currentVertex).Where(v => !visited.Contains(v));

                if (!adjacentVertices.Any())
                {
                    path.Pop();

                    if (path.Count == 0)
                    {
                        break;
                    }

                    currentVertex = path.Peek();
                }
                else
                {
                    var nextVertex = adjacentVertices.First();
                    path.Push(nextVertex);
                    visited.Add(nextVertex);
                    currentVertex = nextVertex;

                    if (currentVertex == end)
                    {
                        var resultPath = path.ToList();
                        resultPath.Reverse();

                        return resultPath;
                    }
                }
            }

            throw new Exception("No path exists");
        }

        /// <summary>
        /// Executes a BFS to produce an ordered sequence of vertices.
        /// </summary>
        /// <param name="start">Starting vertex.</param>
        /// <param name="vertexComparer">Optional delegate to compute the sorting of same-level vertices. Default is int comparison of the vertex number.</param>
        /// <returns></returns>
        public IEnumerable<int> GetGraphTraversal(int start, Func<int, int, int>? vertexComparer = null)
        {
            var comparer = vertexComparer == null 
                ? Comparer<int>.Default
                : new VertexComparer(vertexComparer) as IComparer<int>;

            var path = new Queue<int>();
            var traversalOrder = new Queue<int>();
            var visited = new HashSet<int>(VerticesCount);

            int currentVertex;

            path.Enqueue(start);
            traversalOrder.Enqueue(start);

            visited.Add(start);

            while (path.Count > 0)
            {
                currentVertex = path.Dequeue();

                var adjacentVertices = GetAdjacentVertices(currentVertex).Where(v => !visited.Contains(v))
                                                                         .ToList();
                if (adjacentVertices.Any())
                {
                    adjacentVertices.Sort(comparer);

                    foreach(var vertex in adjacentVertices)
                    {
                        path.Enqueue(vertex);
                        traversalOrder.Enqueue(vertex);
                        visited.Add(currentVertex);
                    }
                }
            }

            return traversalOrder.ToList();
        }

        public int CalculatePathCost(IList<int> path)
        {
            var cost = 0;

            for (var i = 0; i < path.Count - 1; i++)
            {
                var edge = GetEdgeWeight(path[i], path[i + 1]);

                if (edge == 0)
                {
                    throw new Exception($"Path contains invalid traversal from node {i} to node {i + 1}.");
                }

                cost += edge;
            }

            return cost;
        }

        public static Graph FromGrid<T>(Grid<T> grid,
                                        Func<Vector2D,IEnumerable<Vector2D>> adjacentNodeSelctor,
                                        Func<T, T, int> edgeWeightSelector)
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
