namespace Aoc.Utils
{
    public class NodeGraphEdge<TNodeKey> : IEquatable<NodeGraphEdge<TNodeKey>> where TNodeKey : notnull
    {
        public TNodeKey Node1 { get; set; }
        public TNodeKey Node2 { get; set; }
        public int Weight { get; set; }
        public bool Directed { get; set; }

        public NodeGraphEdge(TNodeKey node1, TNodeKey node2, int weight = 1, bool directed = true)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
            Directed = directed;
        }

        public bool Equals(NodeGraphEdge<TNodeKey>? other)
        {
            if (other is null) return false;

            var isExactlyEqual = Node1.Equals(other.Node1) && Node2.Equals(other.Node2) && Weight == other.Weight;

            if (isExactlyEqual) return true;

            if (!Directed)
            {
                return Node1.Equals(other.Node2) && Node2.Equals(other.Node1) && Weight == other.Weight; ;
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Directed)
            {
                return HashCode.Combine(Node1, Node2, Weight);
            }
            else
            {
                return Weight.GetHashCode() + HashCode.Combine(Math.Min(Node1.GetHashCode(), Node2.GetHashCode()), Math.Max(Node1.GetHashCode(), Node2.GetHashCode()));
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (obj is not NodeGraphEdge<TNodeKey> node)
            {
                return false;
            } else
            {
                return this.Equals(node);
            }
        }

        public override string ToString()
        {
            return $"{Node1} => {Node2} (Weight = {Weight}) - (Directed = {Directed}";
        }
    }
}
