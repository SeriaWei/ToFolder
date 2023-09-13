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
                DateTime createdOn = file.LastWriteTime;

                var matchResult = NameRegex.Match(file.Name);
                if (matchResult.Success)
                {
                    createdOn = new DateTime(int.Parse(matchResult.Groups[1].Value), int.Parse(matchResult.Groups[2].Value), int.Parse(matchResult.Groups[3].Value));
                }
                string yearMonth = Path.Combine(createdOn.ToString("yyyy"), createdOn.ToString("MMM"));
                var subdirectory = rootFolder.CreateSubdirectory(yearMonth);
                if (IsInFolder(file, subdirectory.FullName)) continue;


                string targetFileName = Path.Combine(subdirectory.FullName, file.Name);
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
            return file.DirectoryName.StartsWith(year);
        }
    }
}
