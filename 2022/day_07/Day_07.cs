using aoc.common;
using aoc.utils;

namespace aoc.y2022.day_07
{
    // https://adventofcode.com/2022/day/7
    public class Day_07 : ISolver
    {
        private const string Command = "$";
        private const string Ls = "ls";
        private const string Cd = "cd";
        private const string Back = "..";
        private const string Dir = "dir";

        private const int SizeThreshold = 100_000;
        private const int FileSystemVolume = 70_000_000;
        private const int NeededSpace = 30_000_000;

        private class ElfFile
        {
            public string Name { get; set; } = string.Empty;
            public int Size { get; set; }
        }

        private class ElfDirectory
        {
            public string Name { get; set; } = string.Empty;
            public Dictionary<string, ElfDirectory> Directories { get; set; } = new Dictionary<string, ElfDirectory>();
            public Dictionary<string, ElfFile> Files { get; set; } = new Dictionary<string, ElfFile>();

            public int GetSize()
            {
                return Files.Sum(f => f.Value.Size) + Directories.Sum(d => d.Value.GetSize());
            }

            public List<ElfDirectory> GetDirectories()
            {
                var directories = new List<ElfDirectory>();

                directories.Add(this);

                foreach (var childDir in Directories)
                {
                    directories.AddRange(childDir.Value.GetDirectories());
                }

                return directories;
            }
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_07/input.txt");

            var fileSystem = BuildFileSystem(lines);

            var directories = fileSystem.GetDirectories();

            var smallDirs = directories.Where(d => d.GetSize() <= SizeThreshold);

            Console.WriteLine($"Total size of directories with max size of {SizeThreshold} is {smallDirs.Sum(ld => ld.GetSize())}");

            var currentUsage = fileSystem.GetSize();
            var currentSpace = FileSystemVolume - currentUsage;
            var spaceToDelete = NeededSpace - currentSpace;

            var dirSizeToDelete = directories.Select(d => d.GetSize()).Where(s => s >= spaceToDelete).OrderBy(s => s).First();

            Console.WriteLine($"The directory to delete to free up {spaceToDelete} is of size {dirSizeToDelete}");
        }

        private static ElfDirectory BuildFileSystem(List<string> lines)
        {
            const int dollarIndex = 0;
            const int dirIndex = 0;
            const int sizeIndex = 0;
            const int commandIndex = 1;
            const int nameIndex = 1;
            const int targetIndex = 2;

            var fileSystem = new ElfDirectory
            {
                Name = "/"
            };

            var foldersVisited = new Stack<ElfDirectory>();
            foldersVisited.Push(fileSystem);

            for (var i = 1; i < lines.Count; i++)
            {
                var currentLine = lines[i];
                var words = currentLine.Split(' ');

                var currentFolder = foldersVisited.Peek();

                if (words[dollarIndex] == Command)
                {
                    var commandWord = words[commandIndex];

                    switch (commandWord)
                    {
                        case Ls:
                            continue;

                        case Cd:
                            var target = words[targetIndex];

                            if (target == Back)
                            {
                                foldersVisited.Pop();
                            }
                            else
                            {
                                foldersVisited.Push(currentFolder.Directories[target]);
                            }

                            continue;
                    }
                }
                else
                {
                    var listing = currentLine.Split(' ');

                    if (listing[dirIndex] == Dir)
                    {
                        var newFolderName = listing[nameIndex];

                        currentFolder.Directories.Add(newFolderName, new ElfDirectory { Name = newFolderName });
                    }
                    else
                    {
                        var newFileSize = int.Parse(listing[sizeIndex]);
                        var newFileName = listing[nameIndex];

                        currentFolder.Files.Add(newFileName, new ElfFile { Name = newFileName, Size = newFileSize });
                    }

                    continue;
                }
            }

            return fileSystem;
        }
    }
}
