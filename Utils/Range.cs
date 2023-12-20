using System.Collections;
using System.Numerics;

namespace Aoc.Utils
{
    public struct Range<T> : IEnumerable<T>, IEquatable<T> where T : INumber<T>
    {
        public T LowerBound { get; set; }
        public T UpperBound { get; set; }

        public T Length => UpperBound - LowerBound;

        public Range(T lowerBound, T upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public bool ContainsValue(T num)
        {
            return (num >= LowerBound && num < UpperBound);
        }

        public bool ContainsRange(Range<T> range)
        {
            return (LowerBound <= range.LowerBound && UpperBound >= range.UpperBound);
        }

        public bool Overlaps(Range<T> range)
        {
            return ContainsValue(range.LowerBound) || ContainsValue(range.UpperBound - T.One);
        }

        public Range<T> GetOverlap(Range<T> range)
        {
            if (!Overlaps(range)) throw new Exception($"Range {this} does not overlap with range {range}.");

            var min = LowerBound < range.LowerBound ? LowerBound : range.LowerBound;
            var max = UpperBound > range.UpperBound ? UpperBound : range.UpperBound;

            return new Range<T>(min, max);
        }

        public Range<T> GetRangeGreaterThan(T threshold)
        {
            if (threshold >= UpperBound - T.One)
            {
                return new Range<T>(T.Zero, T.Zero);
            }
            else if (threshold < LowerBound)
            {
                return this;
            }
            else
            {
                return new Range<T>(threshold + T.One, UpperBound);
            }
        }

        public Range<T> GetRangeGreaterThanOrEqual(T threshold)
        {
            if (threshold >= UpperBound)
            {
                return new Range<T>(T.Zero, T.Zero);
            }
            else if (threshold <= LowerBound)
            {
                return this;
            }
            else
            {
                return new Range<T>(threshold, UpperBound);
            }
        }

        public Range<T> GetRangeLessThan(T threshold)
        {
            if (threshold <= LowerBound)
            {
                return new Range<T>(T.Zero, T.Zero);
            }
            else if (threshold >= UpperBound)
            {
                return this;
            }
            else
            {
                return new Range<T>(LowerBound, threshold);
            }
        }

        public Range<T> GetRangeLessThanOrEqual(T threshold)
        {
            if (threshold < LowerBound)
            {
                return new Range<T>(T.Zero, T.Zero);
            }
            else if (threshold >= UpperBound - T.One)
            {
                return this;
            }
            else
            {
                return new Range<T>(LowerBound, threshold + T.One);
            }
        }

        public override string ToString()
        {
            return $"[{LowerBound}, {UpperBound})";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LowerBound, UpperBound);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var n = LowerBound; n < UpperBound - T.One; n++)
            {
                yield return n;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(T? other)
        {
            if (other is null) return false;

            return GetHashCode() == other.GetHashCode();
        }
    }
}
