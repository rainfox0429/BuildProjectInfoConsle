namespace NugetToolConsole
{
    using System.Collections.Generic;
    using System.IO;

    using NugetToolConsole.Const;
    using NugetToolConsole.Models;

    using Utilities;
    using Utilities.Extension;

    /// <summary>
    /// The ProjectScaner class
    /// </summary>
    public class ProjectScaner
    {
        /// <summary>
        /// Lists the project infos.
        /// </summary>
        /// <param name="projectRepoPath">The repo path.</param>
        /// <returns> List project info </returns>
        public IList<ProjectInfoModel> GetListNugetProjectInfos(string projectRepoPath)
        {
            var result = new List<ProjectInfoModel>();
            var listProjectPaths = Directory.GetDirectories(projectRepoPath, CommonConstant.AllDirectoryFilter, SearchOption.TopDirectoryOnly);
            if (!listProjectPaths.IsNullOrEmpty())
            {
                foreach (var projectPath in listProjectPaths)
                {
                    if (ProjectHelper.IsExistedFile(projectPath, CommonConstant.CsprojFileFilter, out var csprojFile) &&
                        ProjectHelper.IsExistedFolder(projectPath, CommonConstant.NugetAssetsFilter, out var nugetAssetFolder))
                    {
                        var projectInfo = new ProjectInfoModel()
                        {
                            ProjectName = ProjectHelper.GetProjectName(csprojFile),
                            ProjectDirectory = projectPath,
                            NuspecPath = Path.Combine(nugetAssetFolder, $"{ProjectHelper.GetProjectName(csprojFile)}{CommonConstant.NuspecFileFilter}")
                        };

                        result.Add(projectInfo);
                    }
                }
            }

            return result;
        }
    }
}
