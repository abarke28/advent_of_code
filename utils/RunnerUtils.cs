using System.Reflection;

namespace aoc.utils;

public static class RunnerUtils
{
    public static void RunProblem(string input)
    {
        const string methodName = "GetResult";
        const string solutionPath = "aoc.day_{0}.Day_{0}";

        if (!TryParseProblemNumber(input, out var problem))
        {
            return;
        }

        var fqTypeName = string.Format(solutionPath, problem);

        var type = Assembly.GetExecutingAssembly().GetType(fqTypeName);

        if (type is null)
        {
            Console.WriteLine($"Could not locate problem '{problem}'. Exiting.");
            return;
        }

        var solver = Activator.CreateInstance(type);

        var methodInfo = solver!.GetType().GetMethod(methodName);
        methodInfo!.Invoke(solver, Array.Empty<object>());
    }

    private static bool TryParseProblemNumber(string input, out string problemString)
    {
        if (!int.TryParse(input, out var problem))
        {
            Console.WriteLine($"Could not parse input '{input}' into a problem number. Exiting.");

            problemString = string.Empty;
            return false;
        }

        var paddedProblem = problem.ToString().PadLeft(2, '0');
        problemString = paddedProblem;

        return true;
    }
}
