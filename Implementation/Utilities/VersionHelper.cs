namespace Utilities
{
    using System;
    using System.Linq;

    using Common.Logging;

    /// <summary>
    /// The VersionHelper class
    /// </summary>
    public static class VersionHelper
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(VersionHelper));

        /// <summary>
        /// Gets the bigger version.
        /// </summary>
        /// <param name="newVersion">The new version.</param>
        /// <param name="oldVersion">The old version.</param>
        /// <returns>
        /// Return higher version
        /// </returns>
        public static string GetHigherVersion(string newVersion, string oldVersion)
        {
            Logger.DebugFormat("[GetHigherVersion] - Entry with Version1: {0}, version2: {1}", oldVersion, newVersion);

            var result = string.Empty;
            var oldVersionList = oldVersion.Split('.').ToList();
            var newVersionList = newVersion.Split('.').ToList();
            if (oldVersionList.Count == newVersionList.Count)
            {
                for (int i = 0; i < oldVersionList.Count; i++)
                {
                    var oldVersionMember = int.Parse(oldVersionList[i]);
                    var newVersionMember = int.Parse(newVersionList[i]);
                    if (oldVersionMember == newVersionMember)
                    {
                        continue;
                    }

                    result = (newVersionMember > oldVersionMember) ? newVersion : oldVersion;
                    Logger.DebugFormat("[GetHigherVersion] - Leave with Version: {0}", result);
                    return result;
                }
            }

            Logger.DebugFormat("[GetHigherVersion] - Leave with Version: {0}", result);
            return result;
        }

        /// <summary>
        /// Gets the lower version.
        /// </summary>
        /// <param name="newVersion">The new version.</param>
        /// <param name="oldVersion">The old version.</param>
        /// <returns>
        /// Lower version
        /// </returns>
        public static string GetLowerVersion(string newVersion, string oldVersion)
        {
            Logger.DebugFormat("[GetLowerVersion] - Entry with Version1: {0}, version2: {1}", oldVersion, newVersion);

            var result = string.Empty;
            var oldVersionList = oldVersion.Split('.').ToList();
            var newVersionList = newVersion.Split('.').ToList();
            if (oldVersionList.Count == newVersionList.Count)
            {
                for (int i = 0; i < oldVersionList.Count; i++)
                {
                    var oldVersionMember = int.Parse(oldVersionList[i]);
                    var newVersionMember = int.Parse(newVersionList[i]);
                    if (oldVersionMember == newVersionMember)
                    {
                        continue;
                    }

                    result = (newVersionMember < oldVersionMember) ? newVersion : oldVersion;
                    Logger.DebugFormat("[GetLowerVersion] - Leave with Version: {0}", result);
                    return result;
                }
            }

            Logger.DebugFormat("[GetLowerVersion] - Leave with Version: {0}", result);
            return result;
        }

        /// <summary>
        /// Determines whether [is need to change version] [the specified old version].
        /// </summary>
        /// <param name="newVersion">The new version.</param>
        /// <param name="oldVersion">The old version.</param>
        /// <param name="maxVersion">The maximum version.</param>
        /// <returns>
        ///   <c>true</c> if [is need to change version] [the specified old version]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNeedToChangeVersion(string newVersion, string oldVersion, string maxVersion)
        {
            Logger.DebugFormat("[IsNeedToChangeVersion] - Entry with Version1: {0}, version2: {1}", oldVersion, newVersion);

            var oldVersionList = oldVersion.Split('.').Select(int.Parse).ToList();
            var newVersionList = newVersion.Split('.').Select(int.Parse).ToList();
            if (oldVersionList.Count == newVersionList.Count)
            {
                if (Math.Abs(oldVersionList[2] - newVersionList[2]) == 1 || string.Equals(newVersion, maxVersion) || string.Equals(oldVersion, maxVersion))
                {
                    return false;
                }
            }

            Logger.DebugFormat("[GetLowerVersion] - Leave");
            return true;
        }

        /// <summary>
        /// Builds the latest nuget version.
        /// </summary>
        /// <param name="nugetVersion">The nuget version.</param>
        /// <returns> Latest nuget version </returns>
        public static string BuildLatestNugetVersion(string nugetVersion)
        {
            Logger.DebugFormat("[BuildLatestNugetVersion] - Entry with Version: {0}", nugetVersion);

            var nugetVersionList = nugetVersion.Split('.').Select(int.Parse).ToList();
            for (int i = 0; i < nugetVersionList.Count; i++)
            {
                if (i == 1)
                {
                    nugetVersionList[i] = nugetVersionList[i] + 1;
                }
                else if (i > 1)
                {
                    nugetVersionList[i] = 0;
                }
            }

            var version = string.Join(".", nugetVersionList.ToArray());

            Logger.DebugFormat("[GetLowerVersion] - Leave");
            return version;
        }
    }
}
