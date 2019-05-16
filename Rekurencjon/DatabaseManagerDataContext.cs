using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rekurencjon
{
    public partial class DatabaseManagerDataContext
    {
        private IEnumerable<string> DatabaseBuildIDs { get; set; }
        private readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void DeleteNotExistingBuild()
        {
            var buildsToDelete = Builds.ToList().Where(b => !File.Exists(b.Path));
            SubmitChanges();

            Logger.Info($"deleted {buildsToDelete.Count()} builds");
        }

        public void GetBuildIDFromDatabase()
        {
            DatabaseBuildIDs =  Builds.Select(b => b.BuildID).ToList();
        }

        public bool ExistInDatabase(string buildId)
        {
            if (!DatabaseBuildIDs.Any(i => i.Contains(buildId)))
            {
                return false;
            }

            return true;
        }

        public void DeleteOldPaths()
        {
            var buildsToDelete = Builds.ToList().Where(build =>
            {
                var days = 7;
                if (build.Type.Trim().Equals("Full", StringComparison.InvariantCultureIgnoreCase))
                    days = 14;

                return (build.CreationDate?.IsOlderThan(days) ?? false)
                       && !build.Mode.Contains("IP", StringComparison.InvariantCultureIgnoreCase);
            });

            var toDelete = buildsToDelete.ToList();
            Builds.DeleteAllOnSubmit(toDelete);
            SubmitChanges();

            Logger.Info($"deleted {toDelete.Count()} builds");
        }

        public void SaveBuildsToDatabase(IEnumerable<Build> builds)
        {
            var buildList = builds.ToList();

            Builds.InsertAllOnSubmit(buildList);
            SubmitChanges();

            Logger.Info($"Saved {buildList.Count} new builds to database");
        }
    }
}
