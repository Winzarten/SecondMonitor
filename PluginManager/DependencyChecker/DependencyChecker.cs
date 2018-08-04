using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SecondMonitor.PluginManager.DependencyChecker
{
    public class DependencyChecker
    {
        public DependencyChecker(IReadOnlyCollection<FileDependency> fileDependencies, Func<bool> shouldInstallDependency)
        {
            FileDependencies = fileDependencies;
            ShouldInstallDependency = shouldInstallDependency;
        }

        public Func<bool> ShouldInstallDependency;

        public bool Checked { get; private set; }

        public IReadOnlyCollection<FileDependency> FileDependencies { get; }

        public void CheckAndInstallDependencies(string basePath)
        {
            Action installAction = CheckAndReturnInstallDependeciesAction(basePath);

            installAction?.Invoke();
        }

        public Action CheckAndReturnInstallDependeciesAction(string basePath)
        {
            IReadOnlyCollection<FileDependency> missingDependencies =
                FileDependencies.Where(x => !x.ExistsDependency(basePath)).ToList();
            Checked = true;

            if (!missingDependencies.Any() || !ShouldInstallDependency())
            {
                return null;
            }
            string filePath = CreateBatchFile(missingDependencies, basePath);
            return () => StartWithElevated(filePath);
        }

        private void StartWithElevated(string filePath)
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

        private string CreateBatchFile(IReadOnlyCollection<FileDependency> dependencies, string basePath)
        {
            string fileName = Path.Combine(Path.GetTempPath(),
                "installSecondMonitorDependencies.cmd");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            StringBuilder cmdFileContent = new StringBuilder();
            foreach (FileDependency fileDependency in dependencies )
            {
                string sourcePath = Path.Combine(Directory.GetCurrentDirectory(), fileDependency.FileToInstall);
                string dstPath = Path.Combine(basePath, fileDependency.FileToCheck);
                cmdFileContent.Append("copy \""  + sourcePath + "\" \"" + dstPath + "\"\n");
            }

            File.WriteAllText(fileName, cmdFileContent.ToString());
            return fileName;
        }
    }
}