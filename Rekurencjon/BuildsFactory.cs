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


        public IEnumerable<Build> GetAllBuilds(IEnumerable<string> buildPaths)
        {
            var builds = new List<Build>();
            foreach (var path in buildPaths)
            {
                if (TryCreateBuild(path, out var build))
                {
                    builds.Add(build);
                }
            }

            Logger.Info($"Number of builds: {builds.Count}");

            return builds;
        }

        private bool TryCreateBuild(string path, out Build newBuild)
        {
            newBuild = null;
            if (TryGetAboutInfo(path, out var aboutInfo)
                && TryGetRelease(path, out var release)
                && TryGetMode(path, out var mode)
                && TryGetBuildID(path, out var buildID)
                && GetType(path, out var type)
                && GetBrand(path, out var brand)
                && GetOem(path, out var oem))
            {
                newBuild = new Build()
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

                return true;
            }
            //Logger.Warn($"Unable to create build: {path}");

            return false;
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

        private bool TryGetAboutForMedium(string path, out string aboutInfo)
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
                            return TryGetAboutInfo(line, out aboutInfo);
                    }
                }
            }

            Logger.Warn($"Unable to get about for: {path}");
            aboutInfo = "";
            return false;
        }

        private bool TryGetAboutInfo(string path, out string aboutInfo)
        {
            var charsToTrim = new[] { '-', '_', '.', '\\' };
            var match = Regex.Match(path, @"\d*\.\d*\.\d*\.?\d*(-|\\|\-|\.|)");
            aboutInfo = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (path.Contains("Medium") && !match.Success)
            {
                return TryGetAboutForMedium(path, out aboutInfo);
            }

            if (!match.Success)
                Logger.Warn($"Unable to get about for: {path}");

            return match.Success;
        }

        public static bool TryGetBuildID(string path, out string buildID)
        {
            var charsToTrim = new[] { '-', '_' };
            var match = Regex.Match(path, @"(((Full|Night|Fitting)(.*?)\\(.*)(\\))|((Full|Night|Fitting)(.*?)\\(.*)))");
            buildID = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (!match.Success)
                Logger.Warn($"Unable to get buildID for: {path}");

            return match.Success;
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

        private bool GetOem(string path, out string oem)
        {
            if (path.Contains("DevResults"))
                return GetBrand(path, out oem);

            var charsToTrim = new[] { '-', '_', '\\' };
            var match = Regex.Match(path, @"(((?<=Medium)([^\d]*)(?=\.exe))|((?<=\\)([^\d]*)(?=setup)))");
            oem = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (!match.Success)
                Logger.Warn($"Unable to get oem for: {path}");

            return match.Success;
        }

        private bool GetType(string path, out string type)
        {
            var charsToTrim = new[] { '-', '_' };
            var match = Regex.Match(path, @"(Medium|Full|Dev|Released)");
            type = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (type.Equals("Dev"))
                type = "Composition";

            if (type.Equals("Released"))
                type = "Full";

            if (!match.Success)
                Logger.Warn($"Unable to get release for: {path}");

            return match.Success;
        }

        private bool TryGetRelease(string path, out string release)
        {
            var charsToTrim = new[] { '-', '_' };
            var match = Regex.Match(path, @"-\d{2}\.\d{1}");
            release = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (!match.Success)
                Logger.Warn($"Unable to get release for: {path}");

            return match.Success;
        }

        private bool TryGetMode(string path, out string mode)
        {
            var charsToTrim = new[] { '-', '_' };
            var match = Regex.Match(path, @"(rc|master|Released|IP\d*)");
            mode = match.Value.TrimStart(charsToTrim).TrimEnd(charsToTrim);

            if (!match.Success)
                Logger.Warn($"Unable to get mode for: {path}");

            return match.Success;
        }
    }
}
