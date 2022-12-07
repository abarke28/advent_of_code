using aoc.utils;

Console.WriteLine("Advent Of Code - 2022\n");

if (args.Any())
{
    var input = args[0];
    RunnerUtils.RunProblem(input);
}
else
{
    Console.WriteLine("No input provided, exiting. Try specifying a problem.");
    Console.WriteLine("Example: dotnet run 1");
}
