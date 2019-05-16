using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rekurencjon
{
    internal class BuildsFactory
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Dictionary<string, string> _typesMapping = new Dictionary<string, string>
        {
            {"Medium",   "Medium"},
            {"Full",     "Full"},
            {"Dev",      "Composition"},
            {"Released", "Full" }
        };

        private static readonly Dictionary<string, string> _regexMap = new Dictionary<string, string>
        {
            {"Mode",    @"(rc|master|Released|IP\d*)"},
            {"Release", @"((-\d{2}\.\d{1})|(\\\d{4}\.\d{1}\\))"},
            {"Type",    @"(Medium|Full|Dev|Released)"},
            {"Oem",     @"(((?<=Medium)([^\d]*)(?=\.exe))|((?<=\\)([^\d]*)(?=setup))|((?<=Full)([^\d]*)(?=Setup)))"},
            {"BuildID", @"(((Full|Night|Fitting)(.*?)\\(.*)(\\))|((Full|Night|Fitting)(.*?)\\(.*)))"},
            {"Brand",   @"(GenieMedical|Genie|HearSuite|Oasis|ExpressFit|Cumulus)"},
            {"About",   @"\d*\.\d*\.\d*\.?\d*(-|\\|\-|\.|)"}
        };

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
            var newBuild = new Build();

            foreach (var prop in typeof(Build).GetProperties())
            {
                bool success;
                object value;
                if (prop.Name == "CreationDate")
                {
                    (success, value) = (true, Directory.GetCreationTime(path).Date);
                }
                else if (prop.Name == "Id")
                {
                    (success, value) = (true, 0);
                }
                else
                {
                    (success, value) = await TryGet(prop.Name, path);
                }

                if (!success)
                {
                    Logger.Warn($"Unable to create build: {path}");
                    return (false, null);
                }

                prop.SetValue(newBuild, value);
            }

            return (true, newBuild);
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

        private static async Task<(bool Success, string AboutInfo)> TryGetAboutForMedium(string path)
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
                            return await TryGet("About",line);
                    }
                }
            }

            Logger.Warn($"Unable to get about, file {buildInfoJSON} doesn't exist");
            var aboutInfo = "";
            return (false, aboutInfo);
        }

        public static async Task<(bool Success, string Value)> TryGet(string param, string path)
        {
            if (param == "Path")
            {
                return (true, path);
            }

            if (param == "Oem")
            {
                if (path.Contains("DevResults"))
                    return await TryGet("Brand", path);
            }

            var (success, value) = await RunRegex(path, _regexMap[param]);

            if (param == "About")
            {
                if (path.Contains("Medium") && !success)
                {
                    (success, value) = await TryGetAboutForMedium(path);
                }
            }

            if (param == "Oem")
            {
                if (value.Trim().Equals("Oticonmedical", StringComparison.InvariantCultureIgnoreCase))
                    value = "GenieMedical";
            }

            if (param == "Type")
                value = _typesMapping[value];

            if (!success)
                Logger.Warn($"Unable to get {param} for: {path}");

            return (success, value);
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
