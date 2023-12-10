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

        public static T GCD<T>(T a, T b) where T : INumber<T>
        {
            while (b != T.Zero)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }

            return a;
        }

        public static T GCD<T>(params T[] numbers) where T : INumber<T>
        {
            if (numbers == null || numbers.Length == 0) return T.Zero;

            var result = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                result = GCD(result, numbers[i]);
            }

            return result;
        }

        public static T LCM<T>(T a, T b) where T : INumber<T>
        {
            return (a / GCD(a, b)) * b;
        }

        public static T LCM<T>(params T[] numbers) where T : INumber<T>
        {
            if (numbers == null || numbers.Length == 0) return T.Zero;
            if (numbers.Length == 1) return numbers[0];

            var result = numbers[0];

            for (int i = 0; i < numbers.Length; i++)
            {
                result = LCM(result, numbers[i]);
            }

            return result;
        }

        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> sequences) where T : INumber<T>
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>()};

            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) => accumulator.SelectMany(accSeq => sequence,
                                                                  (accSeq, item) => accSeq.Concat(new[] { item })));
        }

        public static bool IsCoprime<T>(params T[] numbers) where T : INumber<T>
        {
            return GCD(numbers) == T.One;
        }
    }
}
