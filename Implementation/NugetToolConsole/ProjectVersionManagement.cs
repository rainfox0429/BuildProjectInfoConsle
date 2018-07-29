namespace NugetToolConsole
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    using LibGit2Sharp;

    using NugetToolConsole.Const;
    using NugetToolConsole.Models;

    using Utilities.Extension;

    /// <summary>
    /// The ProjectVersionManagement class
    /// </summary>
    public class ProjectVersionManagement
    {
        /// <summary>
        /// Gets the list file changed.
        /// </summary>
        /// <param name="rootGitPath">The root git path.</param>
        /// <param name="startDay">The start day.</param>
        /// <param name="endDay">The end day.</param>
        /// <returns> List file which was changed </returns>
        public List<string> GetListFileChanged(string rootGitPath, string startDay, string endDay)
        {
            var result = new List<string>();
            try
            {
                // Build start day and end day
                var startDateTime = DateTime.ParseExact(startDay, CommonConstant.DayTimeFormat, CultureInfo.InvariantCulture);
                var endDateTime = DateTime.ParseExact(endDay, CommonConstant.DayTimeFormat, CultureInfo.InvariantCulture);
                var startDayNumber = DateTime.Now > startDateTime ? -(DateTime.Now - startDateTime).Days : (DateTime.Now - startDateTime).Days;
                var endDayNumer = DateTime.Now > endDateTime ? -(DateTime.Now - endDateTime).Days : (DateTime.Now - endDateTime).Days;

                // Using libgit2sharp to get change file
                using (var repo = new Repository(rootGitPath))
                {
                    // get commits from all branches, not just master
                    var commits = repo.Commits.QueryBy(
                        new CommitFilter
                        {
                            SortBy = CommitSortStrategies.Time | CommitSortStrategies.Reverse,
                            IncludeReachableFrom = repo.Branches["NADAE-M2_S005_2418_Dev"].Tip,
                        });

                    // here I can access commit's author, but not time
                    var since = new DateTimeOffset(DateTime.Now.AddDays(startDayNumber));
                    var until = new DateTimeOffset(DateTime.Now.AddDays(endDayNumer));
                    var commitLogs = commits.Where(c => c.Committer.When > since && c.Committer.When < until)?.ToList();
                    if (!commitLogs.IsNullOrEmpty())
                    {
                        // Loop on list commit
                        foreach (var commitLog in commitLogs)
                        {
                            // Ignore if don't have parrent
                            if (commitLog.Parents.IsNullOrEmpty())
                            {
                                continue;
                            }

                            // Loop on parrent commit
                            foreach (var parent in commitLog.Parents)
                            {
                                var listFileChanged = repo.Diff.Compare<TreeChanges>(parent.Tree, commitLog.Tree)
                                    .Where(t => t.Path.EndsWith(CommonConstant.CsFileFilter, StringComparison.OrdinalIgnoreCase))
                                    .Select(t => t.Path).ToList();
                                result.AddRange(listFileChanged);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result.Distinct().ToList();
        }

        /// <summary>
        /// Gets the latest commit change file.
        /// </summary>
        /// <param name="projectInfo">The project information.</param>
        /// <param name="rootGitPath">The root git path.</param>
        /// <param name="startDay">The start day.</param>
        /// <param name="endDay">The end day.</param>
        /// <returns> Latest commit</returns>
        public Commit GetLatestCommitChangeFile(ProjectInfoModel projectInfo, string rootGitPath, string startDay, string endDay)
        {
            var listCommit = new List<Commit>();
            try
            {
                // Build start day and end day
                var startDateTime = DateTime.ParseExact(startDay, CommonConstant.DayTimeFormat, CultureInfo.InvariantCulture);
                var endDateTime = DateTime.ParseExact(endDay, CommonConstant.DayTimeFormat, CultureInfo.InvariantCulture);
                var startDayNumber = DateTime.Now > startDateTime ? -(DateTime.Now - startDateTime).Days : (DateTime.Now - startDateTime).Days;
                var endDayNumer = DateTime.Now > endDateTime ? -(DateTime.Now - endDateTime).Days : (DateTime.Now - endDateTime).Days;

                // Using libgit2sharp to get change file
                using (var repo = new Repository(rootGitPath))
                {
                    // get commits from all branches, not just master
                    var commits = repo.Commits.QueryBy(
                        new CommitFilter
                        {
                            SortBy = CommitSortStrategies.Time | CommitSortStrategies.Reverse
                        });

                    // here I can access commit's author, but not time
                    var since = new DateTimeOffset(DateTime.Now.AddDays(startDayNumber));
                    var until = new DateTimeOffset(DateTime.Now.AddDays(endDayNumer));
                    var commitLogs = commits.Where(c => c.Committer.When > since && c.Committer.When < until)?.ToList();
                    if (!commitLogs.IsNullOrEmpty())
                    {
                        // Loop on list commit
                        foreach (var commitLog in commitLogs)
                        {
                            // Ignore if don't have parrent
                            if (commitLog.Parents.IsNullOrEmpty())
                            {
                                continue;
                            }

                            // Loop on parrent commit
                            foreach (var parent in commitLog.Parents)
                            {
                                var listFileChanged = repo.Diff.Compare<TreeChanges>(parent.Tree, commitLog.Tree)
                                    .Any(t => t.Path.EndsWith($"{projectInfo.ProjectName}{CommonConstant.NuspecFileFilter}", StringComparison.OrdinalIgnoreCase));
                                if (listFileChanged)
                                {
                                    listCommit.Add(commitLog);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var latestCommit = listCommit.OrderBy(t => t.Author.When).LastOrDefault();
            return latestCommit;
        }

        /// <summary>
        /// Reverts the specified root path.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="latestCommit">The latest commit.</param>
        /// <returns> Content at commit </returns>
        public string Revert(string rootPath, string filePath, Commit latestCommit)
        {
            var result = string.Empty;
            try
            {
                var tempRootPath = new Uri($"{rootPath}\\", UriKind.Absolute);
                var tempFilePath = new Uri(filePath, UriKind.Absolute);
                var relativePath = tempRootPath.MakeRelativeUri(tempFilePath).ToString();

                using (var repo = new Repository(rootPath))
                {
                    if (latestCommit != null)
                    {
                        var commit = repo.Lookup<Commit>(latestCommit.Id); // or any other way to retreive a specific commit
                        var treeEntry = commit[relativePath];

                        var blob = (Blob)treeEntry.Target;

                        var contentStream = blob.GetContentStream();

                        using (var tr = new StreamReader(contentStream, Encoding.UTF8))
                        {
                            string content = tr.ReadToEnd();
                            result = content;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Pulls the specified git repo path.
        /// </summary>
        /// <param name="gitRepoPath">The git repo path.</param>
        /// <param name="gitbranch">The gitbranch.</param>
        public void Pull(string gitRepoPath, string gitbranch)
        {
            using (var repo = new Repository(gitRepoPath))
            {
                var branch = repo.Branches[gitbranch];

                if (branch != null)
                {
                    // repository return null object when branch not exists
                    Commands.Checkout(repo, branch);

                    PullOptions options = new PullOptions();
                    options.FetchOptions = new FetchOptions();
                    options.FetchOptions.CredentialsProvider = (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials()
                            {
                                Username = "tuanct",
                                Password = "Gcsvn123"
                            };
                    Commands.Pull(repo, new Signature("tuanct", "tuanct@gcs-vn.com", new DateTimeOffset(DateTime.Now)), options);
                }
            }
        }
    }
}
