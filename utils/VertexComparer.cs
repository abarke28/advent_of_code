namespace aoc.utils
{
    public class VertexComparer: IComparer<int>
    {
        private readonly Func<int, int, int> _vertexComparer;

        public VertexComparer(Func<int, int, int> vertexComparer)
        {
            _vertexComparer = vertexComparer;
        }

        public int Compare(int x, int y)
        {
            return _vertexComparer(x, y);
        }
    }
}
