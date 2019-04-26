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
            var buildList = builds.ToList();

            var db = new DatabaseManagerDataContext();
            db.Builds.InsertAllOnSubmit(buildList);
            db.SubmitChanges();

            Logger.Info($"Saved {buildList.Count} new builds to database");
        }

        public static void DeleteOldPaths()
        {
            var db = new DatabaseManagerDataContext();
            var buildsToDelete = db.Builds.ToList().Where(build =>
            {
                var days = 7;
                if (build.Type.Equals("Full", StringComparison.InvariantCultureIgnoreCase))
                    days = 14;

                return IsOlderThan(build.CreationDate ?? DateTime.Now.Date, days)
                       && !build.Mode.Equals("IP", StringComparison.CurrentCultureIgnoreCase);
            });

            var toDelete = buildsToDelete.ToList();
            db.Builds.DeleteAllOnSubmit(toDelete);
            db.SubmitChanges();

            Logger.Info($"deleted {toDelete.Count()} builds");
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

        public static void DeleteOldLogs()
        {
            var logs = Directory.GetFiles(@"..\logs\");

            var numOfDeletedLogs = 0;
            foreach (var log in logs)
            {
                if (IsOlderThan(Directory.GetCreationTime(log).Date, 2))
                {
                    numOfDeletedLogs++;
                    File.Delete(log);
                }
            }
            Logger.Info($"deleted {numOfDeletedLogs} old logs");

        }

        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static IEnumerable<string> DatabaseBuildIDs { get; set; }

        public static async Task Main()
        {
            try
            {
                Logger.Info("Starting Rekurencjon");

                DeleteOldLogs();

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
