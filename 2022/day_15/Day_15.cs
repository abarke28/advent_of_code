using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_15
{
    // https://adventofcode.com/2022/day/15
    public class Day_15
    {
        private class SensorBeaconPair
        {
            public Vector2D Sensor { get; set; }
            public Vector2D ClosestBeacon { get; set; }

            public int ManhattanDistance => Math.Abs(Sensor.X - ClosestBeacon.X) + Math.Abs(Sensor.Y - ClosestBeacon.Y);

            public SensorBeaconPair(int sensorX, int sensorY, int beaconX, int beaconY)
            {
                Sensor = new Vector2D(sensorX, sensorY);
                ClosestBeacon = new Vector2D(beaconX, beaconY);
            }

            public bool PointIsCoveredBySensor(Vector2D point)
            {
                return (Math.Abs(Sensor.X - point.X) + (Math.Abs(Sensor.Y - point.Y)) <= ManhattanDistance);
            }
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_15/input.txt");

            var sensorBeacons = ParseSensorBeaconPairs(lines);

            var row = 2_000_000;
            var points = ComputePointsThatCannotHaveBeaconForRow(row, sensorBeacons);

            Console.WriteLine(points.Count);

            //var minCoordinate = 0;
            //var maxCoordinate = 20;
            var minCoordinate = 0;
            var maxCoordinate = 4_000_000;

            var point = FindBeaconLocation2(minCoordinate, maxCoordinate, sensorBeacons);

            Console.WriteLine(point.ToString());
            Console.WriteLine(((long)point.X * (long)maxCoordinate) + (long)point.Y);
        }

        private static Vector2D FindBeaconLocation(int minCoordinate, int maxCoordinate, IEnumerable<SensorBeaconPair> sensorBeaconPairs)
        {
            var pointsList = GetCandidateBeaconPoints(minCoordinate, maxCoordinate, sensorBeaconPairs);

            var points = new HashSet<Vector2D>(pointsList.Sum(pl => pl.Count));

            foreach (var pointList in pointsList)
            {
                points.AddRange(pointList);
            }

            foreach (var point in points)
            {
                var currentEvasionCount = 0;

                foreach (var sb in sensorBeaconPairs)
                {
                    if (!sb.PointIsCoveredBySensor(point))
                    {
                        currentEvasionCount++;
                    }
                }

                if (currentEvasionCount == sensorBeaconPairs.Count())
                {
                    return point;
                }
            }

            throw new Exception("No point found");
        }

        private static Vector2D FindBeaconLocation2(int minCoordinate, int maxCoordinate, IEnumerable<SensorBeaconPair> sensorBeaconPairs)
        {
            var points = GetCandidateBeaconPoints(minCoordinate, maxCoordinate, sensorBeaconPairs);

            var candidatePoints = new HashSet<Vector2D>();

            for (var i = 0; i < points.Count; i++)
            {
                for (var j = 0; j < points.Count; j++)
                {
                    if (i != j)
                    {
                        Console.WriteLine($"Intersecting {i} & {j}");

                        var intersectPoints = points[i].Intersect(points[j]);

                        foreach (var intersection in intersectPoints)
                        {
                            candidatePoints.Add(intersection);
                        }
                    }
                }
            }

            foreach (var candidatePoint in candidatePoints)
            {
                var currentEvasionCount = 0;

                foreach (var sb in sensorBeaconPairs)
                {
                    if (!sb.PointIsCoveredBySensor(candidatePoint))
                    {
                        currentEvasionCount++;
                    }
                }

                if (currentEvasionCount == sensorBeaconPairs.Count())
                {
                    return candidatePoint;
                }
            }

            throw new Exception("No point found");
        }

        private static HashSet<Vector2D> ComputePointsThatCannotHaveBeaconForRow(int rowIndex, IEnumerable<SensorBeaconPair> sensorBeacons)
        {
            var minX = sensorBeacons.Select(sb => sb.Sensor.X - sb.ManhattanDistance).Min() - 1;
            var maxX = sensorBeacons.Select(sb => sb.Sensor.X + sb.ManhattanDistance).Max() + 1;

            var points = new HashSet<Vector2D>(maxX - minX);

            Console.WriteLine($"Computing for min x {minX} & max x {maxX}:");

            foreach (var sb in sensorBeacons)
            {
                var yDelta = Math.Abs(sb.Sensor.Y - rowIndex);
                var remainingXSensitivity = sb.ManhattanDistance - yDelta;

                if (remainingXSensitivity > 0)
                {
                    var startingX = sb.Sensor.X - remainingXSensitivity;
                    var endingX = sb.Sensor.X + remainingXSensitivity;

                    for (var i = startingX; i <= endingX; i++)
                    {
                        var candidatePoint = new Vector2D(i, rowIndex);

                        if (candidatePoint != sb.ClosestBeacon)
                        {
                            points.Add(candidatePoint);
                        }
                    }
                }
            }

            return points;
        }

        private static List<HashSet<Vector2D>> GetCandidateBeaconPoints(int minCoordinate, int maxCoordinate, IEnumerable<SensorBeaconPair> sensorBeacons)
        {
            var points = new List<HashSet<Vector2D>>(sensorBeacons.Count());

            foreach (var sb in sensorBeacons)
            {
                var currentPoints = new HashSet<Vector2D>();

                var minX = sb.Sensor.X - sb.ManhattanDistance - 1;
                var maxX = sb.Sensor.X + sb.ManhattanDistance + 1;

                var minY = sb.Sensor.Y - sb.ManhattanDistance - 1;
                var maxY = sb.Sensor.Y + sb.ManhattanDistance + 1;

                var top = new Vector2D(sb.Sensor.X, maxY);
                var left = new Vector2D(minX, sb.Sensor.Y);
                var bottom = new Vector2D(sb.Sensor.X, minY);
                var right = new Vector2D(maxX, sb.Sensor.Y);

                Func<Vector2D, bool> pointPredicate = p => minCoordinate <= p.X &&
                                                           minCoordinate <= p.Y &&
                                                           maxCoordinate >= p.X &&
                                                           maxCoordinate >= p.Y;

                currentPoints.AddRange(Vector2D.GetPointsBetween(top, left).Where(pointPredicate));
                currentPoints.AddRange(Vector2D.GetPointsBetween(left, bottom).Where(pointPredicate));
                currentPoints.AddRange(Vector2D.GetPointsBetween(bottom, right).Where(pointPredicate));
                currentPoints.AddRange(Vector2D.GetPointsBetween(right, top).Where(pointPredicate));

                points.Add(currentPoints);
            }

            return points;
        }

        private static IEnumerable<SensorBeaconPair> ParseSensorBeaconPairs(IEnumerable<string> lines)
        {
            var sensorBeacons = lines.Select(l => l.Split(',', ':', ' ', '=').SelectMany(ls => ls.ReadAllNumbers<int>()).ToList())
                                     .Select(l => new SensorBeaconPair(l[0], l[1], l[2], l[3]));

            return sensorBeacons;
        }
    }
}
