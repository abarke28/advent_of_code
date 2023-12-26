namespace Aoc.Utils
{
    public class NodeGraph<TNodeKey> where TNodeKey : notnull
    {
        private const int Infinity = int.MaxValue / 2;

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

        public void RemoveEdge(TNodeKey n1, TNodeKey n2, bool directed = true)
        {
            _adjacencyLists[n1].Remove(n2);

            if (!directed)
            {
                _adjacencyLists[n2].Remove(n1);
            }
        }

        public int GetEdgeWeight(TNodeKey n1, TNodeKey n2)
        {
            var n1Adjacencies = GetAdjacentNodesWithWeights(n1);

            return n1Adjacencies.TryGetValue(n2, out var weight) ? weight : 0;
        }

        public IList<TNodeKey> GetAllNodes()
        {
            return _nodeKeys.ToList();
        }

        public IList<NodeGraphEdge<TNodeKey>> GetAllEdges()
        {
            var edges = new List<NodeGraphEdge<TNodeKey>>();

            foreach (var node in GetAllNodes())
            {
                var adjacents = GetAdjacentNodes(node).Where(a => !a.Equals(node));

                foreach (var adj in adjacents)
                {
                    var weight = GetEdgeWeight(node, adj);
                    var oppositeWeight = GetEdgeWeight(adj, node);
                    var isDirected = weight != oppositeWeight;

                    edges.Add(new NodeGraphEdge<TNodeKey>(node, adj, weight, isDirected));
                }
            }

            return edges;
        }

        public IDictionary<TNodeKey, int> GetAdjacentNodesWithWeights(TNodeKey n, bool includeSelf = false)
        {
            return _adjacencyLists.TryGetValue(n, out var values) 
                ? values.Where(adj => !adj.Key.Equals(n) || includeSelf).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                : new Dictionary<TNodeKey, int>();
        }

        public IEnumerable<TNodeKey> GetAdjacentNodes(TNodeKey n, bool includeSelf = false)
        {
            var nodesWithWeights = GetAdjacentNodesWithWeights(n).Where(aj => !aj.Key.Equals(n) || includeSelf);

            return nodesWithWeights.Select(n => n.Key);
        }

        public IEnumerable<NodeGraphEdge<TNodeKey>> GetNodeEdges(TNodeKey n)
        {
            var adjs = GetAdjacentNodesWithWeights(n);

            foreach (var adj in adjs)
            {
                var isDirected = GetEdgeWeight(n, adj.Key) != GetEdgeWeight(adj.Key, n);

                yield return new NodeGraphEdge<TNodeKey>(n, adj.Key, adj.Value, isDirected);
            }
        }

        public IEnumerable<NodeGraphEdge<TNodeKey>> GetPathEdges(IList<TNodeKey> nodes)
        {
            var edges = new List<NodeGraphEdge<TNodeKey>>(nodes.Count - 1);

            for (int i = 1; i < nodes.Count; i++)
            {
                var n1 = nodes[i - 1];
                var n2 = nodes[i];

                var weight = GetEdgeWeight(n1, n2);
                var isDirected = weight != GetEdgeWeight(n2, n1);

                var edge = new NodeGraphEdge<TNodeKey>(n1, n2, weight, isDirected);

                edges.Add(edge);
            }

            return edges;
        }

        public IEnumerable<TNodeKey> GetShortestPath(TNodeKey start, TNodeKey goal)
        {
            var (_, predecessors) = GetDistancesAndPredecssors(start);

            var path = new List<TNodeKey>();

            var predecessor = predecessors[goal];

            while (!predecessor.Equals(start))
            {
                path.Add(predecessor);
                predecessor = predecessors[predecessor];
            }

            path.Add(start);
            path.Reverse();

            return path;
        }

        public (IDictionary<TNodeKey, int> Distances, IDictionary<TNodeKey, TNodeKey> Predecessors) GetDistancesAndPredecssors(TNodeKey start)
        {
            var nodeDistances = _nodeKeys.ToDictionary(n => n, _ => Infinity);
            nodeDistances[start] = 0;

            var predecessors = _nodeKeys.ToDictionary(n => n, _ => start);

            var toProcess = new PriorityQueue<TNodeKey, int>();
            toProcess.Enqueue(start, 0);

            while (toProcess.Count > 0)
            {
                var current = toProcess.Dequeue();

                foreach (var adj in GetAdjacentNodes(current))
                {
                    var potentialDistance = nodeDistances[current] + GetEdgeWeight(current, adj);

                    if (potentialDistance < nodeDistances[adj])
                    {
                        nodeDistances[adj] = potentialDistance;
                        predecessors[adj] = current;

                        toProcess.Enqueue(adj, potentialDistance);
                    }
                }
            }

            return (nodeDistances, predecessors);
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
                        currentDistances[i, j] = Math.Min(currentDistances[i, j], currentDistances[i, k] + currentDistances[k, j]);
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
