using System.Numerics;

namespace aoc.utils
{
    public static class MathUtils
    {
        public static bool QuadraticHasRealSolutions(long a, long b, long c)
        {
            return (b * b - 4.0 * a * c) > 0;
        }

        public static (double Root1, double Root2) SolveQuadratic(long a, long b, long c)
        {
            var discriminant = (b * b) - (4.0 * a * c);

            if (discriminant < 0)
            {
                throw new ArgumentException("Quadratic does not have real solutions");
            }

            var root1 = (-b - Math.Sqrt(discriminant)) / (2.0 * a);
            var root2 = (-b + Math.Sqrt(discriminant)) / (2.0 * a);

            return (root1, root2);
        }

        public static (Complex Root1, Complex Root2) SolveQuadraticComplex(long a, long b, long c)
        {
            var discriminant = (b * b) - (4.0 * a * c);

            var root1 = (-b - Complex.Sqrt(discriminant)) / (2.0 * a);
            var root2 = (-b + Complex.Sqrt(discriminant)) / (2.0 * a);

            return (root1, root2);
        }
    }
}
