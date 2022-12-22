using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_18
{
    // https://adventofcode.com/2022/day/18
    public class Day_18 : ISolver
    {
        private const int SidesPerCube = 6;

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_18/input.txt");
            var cubes = ParseInput(lines);
            var cubeHashSet = cubes.ToHashSet();

            var totalFaceScore = ComputeExposedFaceScore(cubeHashSet);
            Console.WriteLine(totalFaceScore);

            var exteriorFaceScore = ComputeExteriorFaceScore(cubeHashSet);
            Console.WriteLine(exteriorFaceScore);
        }

        private static int ComputeExteriorFaceScore(ISet<Vector3D> cubeHashSet)
        {
            var knownExteriorPoints = new HashSet<Vector3D>();
            var totalExteriorScore = 0;

            foreach (var cube in cubeHashSet)
            {
                var cubeExteriorScore = 0;
                var neighbors = Compute6Neighbors(cube);

                foreach (var n in neighbors)
                {
                    if (IsExterior(n, cubeHashSet, knownExteriorPoints))
                    {
                        knownExteriorPoints.Add(n);
                        cubeExteriorScore++;
                    }
                }

                Console.WriteLine($"Cube {cube} has {cubeExteriorScore} exterior faces.");
                totalExteriorScore += cubeExteriorScore;
            }

            return totalExteriorScore;
        }

        private static bool IsExterior(Vector3D v, ISet<Vector3D> cubes, ISet<Vector3D> confirmedExteriorCubes)
        {
            var maxX = cubes.Max(v => v.X) + 1;
            var minX = cubes.Min(v => v.X) - 1;

            var maxY = cubes.Max(v => v.Y) + 1;
            var minY = cubes.Min(v => v.Y) - 1;

            var maxZ = cubes.Max(v => v.Z) + 1;
            var minZ = cubes.Min(v => v.Z) - 1;

            if (cubes.Contains(v))
            {
                return false;
            }

            var alreadyChecked = new HashSet<Vector3D>();
            var toCheck = new Stack<Vector3D>();
            toCheck.Push(v);

            while (toCheck.Count > 0)
            {
                var candidate = toCheck.Pop();

                if (alreadyChecked.Contains(candidate))
                {
                    continue;
                }

                alreadyChecked.Add(candidate);

                if (confirmedExteriorCubes.Contains(candidate))
                {
                    confirmedExteriorCubes.AddRange(alreadyChecked.Except(cubes));

                    return true;
                }

                if (candidate.X >= maxX ||
                    candidate.X <= minX ||
                    candidate.Y >= maxY ||
                    candidate.Y <= minY ||
                    candidate.Z >= maxZ ||
                    candidate.Z <= minZ)
                {
                    confirmedExteriorCubes.Add(candidate);
                    confirmedExteriorCubes.AddRange(alreadyChecked.Except(cubes));

                    return true;
                }

                if (!cubes.Contains(candidate))
                {
                    var neighbors = Compute6Neighbors(candidate);

                    foreach(var neighbor in neighbors)
                    {
                        toCheck.Push(neighbor);
                    }
                }
            }

            return false;
        }

        private static int ComputeExposedFaceScore(ISet<Vector3D> cubeHashSet)
        {
            var totalFaceScore = 0;

            foreach (var cube in cubeHashSet)
            {
                var faceScore = SidesPerCube;

                var faceVectors = Compute6Neighbors(cube);

                foreach (var faceVector in faceVectors)
                {
                    if (cubeHashSet.Contains(faceVector))
                    {
                        faceScore--;
                    }
                }

                Console.WriteLine($"Cube {cube} has {faceScore} exposed faces.");
                totalFaceScore += faceScore;
            }

            return totalFaceScore;
        }

        private static IEnumerable<Vector3D> Compute6Neighbors(Vector3D v)
        {
            return new List<Vector3D>
            {
                v + Vector3D.XHat,
                v + Vector3D.YHat,
                v + Vector3D.ZHat,
                v - Vector3D.XHat,
                v - Vector3D.YHat,
                v - Vector3D.ZHat
            };
        }

        private static IList<Vector3D> ParseInput(IEnumerable<string> lines)
        {
            return lines.Select(l => l.ReadAllNumbers(',').ToList())
                        .Select(ns => new Vector3D(ns[0], ns[1], ns[2]))
                        .ToList();
        }
    }
}
