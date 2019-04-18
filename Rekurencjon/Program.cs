using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net.Config;

namespace Rekurencjon
{
    internal class Program
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        public static bool IsOlderThan(DateTime date, int days)
        {
            if (date.AddDays(days).CompareTo(DateTime.Now.Date) < 0)
                return true;
            return false;
        }

        public static void SaveBuildsToDatabase(IEnumerable<Build> builds)
        {
            var db = new DatabaseManagerDataContext();
            db.Builds.InsertAllOnSubmit(builds);
            db.SubmitChanges();

            Logger.Info($"Saved {builds.ToList().Count} new builds to database");
        }

        public static void DeleteOldPaths()
        {
            var db = new DatabaseManagerDataContext();
            var buildsToDelete = db.Builds.ToList().Where(build => IsOlderThan(build.CreationDate ?? DateTime.Now.Date, 14));

            db.Builds.DeleteAllOnSubmit(buildsToDelete);
            db.SubmitChanges();

            Logger.Info($"deleted {buildsToDelete.Count()} builds");
        }

        public static void DeleteNotExistingBuild()
        {
            var db = new DatabaseManagerDataContext();
            var buildsToDelete = db.Builds.ToList().Where(b => !File.Exists(b.Path));
            db.SubmitChanges();

            Logger.Info($"deleted {buildsToDelete.Count()} builds");
        }

        public static IEnumerable<string> GetBuildIDFromDatabase()
        {
            var db = new DatabaseManagerDataContext();
            return db.Builds.Select(b => b.BuildID).ToList();
        }

        public static async Task<bool> ExistInDatabase(string buildPath)
        {
            var (success, buildId) = await BuildsFactory.TryGetBuildID(buildPath);

            if (success)
            {
                if (!DatabaseBuildIDs.Any(i => i.Contains(buildId)))
                {
                    return false;
                }
            }

            return true;
        }

        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static IEnumerable<string> DatabaseBuildIDs { get; set; }

        public static async Task Main()
        {
            try
            {
                Logger.Info("Starting Rekurencjon");

                var performanceTimer = Stopwatch.StartNew();
                Logger.Info("Started performance timer");

                DatabaseBuildIDs = GetBuildIDFromDatabase();

                DeleteOldPaths();
                DeleteNotExistingBuild();

                var pathFinder = new PathFinder();
                var paths = await pathFinder.GetAllPathsAsync();

                var factory = new BuildsFactory();
                var builds = await factory.GetAllBuilds(paths);

                SaveBuildsToDatabase(builds);

                performanceTimer.Stop();
                Logger.Info("Stop timer");
                Logger.Info($"Finished in {performanceTimer.Elapsed}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            Console.ReadKey();
        }
    }
}
