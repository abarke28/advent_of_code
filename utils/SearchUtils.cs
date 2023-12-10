using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aoc.utils
{
    public static class SearchUtils
    {
        public static HashSet<Vector2D> FloodFill(
            Vector2D start,
            Func<Vector2D, bool> insidePredicate,
            Func<Vector2D, IEnumerable<Vector2D>> neighborSelector)
        {
            var insideSet = new HashSet<Vector2D>();

            var toProcess = new Queue<Vector2D>();
            var processed = new HashSet<Vector2D>();

            toProcess.Enqueue(start);

            while (toProcess.Count > 0)
            {
                var candidate = toProcess.Dequeue();

                if (insidePredicate.Invoke(candidate))
                {
                    insideSet.Add(candidate);

                    var neighbors = neighborSelector.Invoke(candidate);

                    foreach (var neighbor in neighbors.Where(n => !processed.Contains(n)))
                    {
                        toProcess.Enqueue(neighbor);
                    }
                }

                processed.Add(candidate);
            }

            return insideSet;
        }
    }
}
