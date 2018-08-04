using System.IO;

namespace SecondMonitor.PluginManager.DependencyChecker
{
    public class FileDependency
    {

        public FileDependency(string fileToCheck, string fileToInstall)
        {
            FileToCheck = fileToCheck;
            FileToInstall = fileToInstall;
        }

        public string FileToCheck { get; }
        public string FileToInstall { get; }

        public bool ExistsDependency(string basePath)
        {
            return File.Exists(Path.Combine(basePath, FileToCheck));
        }
    }
}