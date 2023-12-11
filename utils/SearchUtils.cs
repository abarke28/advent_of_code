using System;
using System.Collections.Generic;
using System.Linq;
namespace aoc.utils
{
    public static class SearchUtils
    {
        public static HashSet<T> FloodFill<T>(
            T start,
            Func<T, bool> insidePredicate,
            Func<T, IEnumerable<T>> neighborSelector,
            out int maxDepth)
        {
            var insideSet = new HashSet<T>();

            var unvisited = new Queue<T>();
            var visited = new HashSet<T>();
            var depth = -1;

            unvisited.Enqueue(start);

            while (unvisited.Count > 0)
            {
                var nodesAtDepth = unvisited.Count;

                while (nodesAtDepth-- > 0)
                {
                    var candidate = unvisited.Dequeue();

                    if (insidePredicate.Invoke(candidate))
                    {
                        insideSet.Add(candidate);

                        var neighbors = neighborSelector.Invoke(candidate);

                        foreach (var neighbor in neighbors.Where(n => !visited.Contains(n)))
                        {
                            unvisited.Enqueue(neighbor);
                        }
                    }

                    visited.Add(candidate);
                }

                depth++;
            }

            maxDepth = depth;

            return insideSet;
        }
    }
}
