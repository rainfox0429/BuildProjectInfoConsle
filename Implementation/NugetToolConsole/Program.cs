namespace NugetToolConsole
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Common.Logging;

    using NugetToolConsole.Models;

    using Utilities;

    /// <summary>
    /// The Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Logger.Debug("NugetToolConsole <- Start");

            // Build project info before this sprint
            var gitPath = "D:\\SMEE_Project\\NADAE_Sprint_4\\Implementation";
            var oldBranch = "refs/heads/NADAE-M2_S004_1218";

            var listOldProjectInfoModels = BuildAllProjectInfoModel(gitPath, oldBranch);

            //// Temp save to excel file
            //var nugetInfos = new List<object[]>();
            //foreach (var nugetProjectChanged in listOldProjectInfoModels)
            //{
            //    nugetInfos.Add(new object[] { nugetProjectChanged.ProjectName, nugetProjectChanged.NugetVersion });
            //}

            //// Save excel file
            //ExcelHelper.SaveProjectInfo("D:\\Study project\\NugetVerion.xlsx", nugetInfos);

            // Build list project info changed
            var startDat = "06/14/2018";
            var endDay = "07/29/2018";
            var newBranch = "refs/heads/NADAE-M2_S005_2418_Dev";
            var newGitPath = "D:\\SMEE_Project\\NADAE_Sprint_5\\Implementation";
            var listChangedProjectInfoModels = BuildChangedProjectInfoModel(newGitPath, newBranch, startDat, endDay);

            //// Temp save to excel file
            //var newNugetInfos = new List<object[]>();
            //foreach (var nugetProjectChanged in listChangedProjectInfoModels)
            //{
            //    newNugetInfos.Add(new object[] { nugetProjectChanged.ProjectName, nugetProjectChanged.NugetVersion });
            //}

            //// Save excel file
            //ExcelHelper.SaveProjectInfo("D:\\Study project\\NugetVerion1.xlsx", newNugetInfos);

            // Buildm latest version for project
            var maxVersion = "1.15.0";
            var nugetVersionList = new List<NugetInfo>();
            foreach (var changedProjectInfoModel in listChangedProjectInfoModels)
            {
                var nugetInfo = new NugetInfo() { NugetName = changedProjectInfoModel.ProjectName };
                var oldProjectInfo = listOldProjectInfoModels.FirstOrDefault(
                    p => string.Equals(p.ProjectName, changedProjectInfoModel.ProjectName));
                if (oldProjectInfo == null)
                {
                    nugetInfo.NugetVersion = changedProjectInfoModel.NugetVersion;
                    nugetInfo.OldNugetVersion = changedProjectInfoModel.NugetVersion;
                    nugetInfo.NeedUpgrate = !string.Equals(changedProjectInfoModel.NugetVersion, maxVersion);
                }
                else
                {
                    if (string.Equals(changedProjectInfoModel.NugetVersion, oldProjectInfo.NugetVersion) && string.Equals(changedProjectInfoModel.NugetVersion, maxVersion))
                    {
                        nugetInfo.NugetVersion = maxVersion;
                        nugetInfo.OldNugetVersion = maxVersion;
                        nugetInfo.NeedUpgrate = false;
                    }
                    else if (string.Equals(changedProjectInfoModel.NugetVersion, oldProjectInfo.NugetVersion) && !string.Equals(changedProjectInfoModel.NugetVersion, maxVersion))
                    {
                        nugetInfo.NugetVersion = changedProjectInfoModel.NugetVersion;
                        nugetInfo.OldNugetVersion = oldProjectInfo.NugetVersion;
                        nugetInfo.NeedUpgrate = true;
                    }
                    else if (!string.Equals(changedProjectInfoModel.NugetVersion, oldProjectInfo.NugetVersion))
                    {
                        nugetInfo.OldNugetVersion = VersionHelper.GetLowerVersion(
                            changedProjectInfoModel.NugetVersion, oldProjectInfo.NugetVersion);
                        nugetInfo.NugetVersion = VersionHelper.GetHigherVersion(changedProjectInfoModel.NugetVersion, oldProjectInfo.NugetVersion);
                        nugetInfo.NeedUpgrate = VersionHelper.IsNeedToChangeVersion(changedProjectInfoModel.NugetVersion, oldProjectInfo.NugetVersion, maxVersion);
                    }
                }

                nugetVersionList.Add(nugetInfo);
            }

            var latestBuild = 100;
            var listNugetProjectNeedToChangeds = nugetVersionList.Where(n => n.NeedUpgrate && n.NugetName.StartsWith("SMEE.")).ToList();
            foreach (var nugetProjectNeedToChanged in listNugetProjectNeedToChangeds)
            {
                nugetProjectNeedToChanged.NugetVersionNeedToUpgrate =
                    VersionHelper.BuildLatestNugetVersion(nugetProjectNeedToChanged.NugetVersion);
                nugetProjectNeedToChanged.AssemblyVersionNeedToUpgrate =
                    $"{nugetProjectNeedToChanged.NugetVersionNeedToUpgrate}.{latestBuild + 1}";

            }

            var nugetVersionInfos = new List<object[]>();
            foreach (var nugetVersionInfo in listNugetProjectNeedToChangeds)
            {
                nugetVersionInfos.Add(new object[]
                                          {
                                              nugetVersionInfo.NugetName,
                                              nugetVersionInfo.OldNugetVersion,
                                              nugetVersionInfo.NugetVersion,
                                              nugetVersionInfo.NeedUpgrate,
                                              nugetVersionInfo.NugetVersionNeedToUpgrate,
                                              nugetVersionInfo.AssemblyVersionNeedToUpgrate
                                          });
            }

            // Save excel file
            ExcelHelper.SaveProjectInfo("D:\\Study project\\NugetVerion.xlsx", nugetVersionInfos);

            Logger.Debug("NugetToolConsole -> End");
        }

        /// <summary>
        /// Builds the project information model.
        /// </summary>
        /// <param name="gitPath">The git path.</param>
        /// <param name="gitBranch">The git branch.</param>
        /// <returns>
        /// List project info model
        /// </returns>
        private static List<ProjectInfoModel> BuildAllProjectInfoModel(string gitPath, string gitBranch)
        {
            var listProjectInfoModel = new List<ProjectInfoModel>();

            // Pull source old sprint
            var projectVersionManagement = new ProjectVersionManagement();
            projectVersionManagement.Pull(Directory.GetParent(gitPath).ToString(), gitBranch);

            // Get list file changed
            var projectScaner = new ProjectScaner();
            listProjectInfoModel = projectScaner.GetListNugetProjectInfos(gitPath).ToList();

            // Read nuspecfile to build project info
            foreach (var nugetProject in listProjectInfoModel)
            {
                nugetProject.NugetVersion = ProjectHelper.GetNugetVersion(nugetProject.NuspecPath);
            }

            return listProjectInfoModel;
        }

        /// <summary>
        /// Builds the changed project information model.
        /// </summary>
        /// <param name="gitPath">The git path.</param>
        /// <param name="gitBranch">The git branch.</param>
        /// <param name="startDay">The start day.</param>
        /// <param name="endDay">The end day.</param>
        /// <returns>
        /// List change project info model
        /// </returns>
        private static List<ProjectInfoModel> BuildChangedProjectInfoModel(string gitPath, string gitBranch, string startDay, string endDay)
        {
            var listProjectInfoModel = new List<ProjectInfoModel>();

            // Pull source old sprint
            var projectVersionManagement = new ProjectVersionManagement();
            projectVersionManagement.Pull(Directory.GetParent(gitPath).ToString(), gitBranch);

            // Get project nuget
            var listFileChanged = projectVersionManagement.GetListFileChanged(Directory.GetParent(gitPath).ToString(), startDay, endDay);

            // Get list file changed
            var projectScaner = new ProjectScaner();
            var listNugetProject = projectScaner.GetListNugetProjectInfos(gitPath);

            // Get list nuget project wich was changed
            foreach (var nugetProject in listNugetProject)
            {
                var isChanged = listFileChanged.Any(f => f.Contains(nugetProject.ProjectName));
                if (isChanged)
                {
                    listProjectInfoModel.Add(nugetProject);
                }
            }

            // Read nuspecfile to build project info
            foreach (var nugetProject in listProjectInfoModel)
            {
                nugetProject.NugetVersion = ProjectHelper.GetNugetVersion(nugetProject.NuspecPath);
            }

            return listProjectInfoModel;
        }
    }
}
