namespace Utilities
{
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Utilities.Extension;

    /// <summary>
    /// The ProjectUtilities class
    /// </summary>
    public static class ProjectHelper
    {
        /// <summary>
        /// Determines whether [is nuget project] [the specified project path].
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="fileFilter">The file filter.</param>
        /// <param name="selectedFile">The list files.</param>
        /// <returns>
        ///   <c>true</c> if [is nuget project] [the specified project path]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExistedFile(string projectPath, string fileFilter, out string selectedFile)
        {
            selectedFile = string.Empty;
            var selectedFiles = Directory.GetFiles(projectPath, fileFilter, SearchOption.TopDirectoryOnly);
            if (!selectedFiles.IsNullOrEmpty())
            {
                selectedFile = selectedFiles.FirstOrDefault();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is existed folder] [the specified project path].
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="selectedFolder">The selected folder.</param>
        /// <returns>
        ///   <c>true</c> if [is existed folder] [the specified project path]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExistedFolder(string projectPath, string folderName, out string selectedFolder)
        {
            selectedFolder = string.Empty;
            var selectedFolders = Directory.GetDirectories(projectPath, folderName, SearchOption.TopDirectoryOnly);
            if (!selectedFolders.IsNullOrEmpty())
            {
                selectedFolder = selectedFolders.FirstOrDefault();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        /// <param name="projectFilePath">The project file path.</param>
        /// <returns> preoject name </returns>
        public static string GetProjectName(string projectFilePath)
        {
            var result = string.Empty;

            // Get project file name
            XDocument xdoc = XDocument.Load(projectFilePath);

            if (xdoc.Root != null)
            {
                foreach (XElement el in xdoc.Root.Elements())
                {
                    if (el.Elements().Any())
                    {
                        foreach (var subEl in el.Elements())
                        {
                            if (string.Equals(subEl.Name.LocalName, "AssemblyName"))
                            {
                                result = subEl.Value;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the nuget version.
        /// </summary>
        /// <param name="nuspecFilePath">Content of the XML.</param>
        /// <returns> Version of nuget </returns>
        public static string GetNugetVersion(string nuspecFilePath)
        {
            var result = string.Empty;

            // Check precondition
            if (string.IsNullOrEmpty(nuspecFilePath))
            {
                return result;
            }

            // Get project file name
            XDocument xdoc = XDocument.Load(nuspecFilePath);

            if (xdoc.Root != null)
            {
                foreach (XElement el in xdoc.Root.Elements())
                {
                    if (el.Elements().Any())
                    {
                        foreach (var subEl in el.Elements())
                        {
                            if (string.Equals(subEl.Name.LocalName, "version"))
                            {
                                return subEl.Value;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
