using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rekurencjon
{
    internal class PathFinder
    {
        private const string GENERAL_PATH = @"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix";
        private const string PATH_TO_RELEASED_FSW = @"\\demant.com\data\KBN\RnD\FS_Programs\Fitting Applications\";

        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool PathIsNightly(string path)
        {
            return path.Contains("\\Nightly");
        }

        private bool IsBuildNew(string pathToCheck)
        {
            var directoryInfo = new DirectoryInfo(pathToCheck);
            var dateOfLastModify = directoryInfo.LastWriteTime.Date;

            if (PathIsNightly(pathToCheck))
                return !Program.IsOlderThan(dateOfLastModify, 7);

            return !Program.IsOlderThan(dateOfLastModify, 14);
        }

        private async Task<IEnumerable<string>> GetFullPathsAsync(string buildPath)
        {
            return await Task.Run(() => Directory.GetFiles(buildPath, "setup.exe", SearchOption.AllDirectories));
        }

        private async Task<IEnumerable<string>> GetNightlyPathsAsync(string buildPath)
        {
            return await Task.Run(() => Directory.GetFiles(buildPath, "*.exe", SearchOption.AllDirectories));
        }

        private async Task<IEnumerable<string>> GetInstallers(string buildPath)
        {
            if (buildPath.Contains("\\Nightly"))
            {
                return await GetNightlyPathsAsync(buildPath);
            }

            return await GetFullPathsAsync(buildPath);
        }

        private IEnumerable<string> GetFullBuildsPaths()
        {
            return GetAllPreReleasedInstallers().SelectMany(Directory.GetDirectories)
                .Where(i => (i.Contains("rc") || i.Contains("master") || i.Contains("IP")) && IsBuildNew(i));
        }

        private IEnumerable<string> GetAllPreReleasedInstallers()
        {
            var dirs = Directory.GetDirectories(GENERAL_PATH);
            return dirs.Where(i => ( i.Contains("\\FullInstaller") || PathIsNightly(i) )
                                   && IsBuildNew(i));
        }

        public async Task<IEnumerable<string>> GetAllPathsAsync()
        {
            Logger.Info("Started getting path, it will take a while...");

            var buildPaths = GetFullBuildsPaths();

            return (await Task.WhenAll(buildPaths.Select(async path =>
            {
                var setupPaths = Enumerable.Empty<string>();
                try
                {
                    if (! await Program.ExistInDatabase(path))
                        setupPaths = await GetInstallers(path);
                }
                catch (Exception e)
                {
                    Logger.Warn(e);
                }
                return setupPaths;
            }))).SelectMany(path => path);
        }
    }
}
