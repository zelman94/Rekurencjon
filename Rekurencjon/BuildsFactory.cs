using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Rekurencjon
{
    internal class BuildsFactory
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public async Task<IEnumerable<Build>> GetAllBuilds(IEnumerable<string> buildPaths)
        {
            var builds = new List<Build>();
            foreach (var path in buildPaths)
            {
                var (success, build) = await TryCreateBuild(path);
                if (success)
                {
                    builds.Add(build);
                }
            }

            Logger.Info($"Number of builds: {builds.Count}");

            return builds;
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

            var buildInfoJson = parts[0];
            for (var i = 1; i < parts.Length - 2; i++)
            {
                buildInfoJson += "\\" + parts[i];
            }

            buildInfoJson += @"\Doc\BuildInfo.json";
            return buildInfoJson;
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
            var charsToTrim = new[] { '-', '_', '.', '\\' };
            var match = await Task.Run(() => Regex.Match(path, @"\d*\.\d*\.\d*\.?\d*(-|\\|\-|\.|)"));

            var aboutInfo = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (path.Contains("Medium") && !match.Success)
            {
                return await TryGetAboutForMedium(path);
            }

            if (!match.Success)
                Logger.Warn($"Unable to get about for: {path}");

            return (match.Success, aboutInfo);
        }

        public static async Task<(bool Success, string BuildID)> TryGetBuildID(string path)
        {
            var charsToTrim = new[] { '-', '_' };
            var match = await Task.Run(() => Regex.Match(path, @"(((Full|Night|Fitting)(.*?)\\(.*)(\\))|((Full|Night|Fitting)(.*?)\\(.*)))"));
            var buildID = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (!match.Success)
                Logger.Warn($"Unable to get buildID for: {path}");

            return (match.Success, buildID);
        }

        private bool GetBrand(string path, out string brandOut)
        {
            var brands = new List<string>() { "Genie", "GenieMedical", "HearSuite", "Oasis", "ExpressFit" };
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

            var charsToTrim = new[] { '-', '_', '\\' };
            var match = await Task.Run(() => Regex.Match(path, @"(((?<=Medium)([^\d]*)(?=\.exe))|((?<=\\)([^\d]*)(?=setup)))"));
            var oem = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (!match.Success)
                Logger.Warn($"Unable to get oem for: {path}");

            return (match.Success, oem);
        }

        private async Task<(bool Success, string Type)> GetType(string path)
        {
            var charsToTrim = new[] { '-', '_' };
            var match = await Task.Run(() => Regex.Match(path, @"(Medium|Full|Dev|Released)"));
            var type = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (type.Equals("Dev"))
                type = "Composition";

            if (type.Equals("Released"))
                type = "Full";

            if (!match.Success)
                Logger.Warn($"Unable to get release for: {path}");

            return (match.Success, type);
        }

        private async Task<(bool Success, string Release)> TryGetRelease(string path)
        {
            var charsToTrim = new[] { '-', '_' };
            var match = await Task.Run(() => Regex.Match(path, @"-\d{2}\.\d{1}"));
            var release = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (!match.Success)
                Logger.Warn($"Unable to get release for: {path}");

            return (match.Success, release);
        }

        private async Task<(bool Success, string Mode)> TryGetMode(string path)
        {
            var charsToTrim = new[] { '-', '_' };
            var match = await Task.Run(() => Regex.Match(path, @"(rc|master|Released|IP\d*)"));
            var mode = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (!match.Success)
                Logger.Warn($"Unable to get mode for: {path}");

            return (match.Success, mode);
        }
    }
}
