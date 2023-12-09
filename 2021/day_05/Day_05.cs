using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2021.day_05
{
    // https://adventofcode.com/2021/day/05
    public class Day_05
    {
        private class Vector2DPair
        {
            public Vector2D V1 { get; set; }
            public Vector2D V2 { get; set; }
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_05/input.txt");

            ParseVectors(lines[0]);
            var vectorPairs = lines.Select(l => ParseVectors(l));

            var points = vectorPairs.SelectMany(vp => ComputePointsFromVectorPair(vp));

            var pointsVisited = ComputePointsOverlap(points.ToList());

            var duplicatePoints = pointsVisited.Where(kvp => kvp.Value > 1);

            Console.WriteLine(duplicatePoints.Count());

            var points2 = vectorPairs.SelectMany(vp => ComputePointsFromVectorPairV2(vp));
            points2.ToList().Sort();

            var pointsVisited2 = ComputePointsOverlap(points2.ToList());

            var duplicatePoints2 = pointsVisited2.Where(kvp => kvp.Value > 1);

            Console.WriteLine(duplicatePoints2.Count());
        }

        private static Dictionary<Vector2D, int> ComputePointsOverlap(IList<Vector2D> points)
        {
            var pointsEcountered = new Dictionary<Vector2D, int>();

            foreach(var point in points)
            {
                if (!pointsEcountered.TryAdd(point, 1))
                {
                    pointsEcountered[point] += 1;
                }
            }

            return pointsEcountered;
        }

        private static List<Vector2D> ComputePointsFromVectorPair(Vector2DPair pair)
        {
            var points = new List<Vector2D>();

            var v1 = pair.V1;
            var v2 = pair.V2;

            if (v1 == v2)
            {
                points.Add(v1);
                return points;
            }

            if (v1.X == v2.X)
            {
                points.Add(v1);
                points.Add(v2);

                var yDeltaSign = Math.Sign(v2.Y - v1.Y);

                var x = v1.X;
                var y = v1.Y + yDeltaSign;


                while (y != v2.Y)
                {
                    points.Add(new Vector2D(x, y));

                    y += yDeltaSign;
                }
            }

            if (v1.Y == v2.Y)
            {
                points.Add(v1);
                points.Add(v2);

                var xDeltaSign = Math.Sign(v2.X - v1.X);

                var x = v1.X + xDeltaSign;
                var y = v1.Y;

                while (x != v2.X)
                {
                    points.Add(new Vector2D(x, y));

                    x += xDeltaSign;
                }
            }

            return points;
        }

        private static List<Vector2D> ComputePointsFromVectorPairV2(Vector2DPair pair)
        {
            var points = new List<Vector2D>();

            var v1 = pair.V1;
            var v2 = pair.V2;

            if (v1 == v2)
            {
                points.Add(v1);
                return points;
            }

            points.Add(v1);
            points.Add(v2);

            var xDeltaSign = Math.Sign(v2.X - v1.X);
            var yDeltaSign = Math.Sign(v2.Y - v1.Y);

            var x = v1.X + xDeltaSign;
            var y = v1.Y + yDeltaSign;

            while (x != v2.X || y != v2.Y)
            {
                points.Add(new Vector2D(x, y));

                x += xDeltaSign;
                y += yDeltaSign;
            }

            return points;
        }

        private static Vector2DPair ParseVectors(string s)
        {
            var words = s.GetWords();
            var coords = words.SelectMany(w => w.ReadAllNumbers<int>(',')).ToList();

            return new Vector2DPair
            {
                V1 = new Vector2D(coords[0], coords[1]),
                V2 = new Vector2D(coords[2], coords[3])
            };
        }
    }
}
