namespace aoc.utils
{
    public class NodeGraph<TNodeKey> where TNodeKey : notnull
    {
        private readonly HashSet<TNodeKey> _nodeKeys;
        private readonly Dictionary<TNodeKey, Dictionary<TNodeKey, int>> _adjacencyLists;

        public int NodeCount { get; init; }

        public NodeGraph(int nodeCount)
        {
            if (nodeCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeCount));
            }

            NodeCount = nodeCount;

            _nodeKeys = new HashSet<TNodeKey>(nodeCount);
            _adjacencyLists = new Dictionary<TNodeKey, Dictionary<TNodeKey, int>>(nodeCount);
        }

        public void AddNode(TNodeKey key)
        {
            _nodeKeys.Add(key);
            AddEdge(key, key, 0);
        }

        public void AddEdge(TNodeKey n1, TNodeKey n2, int weight = 1, bool directed = true)
        {
            _nodeKeys.Add(n1);
            _nodeKeys.Add(n2);

            if (!_adjacencyLists.ContainsKey(n1))
            {
                _adjacencyLists.Add(n1, new Dictionary<TNodeKey, int>(NodeCount - 1));
            }

            if (!_adjacencyLists[n1].ContainsKey(n2))
            {
                _adjacencyLists[n1].Add(n2, weight);
            }
            else
            {
                _adjacencyLists[n1][n2] = weight;
            }

            if (!directed)
            {
                AddEdge(n2, n1, weight, directed: true);
            }
        }

        public int GetEdgeWeight(TNodeKey n1, TNodeKey n2)
        {
            var n1Adjacencies = GetAdjacentNodesWithWeights(n1);

            return n1Adjacencies.TryGetValue(n2, out var weight) ? weight : 0;
        }

        public IDictionary<TNodeKey, int> GetAdjacentNodesWithWeights(TNodeKey n)
        {
            return _adjacencyLists.TryGetValue(n, out var values) ? values : new Dictionary<TNodeKey, int>();
        }

        public IEnumerable<TNodeKey> GetAdjacentNodes(TNodeKey n)
        {
            var nodesWithWeights =  GetAdjacentNodesWithWeights(n);

            return nodesWithWeights.Select(n => n.Key);
        }

        public IDictionary<TNodeKey, int> GetMinDistances(TNodeKey start)
        {
            var unvisitedNodes = new HashSet<TNodeKey>(_nodeKeys);
            var nodeDistances = _nodeKeys.ToDictionary(n => n, _ => int.MaxValue);

            nodeDistances[start] = 0;

            for (var i = 0; i < _nodeKeys.Count; i++)
            {
                var current = GetMinUnvisited(nodeDistances, unvisitedNodes);
                unvisitedNodes.Remove(current);

                var adjacentNodes = GetAdjacentNodes(current);

                foreach (var adjacentNode in adjacentNodes)
                {
                    var newDistanceToAdjacentNode = nodeDistances[current] + GetEdgeWeight(current, adjacentNode);

                    nodeDistances[adjacentNode] = Math.Min(nodeDistances[adjacentNode], newDistanceToAdjacentNode);
                }
            }

            return nodeDistances;
        }

        /// <summary>
        /// Computes shortest path for all node pairs via Floyd-Warshall
        /// </summary>
        /// <returns>Dictionary taking the (to, from) tuple, returning the shortest path</returns>
        public IDictionary<(TNodeKey n1, TNodeKey n2), int> GetMinDistances()
        {
            var currentDistances = new int[NodeCount, NodeCount];
            var nodeKeysList = _nodeKeys.ToList();

            for (var i = 0; i < NodeCount; i++)
            {
                for (var j = 0; j < NodeCount; j++)
                {
                    if (i == j)
                    {
                        currentDistances[i, j] = 0;
                        continue;
                    }

                    var nodeKey1 = nodeKeysList[i];
                    var nodeKey2 = nodeKeysList[j];

                    var adjacencyDistance = GetEdgeWeight(nodeKey1, nodeKey2);
                    currentDistances[i, j] = adjacencyDistance > 0 ? adjacencyDistance : int.MaxValue / 3;
                }
            }

            for (var k = 0; k < NodeCount; k++)
            {
                for (var i = 0; i < NodeCount; i++)
                {
                    for (var j = 0; j < NodeCount; j++)
                    {
                        if (currentDistances[i, j] > currentDistances[i, k] + currentDistances[k, j])
                        {
                           currentDistances[i, j] = currentDistances[i, k] + currentDistances[k, j];
                        }
                    }
                }
            }

            var distancesMap = new Dictionary<(TNodeKey n1, TNodeKey n2), int>(NodeCount * NodeCount);

            for (var i = 0; i < NodeCount; i++)
            {
                for (var j = 0; j < NodeCount; j++)
                {
                    var nodeKey1 = nodeKeysList[i];
                    var nodeKey2 = nodeKeysList[j];

                    var shortestDistance = currentDistances[i, j];

                    distancesMap.TryAdd((nodeKey1, nodeKey2), shortestDistance);
                }
            }

            return distancesMap;
        }

        private static TNodeKey GetMinUnvisited(IDictionary<TNodeKey, int> distances, HashSet<TNodeKey> unvisitedNodes)
        {
            var min = int.MaxValue;
            var closest = default(TNodeKey);

            foreach (var (node, distance) in distances)
            {
                if (unvisitedNodes.Contains(node) && distance <= min)
                {
                    min = distance;
                    closest = node;
                }
            }

            return closest!;
        }
    }
}
