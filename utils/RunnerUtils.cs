using aoc.common;
using System.Reflection;

namespace aoc.utils;

public static class RunnerUtils
{
    private const string DefaultYear = "2022";
    private const string MethodName = "Solve";
    private const string SolutionPath = "aoc.y{0}.day_{1}.Day_{1}";

    public static void RunProblem(string[] input)
    {
        if (!TryParseProblemNumber(input, out var problem, out var year))
        {
            return;
        }

        var fqTypeName = string.Format(SolutionPath, year, problem);

        var type = Assembly.GetExecutingAssembly().GetType(fqTypeName);

        if (type is null)
        {
            Console.WriteLine($"Could not locate problem '{year}-{problem}'. Exiting.");
            return;
        }

        var problemSolution = Activator.CreateInstance(type);

        if (problemSolution is not ISolver solver)
        {
            Console.WriteLine("Problem solution does not follow expected pattern - it must implement ISolver. Exiting.");
            return;
        }

        var methodInfo = solver!.GetType().GetMethod(MethodName);

        Console.WriteLine($"Running solver for problem {year}-{problem}:\n");
        methodInfo!.Invoke(solver, Array.Empty<object>());
    }

    private static bool TryParseProblemNumber(string[] input, out string problemString, out string yearString)
    {
        const int dayLength = 2;
        const char dayPadChar = '0';

        var inputCount = input.Length;
        var dayIndex = inputCount - 1;
        
        yearString = inputCount > 1 ? input[0] : DefaultYear;

        if (!int.TryParse(input[dayIndex], out var problem))
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
