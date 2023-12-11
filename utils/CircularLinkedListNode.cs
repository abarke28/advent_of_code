namespace Aoc.Utils
{
    public class CircularLinkedListNode<T>
    {
        private CircularLinkedListNode<T>? _previous;
        private CircularLinkedListNode<T>? _next;

        public T Value { get; set; }

        public CircularLinkedListNode<T> Previous
        {
            get { return _previous!; }
            set { _previous = value; }
        }

        public CircularLinkedListNode<T> Next
        {
            get { return _next!; }
            set { _next = value; }
        }

        public CircularLinkedListNode(T value, CircularLinkedListNode<T>? previous, CircularLinkedListNode<T>? next)
        {
            Value = value;
            _previous = previous;
            _next = next;
        }
    }
}
