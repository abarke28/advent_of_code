namespace aoc.utils.extensions
{
    public static class GridExtensions
    {
        public static IDictionary<Vector2D, int> GetMinDistances<T>(this Grid<T> source, Vector2D start, Func<T, T, int> weightSelector)
        {
            var allNodes = new List<Vector2D>(source.Width * source.Height);

            for (var x = 0; x < source.Width; x++)
            {
                for (var y = 0; y < source.Height; y++)
                {
                    allNodes.Add(new Vector2D(x, y));
                }
            }

            var unvisited = new HashSet<Vector2D>(allNodes);
            var distances = allNodes.ToDictionary(n => n, _ => int.MaxValue);

            distances[start] = 0;

            for (var i = 0; i < allNodes.Count; i++)
            {
                var current = GetMinUnvisited(distances, unvisited);
                unvisited.Remove(current);

                var adjacentNodes = current.Get4Neighbors().Where(n => source.IsInBounds(n));

                foreach (var adjacentNode in adjacentNodes)
                {
                    var currentValue = source.GetValue(current);
                    var adjacentValue = source.GetValue(adjacentNode);

                    var newDistanceToAdjacentNode = distances[current] + weightSelector(currentValue!, adjacentValue!);

                    if (newDistanceToAdjacentNode < distances[adjacentNode])
                    {
                        distances[adjacentNode] = newDistanceToAdjacentNode;
                    }
                }
            }

            return distances;
        }

        private static Vector2D GetMinUnvisited(IDictionary<Vector2D, int> distances, ISet<Vector2D> unvisited)
        {
            var min = int.MaxValue;
            var closest = Vector2D.Zero;

            foreach (var (node, distance) in distances)
            {
                if (unvisited.Contains(node) && distance <= min)
                {
                    min = distance;
                    closest = node;
                }
            }

            return closest;
        }
    }
}
