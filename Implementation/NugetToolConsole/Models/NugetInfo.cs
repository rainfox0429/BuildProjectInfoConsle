namespace NugetToolConsole.Models
{
    /// <summary>
    /// The NugetInfo class
    /// </summary>
    public class NugetInfo
    {
        /// <summary>
        /// Gets or sets the name of the nuget.
        /// </summary>
        /// <value>
        /// The name of the nuget.
        /// </value>
        public string NugetName { get; set; }

        /// <summary>
        /// Gets or sets the old nuget version.
        /// </summary>
        /// <value>
        /// The old nuget version.
        /// </value>
        public string OldNugetVersion { get; set; }

        /// <summary>
        /// Gets or sets the nuget version.
        /// </summary>
        /// <value>
        /// The nuget version.
        /// </value>
        public string NugetVersion { get; set; }

        /// <summary>
        /// Gets or sets the nuget version need to upgrate.
        /// </summary>
        /// <value>
        /// The nuget version need to upgrate.
        /// </value>
        public string NugetVersionNeedToUpgrate { get; set; }

        /// <summary>
        /// Gets or sets the assembly version need to upgrate.
        /// </summary>
        /// <value>
        /// The assembly version need to upgrate.
        /// </value>
        public string AssemblyVersionNeedToUpgrate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [need upgrate].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [need upgrate]; otherwise, <c>false</c>.
        /// </value>
        public bool NeedUpgrate { get; set; }
    }
}
