namespace SecondMonitor.PluginManager.DependencyChecker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class DependencyChecker
    {
        public DependencyChecker(IReadOnlyCollection<IDependency> dependencies, Func<bool> shouldInstallDependency)
        {
            Dependencies = dependencies;
            ShouldInstallDependency = shouldInstallDependency;
        }

        public Func<bool> ShouldInstallDependency { get; set; }

        public bool Checked { get; private set; }

        public IReadOnlyCollection<IDependency> Dependencies { get; }

        public void CheckAndInstallDependencies(string basePath)
        {
            Action installAction = CheckAndReturnInstallDependenciesAction(basePath);

            installAction?.Invoke();
        }

        public Action CheckAndReturnInstallDependenciesAction(string basePath)
        {
            IReadOnlyCollection<IDependency> missingDependencies =
                Dependencies.Where(x => !x.ExistsDependency(basePath)).ToList();
            Checked = true;

            if (!missingDependencies.Any() || !ShouldInstallDependency())
            {
                return null;
            }
            string filePath = CreateBatchFile(missingDependencies, basePath);
            return () => StartWithElevated(filePath);
        }

        private static void StartWithElevated(string filePath)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.FileName = filePath;
            startInfo.Verb = "runas";
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            process.StartInfo = startInfo;
            process.Start();
        }

        private string CreateBatchFile(IReadOnlyCollection<IDependency> dependencies, string basePath)
        {
            string fileName = Path.Combine(Path.GetTempPath(), "installSecondMonitorDependencies.cmd");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            StringBuilder cmdFileContent = new StringBuilder();
            foreach (IDependency dependency in dependencies)
            {
                cmdFileContent.Append(dependency.GetBatchCommand(basePath) + "\n");
            }

            File.WriteAllText(fileName, cmdFileContent.ToString());
            return fileName;
        }
    }
}