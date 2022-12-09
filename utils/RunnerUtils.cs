using aoc.common;
using System.Reflection;

namespace aoc.utils;

public static class RunnerUtils
{
    public static void RunProblem(string input)
    {
        const string methodName = "Solve";
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

        var problemSolution = Activator.CreateInstance(type);

        if (problemSolution is not ISolver solver)
        {
            Console.WriteLine("Problem solution does not follow expected pattern - it must implement ISolver. Exiting.");
            return;
        }

        var methodInfo = solver!.GetType().GetMethod(methodName);

        Console.WriteLine($"Running solver for problem {problem}:\n");
        methodInfo!.Invoke(solver, Array.Empty<object>());
    }

    private static bool TryParseProblemNumber(string input, out string problemString)
    {
        const int dayLength = 2;
        const char dayPadChar = '0';

        if (!int.TryParse(input, out var problem))
        {
            Console.WriteLine($"Could not parse input '{input}' into a problem number. Exiting.");

            problemString = string.Empty;
            return false;
        }

        var paddedProblem = problem.ToString().PadLeft(dayLength, dayPadChar);
        problemString = paddedProblem;

        return true;
    }
}
