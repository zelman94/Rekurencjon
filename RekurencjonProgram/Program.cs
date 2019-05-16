using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Rekurencjon
{
    internal class Program
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        public static async Task<bool> ExistInDatabase(string buildPath)
        {
            var (success, buildId) = await BuildsFactory.TryGet("BuildID", buildPath);

            if (success)
            {
                return databaseMng.ExistInDatabase(buildId);
            }

            return true;
        }

        public static void DeleteOldLogs()
        {
            var logs = Directory.GetFiles(@"..\logs\");
            var max_days = 2;

            var numOfDeletedLogs = 0;
            foreach (var log in logs)
            {
                if (Directory.GetCreationTime(log).Date.IsOlderThan(max_days))
                {
                    numOfDeletedLogs++;
                    File.Delete(log);
                }
            }
            Logger.Info($"deleted {numOfDeletedLogs} old logs");

        }

        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static DatabaseManagerDataContext databaseMng = new DatabaseManagerDataContext();

        public static async Task Main()
        {
            try
            {
                Logger.Info("Starting Rekurencjon");

                DeleteOldLogs();

                var performanceTimer = Stopwatch.StartNew();
                Logger.Info("Started performance timer");

                DatabaseManagerDataContext databaseMng = new DatabaseManagerDataContext();
                databaseMng.GetBuildIDFromDatabase();

                databaseMng.DeleteOldPaths();
                databaseMng.DeleteNotExistingBuild();

                var pathFinder = new PathFinder();
                var paths = await pathFinder.GetAllPathsAsync();

                var factory = new BuildsFactory();
                var builds = await factory.GetAllBuilds(paths);

                databaseMng.SaveBuildsToDatabase(builds);

                performanceTimer.Stop();
                Logger.Info("Stop timer");
                Logger.Info($"Finished in {performanceTimer.Elapsed}");
            }
            catch (Exception e)
            {
                Logger.Error(e.FullMessage());
            }

            Console.ReadKey();
        }
    }
}
