param(
    [Parameter(Mandatory=$true)]
	[string]$year,
	[Parameter(Mandatory=$true)]
	[string]$day
)

Write-Output "Generating files for problem $year-$day...`n"

$projectPath = "C:\git\aoc"
$folderName = "day_$day"
$className = "Day_$day.cs"
$inputName = "input.txt"
$input2Name = "input2.txt"

Push-Location $projectPath

$yearExists = Test-Path $year
if (!$yearExists){
    New-Item -Path $year -ItemType Directory | Out-Null
}

Push-Location $year

if (Test-Path $folderName){
    Write-Output "Directory for $day already exists, exiting..."
    
    Pop-Location
    Exit -1
}

New-Item -Path $folderName -ItemType Directory | Out-Null

Push-Location $folderName

New-Item -Name $inputName | Out-Null
New-Item -Name $input2Name | Out-Null
New-Item -Name $className -Value `
"using aoc.common;
using aoc.utils;

namespace aoc.y$year.day_$day
{
    // https://adventofcode.com/$year/day/$day
    public class Day_$day : ISolver
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines(`"$year/day_$day/input.txt`");

            Console.WriteLine(`"Hello World`");
        }
    }
}
" | Out-Null

Write-Output "Generated files:"

Get-ChildItem

Pop-Location
Pop-Location
Pop-Location
