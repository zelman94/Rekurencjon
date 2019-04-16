using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rekurencjon
{
    internal class Program
    {
        private const string GENERAL_PATH = @"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix";
        private const string PATH_TO_RELEASED_FSW = @"\\demant.com\data\KBN\RnD\FS_Programs\Fitting Applications\";

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        private static PathsAndDirs GetListOfFullPaths(string rootPath)
        {
            var IPs_paths = new List<string>();
            var directoriesFull = new List<string>();
            var directoriesMedium = new List<string>();
            try
            {
                if (Directory.Exists(rootPath))
                {
                    IPs_paths = Directory.GetDirectories(rootPath).ToList();

                    foreach (var psPath in IPs_paths)
                    {
                        var paths = Directory.GetDirectories(psPath).ToList();
                        Console.WriteLine(psPath);
                    }

                    directoriesFull = Directory.GetFiles(rootPath, "setup.exe", SearchOption.AllDirectories).ToList();
                    var EXEs = Directory.GetFiles(rootPath, "*.exe", SearchOption.AllDirectories).ToList();
                    directoriesMedium = EXEs.FindAll(s => s.Contains("Medium"));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString() + " \nat: " + rootPath);
                Console.ReadKey();
            }
            directoriesFull.AddRange(directoriesMedium);

            return new PathsAndDirs { Paths = IPs_paths, Dirs = directoriesFull };
        }

        public static void SaveBuildsToDatabase(IEnumerable<Build> builds)
        {
            var db = new DatabaseManagerDataContext();
            db.Builds.InsertAllOnSubmit(builds);
            db.SubmitChanges();
        }

        public static IEnumerable<string> GetAllPreReleasedInstallers()
        {
            var dirs = Directory.GetDirectories(GENERAL_PATH);
            return dirs.Where(i => i.Contains("\\FullInstaller") && IsBuildNew(i));
        }

        public static bool IsOlderThan14Days(DateTime date)
        {
            if (date.AddDays(14).CompareTo(DateTime.Now.Date) < 0)
                return true;
            return false;
        }

        public static bool IsBuildNew(string pathToCheck)
        {
            var directoryInfo = new DirectoryInfo(pathToCheck);
            var dateOfLastModify = directoryInfo.LastWriteTime.Date;

            return !IsOlderThan14Days(dateOfLastModify);
        }

        public static IEnumerable<string> GetFullBuildsPaths()
        {
            return GetAllPreReleasedInstallers().SelectMany(Directory.GetDirectories)
                .Where(i => (i.Contains("rc") || i.Contains("master") || i.Contains("IP")) && IsBuildNew(i));
        }

        public static async Task<IEnumerable<string>> GetSetupPathsAsync(string buildPath)
        {
            return await Task.Run(() => Directory.GetFiles(buildPath, "setup.exe", SearchOption.AllDirectories));
        }

        public static bool TryGetAboutInfo(string buildPath, out string aboutInfo)
        {
            var charsToTrim = new[] {'-', '_'};
            var match = Regex.Match(buildPath, @"_\d*\.?\d*\.?\d*\.?\d*\-");
            aboutInfo = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);
            return match.Success;
        }

        public static IEnumerable<string> GetAboutInfosFromDatabase()
        {
            var db = new DatabaseManagerDataContext();
            return db.Builds.Select(b => b.About);
        }

        public static string GetBrand(string path)
        {
            var brands = new List<string>() {"Genie", "GenieMedical", "HearSuite", "Oasis", "ExpressFit"};
            foreach (var brand in brands)
            {
                if (path.Contains(brand))
                    return brand;
            }

            return "";
        }

        public static string GetOem(string path)
        {
            var parts = path.Split('\\');
            return parts[parts.Length - 2];
        }

        public static string GetType(string path)
        {
            if (path.Contains("Nightly"))
            {
                return "NIGHTLY";
            }
            return "FULL";
        }

        public static bool TryGetRelease(string path, out string release)
        {
            var charsToTrim = new[] { '-', '_' };
            var match = Regex.Match(path, @"-\d{2}.\d{1}-");
            release = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);
            return match.Success;
        }

        public static bool TryGetMode(string path, out string mode)
        {
            var charsToTrim = new[] { '-', '_' };
            var match = Regex.Match(path, @"(rc|master|IP\d*)");
            mode = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);
            return match.Success;
        }

        public static bool TryCreateBuild(string path, out Build newBuild)
        {
            newBuild = null;
            if (TryGetAboutInfo(path, out var aboutInfo)
                 && TryGetRelease(path, out var release)
                 && TryGetMode(path, out var mode))
            {
                newBuild = new Build()
                {
                    Type = GetType(path),
                    Release = release,
                    Mode = mode,
                    About = aboutInfo,
                    Brand = GetBrand(path),
                    Oem = GetOem(path),
                    Path = path,
                    CreationDate = Directory.GetCreationTime(path).Date
                };
                return true;
            }

            return false;
        }

        public static bool ExistInDatabase(string buildPath)
        {
            var buildsInDatabase = GetAboutInfosFromDatabase();
            if (TryGetAboutInfo(buildPath, out var aboutInfo))
            {
                if (!buildsInDatabase.Any(i => i.Contains(aboutInfo)))
                    return false;
            }

            return true;
        }

        public static IEnumerable<Build> GetAllBuilds(IEnumerable<string> buildPaths)
        {
            var counter = 0;
            var builds = new List<Build>();
            foreach (var path in buildPaths)
            {
                if (TryCreateBuild(path, out var build))
                {
                    builds.Add(build);
                    counter++;
                }
            }

            Console.WriteLine("NUmber of builds: " + counter);
            return builds;
        }

        public static async Task<IEnumerable<string>> GetAllPathsAsync()
        {
            var buildPaths = GetFullBuildsPaths();

            return (await Task.WhenAll(buildPaths.Select(async path =>
            {
                var setupPaths = Enumerable.Empty<string>();
                if (!ExistInDatabase(path))
                    setupPaths = await GetSetupPathsAsync(path);
                return setupPaths;
            }))).SelectMany(path => path);
        }

        public static void DeleteOldPaths()
        {
            Console.WriteLine("INFO: Deleting old builds: ");

            var db = new DatabaseManagerDataContext();
            var buildsToDelete = db.Builds.ToList().Where(build => IsOlderThan14Days(build.CreationDate ?? DateTime.Now.Date));

            db.Builds.DeleteAllOnSubmit(buildsToDelete);
            db.SubmitChanges();

            Console.WriteLine($"{buildsToDelete.Count()} deleted");
        }

        public static void DeleteNotExistingBuild()
        {
            Console.WriteLine("INFO: Deleting not existing builds: ");

            var db = new DatabaseManagerDataContext();
            var buildsToDelete = db.Builds.Where(b => !File.Exists(b.Path));
            db.SubmitChanges();

            Console.Write($"{buildsToDelete.Count()} deleted");
        }

        public static async Task Main()
        {
            try
            {
                DeleteOldPaths();
                DeleteNotExistingBuild();
                var stopwatch = Stopwatch.StartNew();

                Console.WriteLine("Start application");
                var paths = await GetAllPathsAsync();
                Console.WriteLine("Get builds");
                var builds = GetAllBuilds(paths);
                Console.WriteLine("Save to database");

                SaveBuildsToDatabase(builds);

                stopwatch.Stop();
                Console.WriteLine("Finished in: " + stopwatch.Elapsed);
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
   

        }
    }
}
