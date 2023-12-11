using Aoc.Utils;

Console.WriteLine("Advent Of Code\n");

if (args.Any())
{
    await RunnerUtils.RunProblem(args);
}
else
{
    Console.WriteLine("No input provided, exiting. Try specifying a problem.");
    Console.WriteLine("Example: dotnet run 2022 7\n");
}

Console.WriteLine();
