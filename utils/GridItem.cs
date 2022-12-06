namespace aoc.utils
{
    public class GridItem<T>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public T? Item { get; set; }

        public GridItem(int x, int y, T? item)
        {
            X = x;
            Y = y;
            Item = item;
        }
    }
}
