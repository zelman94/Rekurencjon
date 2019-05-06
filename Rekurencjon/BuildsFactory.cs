using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.SystemFunctions;
using Newtonsoft.Json;

namespace Rekurencjon
{
    internal class BuildsFactory
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public async Task<IEnumerable<Build>> GetAllBuilds(IEnumerable<string> buildPaths)
        {
            var builds = (await Task.WhenAll(buildPaths.Select(async path =>
            {
                var (success, build) = await TryCreateBuild(path);
                return success ? build : null;
            }))).Where(b => b != null);

            var buildList = builds.ToList();
            Logger.Info($"Number of builds: {buildList.Count}");

            return buildList;
        }

        private async Task<(bool Success, Build Build)> TryCreateBuild(string path)
        {
            var (infoSuccess, aboutInfo) = await TryGetAboutInfo(path);
            var (releaseSuccess, release) = await TryGetRelease(path);
            var (modeSuccess, mode) = await TryGetMode(path);
            var (buildIdSuccess, buildID) = await TryGetBuildID(path);
            var (typeSuccess, type) = await GetType(path);
            var (oemSuccess, oem) = await GetOem(path);

            if (infoSuccess && releaseSuccess && modeSuccess
                && buildIdSuccess&& typeSuccess
                && GetBrand(path, out var brand)
                && oemSuccess)
            {
                var newBuild = new Build()
                {
                    Type = type,
                    Release = release,
                    Mode = mode,
                    About = aboutInfo,
                    Brand = brand,
                    Oem = oem,
                    CreationDate = Directory.GetCreationTime(path).Date,
                    BuildID = buildID,
                    Path = path
                };

                return (true, newBuild);
            }

            Logger.Warn($"Unable to create build: {path}");

            return (false, null);
        }

        private static string CreateJSONPath(string path)
        {
            var parts = path.Split('\\');

            var pathToBuildInfo = parts[0];
            for (var i = 1; i < parts.Length - 2; i++)
            {
                pathToBuildInfo += "\\" + parts[i];
            }

            var buildInfoJson = @"\BuildInfo.json";
            if (!File.Exists(pathToBuildInfo + buildInfoJson))
                pathToBuildInfo += @"\DOC";

            return pathToBuildInfo + buildInfoJson;
        }

        private async Task<(bool Success, string AboutInfo)> TryGetAboutForMedium(string path)
        {
            var buildInfoJSON = CreateJSONPath(path);

            if (File.Exists(buildInfoJSON))
            {
                using (StreamReader reader = new StreamReader(buildInfoJSON))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line.Contains("Version"))
                            return await TryGetAboutInfo(line);
                    }
                }
            }

            Logger.Warn($"Unable to get about for: {path}");
            var aboutInfo = "";
            return (false, aboutInfo);
        }

        private async Task<(bool Success, string AboutInfo)> TryGetAboutInfo(string path)
        {
            var (success, aboutInfo) = await RunRegex(path, @"\d*\.\d*\.\d*\.?\d*(-|\\|\-|\.|)");

            if (path.Contains("Medium") && !success)
            {
                return await TryGetAboutForMedium(path);
            }

            if (!success && PathFinder.IsReleased(path))
            {
                return (true, "Full");
            }

            if (!success)
                Logger.Warn($"Unable to get about for: {path}");

            return (success, aboutInfo);
        }

        public static async Task<(bool Success, string BuildID)> TryGetBuildID(string path)
        {
            var (success, buildID) = await RunRegex(path, @"(((Full|Night|Fitting)(.*?)\\(.*)(\\))|((Full|Night|Fitting)(.*?)\\(.*)))");

            if (!success)
                Logger.Warn($"Unable to get buildID for: {path}");

            return (success, buildID);
        }

        private bool GetBrand(string path, out string brandOut)
        {
            var brands = new List<string>() { "GenieMedical", "Genie", "HearSuite", "Oasis", "ExpressFit", "Cumulus" };
            foreach (var brand in brands)
            {
                if (path.IndexOf(brand, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    brandOut = brand;
                    return true;
                }
            }

            Logger.Warn($"Unable to get brand for: {path}");
            brandOut = "";
            return false;
        }

        private async Task<(bool Success, string orm)> GetOem(string path)
        {
            if (path.Contains("DevResults"))
                return (GetBrand(path, out var brand), brand);

            var (success, oem) = await RunRegex(path, @"(((?<=Medium)([^\d]*)(?=\.exe))|((?<=\\)([^\d]*)(?=setup))|((?<=Full)([^\d]*)(?=Setup)))");

            if (oem.Trim().Equals("Oticonmedical", StringComparison.InvariantCultureIgnoreCase))
                oem = "GenieMedical";

            if (!success)
                Logger.Warn($"Unable to get oem for: {path}");

            return (success, oem);
        }

        private async Task<(bool Success, string Type)> GetType(string path)
        {
            var (success, type) = await RunRegex(path, @"(Medium|Full|Dev|Released)");

            if (type.Equals("Dev"))
                type = "Composition";

            if (type.Equals("Released"))
                type = "Full";

            if (!success)
                Logger.Warn($"Unable to get release for: {path}");

            return (success, type);
        }

        private async Task<(bool Success, string Release)> TryGetRelease(string path)
        {
            var (success, release) = await RunRegex(path, @"((-\d{2}\.\d{1})|(\\\d{4}\.\d{1}\\))");
            if (!success)
                Logger.Warn($"Unable to get release for: {path}");

            return (success, release);
        }

        private async Task<(bool Success, string Mode)> TryGetMode(string path)
        {
            var (success, mode) = await RunRegex(path, @"(rc|master|Released|IP\d*)");
            if (!success)
                Logger.Warn($"Unable to get mode for: {path}");

            return (success, mode);
        }

        private static async Task<(bool Success, string Value)> RunRegex(string text, string regex)
        {
            var charsToTrim = new[] { '-', '_', '.', '\\' };
            var match = await Task.Run(() => Regex.Match(text, regex));
            var value = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);
            return (match.Success, value);
        }
    }
}
