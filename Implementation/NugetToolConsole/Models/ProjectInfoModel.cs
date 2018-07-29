namespace NugetToolConsole.Models
{
    /// <summary>
    /// The ProjectInfoModel class
    /// </summary>
    public class ProjectInfoModel
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the directory.
        /// </summary>
        /// <value>
        /// The directory.
        /// </value>
        public string ProjectDirectory { get; set; }

        /// <summary>
        /// Gets or sets the nuspec path.
        /// </summary>
        /// <value>
        /// The nuspec path.
        /// </value>
        public string NuspecPath { get; set; }

        /// <summary>
        /// Gets or sets the nuget version.
        /// </summary>
        /// <value>
        /// The nuget version.
        /// </value>
        public string NugetVersion { get; set; }
    }
}
