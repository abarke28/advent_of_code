using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_15
{
    // https://adventofcode.com/2022/day/15
    public class Day_15 : ISolver
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
            //var minCoordinate = 0;
            //var maxCoordinate = 4_000_000;

            //var point = FindBeaconLocation3(minCoordinate, maxCoordinate, sensorBeacons);

            //Console.WriteLine(point.ToString());
            //Console.WriteLine((point.X * maxCoordinate) + point.Y);

        }

        private static Vector2D FindBeaconLocation(int minCoordinate, int maxCoordinate, IEnumerable<SensorBeaconPair> sensorBeaconPairs)
        {
            for (var x = minCoordinate; x <= maxCoordinate; x++)
            {
                for (var y = minCoordinate; y <= maxCoordinate; y++)
                {
                    var candidatePoint = new Vector2D(x, y);
                    var evasionCount = 0;

                    foreach (var sb in sensorBeaconPairs)
                    {
                        if (sb.PointIsCoveredBySensor(candidatePoint))
                        {
                            break;
                        }

                        evasionCount++;
                    }

                    if (evasionCount == sensorBeaconPairs.Count())
                    {
                        return candidatePoint;
                    }
                }
            }

            throw new Exception("No point found");
        }

        private static Vector2D FindBeaconLocation2(int minCoordinate, int maxCoordinate, IEnumerable<SensorBeaconPair> sensorBeaconPairs)
        {
            var points = new HashSet<Vector2D>();

            foreach (var sb in sensorBeaconPairs)
            {
                var minX = sb.Sensor.X - sb.ManhattanDistance - 1;
                var maxX = sb.Sensor.X + sb.ManhattanDistance + 1;

                var minY = sb.Sensor.Y - sb.ManhattanDistance - 1;
                var maxY = sb.Sensor.Y + sb.ManhattanDistance + 1;

                for (var x = Math.Max(minX, minCoordinate); x <= Math.Min(maxX, maxCoordinate); x++)
                {
                    for (var y = Math.Max(minY, minCoordinate); y <= Math.Min(maxY, maxCoordinate); y++)
                    {
                        var v = new Vector2D(x, y);

                        if (Vector2D.ManhattanDistance(sb.Sensor, v) == (sb.ManhattanDistance + 1))
                        {
                            points.Add(v);
                        }
                    }
                }
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

        private static Vector2D FindBeaconLocation3(int minCoordinate, int maxCoordinate, IEnumerable<SensorBeaconPair> sensorBeaconPairs)
        {
            var points = new List<List<Vector2D>>(sensorBeaconPairs.Count());

            foreach (var sb in sensorBeaconPairs)
            {
                var currentPoints = new HashSet<Vector2D>();

                var minX = sb.Sensor.X - sb.ManhattanDistance - 1;
                var maxX = sb.Sensor.X + sb.ManhattanDistance + 1;

                var minY = sb.Sensor.Y - sb.ManhattanDistance - 1;
                var maxY = sb.Sensor.Y + sb.ManhattanDistance + 1;

                for (var x = Math.Max(minX, minCoordinate); x <= Math.Min(maxX, maxCoordinate); x++)
                {
                    for (var y = Math.Max(minY, minCoordinate); y <= Math.Min(maxY, maxCoordinate); y++)
                    {
                        var v = new Vector2D(x, y);

                        if (Vector2D.ManhattanDistance(sb.Sensor, v) == (sb.ManhattanDistance + 1))
                        {
                            currentPoints.Add(v);
                        }
                    }
                }

                points.Add(currentPoints.ToList());
            }

            var candidatePoints = new HashSet<Vector2D>();

            for (var i = 0; i < points.Count(); i++)
            {
                for (var j = 0; j < points.Count(); j++)
                {
                    if (i != j)
                    {
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

        private static IEnumerable<SensorBeaconPair> ParseSensorBeaconPairs(IEnumerable<string> lines)
        {
            var sensorBeacons = lines.Select(l => l.Split(',', ':', ' ', '=').SelectMany(ls => ls.ReadAllNumbers()).ToList())
                                     .Select(l => new SensorBeaconPair(l[0], l[1], l[2], l[3]));

            return sensorBeacons;
        }
    }
}
