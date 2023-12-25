using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;
using Range = Aoc.Utils.Range<long>;

namespace Aoc.Y2023.Day_24
{
    // https://adventofcode.com/2023/day/24
    public class Day_24 : ISolver
    {
        private const long IntersectionLowBoundXY  = 200_000_000_000_000;
        private const long IntersectionHighBoundXY = 400_000_000_000_000;

        private const long IntersecionLowBoundXYExample = 7;
        private const long IntersectionHighBoundXYExample = 27;

        private record Hail(Vector3D Pos, Vector3D Velocity);

        public object Part1(IList<string> lines)
        {
            var hailstones = ParseLines(lines).ToList();

            var hailstonePairs = hailstones.Combinations(2);

            var count = 0;

            foreach (var pair in hailstonePairs)
            {
                if (true)
                {
                    count++;
                }
            }

            return count;
        }

        public object Part2(IList<string> lines)
        {


            return 0;
        }

        private static bool TryGetIntersection2D(Hail h1, Hail h2, out long X, out long Y)
        {
            X = 0;
            Y = 0;

            var h1_p1 = h1.Pos;
            var h1_p2 = h1.Pos + h1.Velocity;

            var h1_m = (h1_p2.Y - h1_p1.Y) / (double)(h1_p2.X - h1_p1.X);
            /* P1 = p1_0 + v1 * t, t > 0
             * P2 = p2_0 + v2 * t, t > 0
             *
             * P1 = P2 => p1_0 + v1 *t = 
             *
            */

            return false;
        }

        private static bool HailstoneWillIntersectArea2Dv2(Hail hail, long xyBoundLow, long xyBoundHigh)
        {
            var timeHailCrossesMinX = (xyBoundLow - hail.Pos.X) / hail.Velocity.X;
            var timeHailCrossesMaxX = (xyBoundHigh - hail.Pos.X) / hail.Velocity.X;

            if (timeHailCrossesMinX < 0 && timeHailCrossesMaxX < 0) return false;

            var timeHailCrossesMinY = (xyBoundLow - hail.Pos.Y) / hail.Velocity.Y;
            var timeHailCrossesMaxY = (xyBoundHigh - hail.Pos.Y) / hail.Velocity.Y;

            if (timeHailCrossesMinY < 0 && timeHailCrossesMaxY < 0) return false;

            var xTimeRange = new Range(timeHailCrossesMinX, timeHailCrossesMaxX);
            var yTimeRange = new Range(timeHailCrossesMinY, timeHailCrossesMaxY);

            return xTimeRange.Overlaps(yTimeRange);
        }

        private static bool HailstoneWillIntersectArea2D(Hail hail, long xyBoundLow, long xyBoundHigh)
        {
            var areaLines = new List<((long X, long Y) P1, (long X, long Y) P2)>
            {
                ((xyBoundLow, xyBoundLow), (xyBoundLow, xyBoundHigh)),
                ((xyBoundLow, xyBoundLow), (xyBoundHigh, xyBoundLow)),
                ((xyBoundLow, xyBoundHigh), (xyBoundHigh, xyBoundHigh)),
                ((xyBoundHigh, xyBoundLow), (xyBoundHigh, xyBoundHigh))
            };

            return false;
        }

        private static bool LinesWillIntersect(((long X, long Y) P1, (long X, long Y) P2) l1, ((long X, long Y) P1, (long X, long Y) P2) l2)
        {
            return false;
        }

        private IEnumerable<Hail> ParseLines(IList<string> lines)
        {
            foreach (var line in lines)
            {
                var nums = line.ReadAllNumbers<int>(new char[] { ',', ' '}).ToArray();

                yield return new Hail(Pos: new Vector3D(nums[0], nums[1], nums[2]), Velocity: new Vector3D(nums[3], nums[4], nums[5]));
            }
        }
    }
}
