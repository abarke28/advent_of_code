using Aoc.Common;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Aoc.Utils;

public static class RunnerUtils
{
    private const string DefaultYear = "2023";
    private const string Part1Method = "Part1";
    private const string Part2Method = "Part2";
    private const string SolutionPath = "Aoc.Y{0}.Day_{1}.Day_{1}";
    private const string InputPath = "{0}/Day_{1}/input{2}.txt";
    private const string Example = "-Example";
    private const string NoFetch = "-NoFetch";

    public static async Task RunProblem(string[] input)
    {
        if (!TryParseProblemNumber(input, out var problem, out var year))
        {
            return;
        }

        if (!input.Any(i => i.Equals(NoFetch, StringComparison.OrdinalIgnoreCase)))
        {
            var json = File.ReadAllText("config.json");
            var config = JsonSerializer.Deserialize<AocConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            var fetcher = new InputFetcher(config);
            await fetcher.FetchInput(year, problem);
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

        var p1 = solver!.GetType().GetMethod(Part1Method);
        var p2 = solver!.GetType().GetMethod(Part2Method);

        var runExample = input.Any(i => i.Equals(Example, StringComparison.OrdinalIgnoreCase));
        var fileInput = string.Format(InputPath, year, problem, runExample ? "2" : string.Empty);
        var problemInput = FileUtils.ReadAllLines(fileInput);

        var stopWatch = Stopwatch.StartNew();
        var result1 = p1!.Invoke(solver, new object[] { problemInput });
        Console.WriteLine($"[{year}-{problem} Part 1] [{stopWatch.ElapsedMilliseconds} ms] - {result1?.ToString()}");

        stopWatch.Restart();
        var result2 = p2!.Invoke(solver, new object[] { problemInput });
        Console.WriteLine($"[{year}-{problem} Part 2] [{stopWatch.ElapsedMilliseconds} ms] - {result2?.ToString()}");
    }

    private static bool TryParseProblemNumber(string[] input, out string problemString, out string yearString)
    {
        const int dayLength = 2;
        const char dayPadChar = '0';

        var numberInput = input
            .Where(i => !i.Equals(Example, StringComparison.OrdinalIgnoreCase))
            .Where(i => !i.Equals(NoFetch, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var inputCount = numberInput.Length;
        var dayIndex = inputCount - 1;
        
        yearString = inputCount > 1 ? numberInput[0] : DefaultYear;

        if (!int.TryParse(numberInput[dayIndex], out var problem))
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
