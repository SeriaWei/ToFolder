using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ToFolder
{
    class Program
    {
        private static Regex NameRegex = new Regex(@"_(\d{4})(\d{2})(\d{2})_", RegexOptions.Compiled);
        public static void Main(string[] args)
        {
            DirectoryInfo rootFolder;
            if (args.Length == 1)
            {
                rootFolder = new DirectoryInfo(args[0]);
            }
            else
            {
                rootFolder = new DirectoryInfo(Directory.GetCurrentDirectory());
            }
            var files = rootFolder.GetFiles("*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string year = file.LastWriteTime.Year.ToString();

                var matchResult = NameRegex.Match(file.Name);
                if (matchResult.Success)
                {
                    year = matchResult.Groups[1].Value;
                }
                if (IsInFolder(file, year)) continue;

                var yearFolder = rootFolder.CreateSubdirectory(year);
                string targetFileName = Path.Combine(yearFolder.FullName, file.Name);
                if (!File.Exists(targetFileName))
                {
                    Console.WriteLine("Move to: {0}", targetFileName);
                    file.MoveTo(targetFileName);
                }
                else
                {
                    Console.WriteLine("Skip: {0}", file.FullName);
                }
            }
        }
        static bool IsInFolder(FileInfo file, string year)
        {
            return file.DirectoryName.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).Contains(year);
        }
    }
}
