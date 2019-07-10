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
        private const string PRE_RELEASES_PATH = @"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix";
        private const string RELEASED_PATH = @"\\demant.com\data\KBN\RnD\FS_Programs\Fitting Applications\";

        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool IsNightly(string path) => path.Contains("\\Nightly");

        private bool IsFull(string path) => path.Contains("\\FullInstaller");

        private bool IsReleased(string path) => path.Contains("\\Released");

        private bool IsBuildNew(string pathToCheck)
        {
            var directoryInfo = new DirectoryInfo(pathToCheck);
            var dateOfLastModify = directoryInfo.LastWriteTime.Date;

            if (IsNightly(pathToCheck))
                return !Program.IsOlderThan(dateOfLastModify, 7);

            return !Program.IsOlderThan(dateOfLastModify, 14);
        }

        private async Task<IEnumerable<string>> GetSetupPathsAsync(string buildPath)
        {
            return await Task.Run(() => Directory.GetFiles(buildPath, "setup.exe", SearchOption.AllDirectories));
        }

        private async Task<IEnumerable<string>> GetExePathsAsync(string buildPath)
        {
            return await Task.Run(() => Directory.GetFiles(buildPath, "*.exe", SearchOption.AllDirectories).
                Where(path => !path.Contains("EmulatorProgram") && !path.Contains("ScriptingTool")));
        }

        private async Task<IEnumerable<string>> GetInstallers(string buildPath)
        {
            if (IsNightly(buildPath))
            {
                return await GetExePathsAsync(buildPath);
            }

            return await GetSetupPathsAsync(buildPath);
        }

        private IEnumerable<string> GetAllBuildsPaths()
        {
            var preReleases = Enumerable.Empty<string>();
            bool success = false;
            int counter = 0;
            while(!success)
            {
                try
                {
                    preReleases = GetAllPreReleasedVersions().SelectMany(Directory.GetDirectories)
                                      .Where(i => ((i.Contains("rc") || i.Contains("master")) && IsBuildNew(i)) ||
                                  i.Contains("IP"));
                    success = true;
                    counter++;
                }    
                catch(Exception e)
                {
                    Logger.Warn(e);
                    if (counter > 20)
                        throw;
                }
            }
            

            //var preReleases = GetAllReleasedVersions();
            return preReleases;
        }

        private IEnumerable<string> GetAllReleasedVersions()
        {
            var dirs = Directory.GetDirectories(RELEASED_PATH, "Released", SearchOption.AllDirectories);
            return dirs;
        }

        private IEnumerable<string> GetAllPreReleasedVersions()
        {
            var dirs = Directory.GetDirectories(PRE_RELEASES_PATH);
            return dirs.Where(path => (IsFull(path) || IsNightly(path))
                                    && IsBuildNew(path));
        }

        public async Task<IEnumerable<string>> GetAllPathsAsync()
        {
            Logger.Info("Started getting path, it will take a while...");

            var buildPaths = GetAllBuildsPaths();

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
