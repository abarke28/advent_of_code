param(
	[Parameter()]
	[string]$day
)

Write-Output "Generating files for day $day..."

$folderName = "day_$day"
$className = "Day_$day.cs"
$inputName = "input.txt"

New-Item -Path $folderName -ItemType Directory | Out-Null

Push-Location $folderName

New-Item -Name $inputName | Out-Null
New-Item -Name $className -Value `
"using aoc.common;
using aoc.utils;

namespace aoc.day_$day
{
    // https://adventofcode.com/2022/day/$day
    public class Day_$day : ISolver
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines(`"day_$day/input.txt`");

            Console.WriteLine(`"Hello World`");
        }
    }
}
" | Out-Null

Write-Output "Generated files:"

Get-ChildItem

Pop-Location
