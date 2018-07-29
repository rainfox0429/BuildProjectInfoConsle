namespace NugetToolConsole.Interfaces
{
    using System.Collections.Generic;

    using NugetToolConsole.Models;

    /// <summary>
    /// The IProjectScaner interface
    /// </summary>
    public interface IProjectScaner
    {
        /// <summary>
        /// Lists the project infos.
        /// </summary>
        /// <param name="repoPath">The repo path.</param>
        /// <returns></returns>
        IList<ProjectInfoModel> ListProjectInfos(string repoPath);
    }
}
