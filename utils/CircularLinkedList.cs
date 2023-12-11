namespace Aoc.Utils
{
    public class CircularLinkedList<T>
    {
        public List<CircularLinkedListNode<T>> Nodes { get; set; } = new List<CircularLinkedListNode<T>>();

        public int Length => Nodes.Count;

        public CircularLinkedList(IList<T> collection)
        {
            var first = new CircularLinkedListNode<T>(collection[0], previous: null, next: null);
            var previous = first;

            for (int i = 1; i < collection.Count; i++)
            {
                var node = new CircularLinkedListNode<T>(collection[i], previous, next: null);
                Nodes[i - 1].Next = node;

                Nodes.Add(node);
            }

            first.Previous = Nodes.Last();
            Nodes.Last().Next = first;
        }

        public void AddBefore(T value, CircularLinkedListNode<T> target)
        {
            var currentPrevious = target.Previous;

            var newNode = new CircularLinkedListNode<T>(value, previous: currentPrevious, next: target);
            Nodes.Add(newNode);

            target.Previous = newNode;
            currentPrevious.Next = newNode;
        }

        public void AddAfter(T value, CircularLinkedListNode<T> target)
        {
            var currentNext = target.Next;

            var newNode = new CircularLinkedListNode<T>(value, previous: target, next: currentNext);
            Nodes.Add(newNode);

            target.Next = newNode;
            currentNext.Previous = newNode;
        }
    }
}
