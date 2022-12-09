namespace aoc.utils
{
    public class Graph
    {
        protected readonly int _numberOfVertices;
        protected readonly bool _directed;
        protected readonly Grid<int?> _vertexMatrix;

        public int VerticesCount => _numberOfVertices;

        public Graph(int numOfVertices, bool directed = false)
        {
            if (numOfVertices <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numOfVertices));
            }

            _numberOfVertices = numOfVertices;
            _directed = directed;

            _vertexMatrix = new Grid<int?>(numOfVertices, numOfVertices);
        }

        public void AddEdge(int v1, int v2, int weight)
        {
            _vertexMatrix.SetValue(v1, v2, weight);

            if (!_directed)
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

                if (weight.HasValue)
                {
                    vertices.Add((Vertex: i, Weight: weight.Value));
                }
            }

            return vertices;
        }

        public int? GetEdgeWeight(int v1, int v2)
        {
            return _vertexMatrix.GetValue(v1, v2);
        }

        public void Print()
        {
            _vertexMatrix.PrintGrid();
        }
    }
}
