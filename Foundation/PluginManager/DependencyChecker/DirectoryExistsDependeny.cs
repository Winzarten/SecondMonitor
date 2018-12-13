namespace SecondMonitor.PluginManager.DependencyChecker
{
    using System.IO;
    public class DirectoryExistsDependency : IDependency
    {

        public DirectoryExistsDependency(string directoryToCheck)
        {
            DirectoryToCheck = directoryToCheck;
        }

        public string DirectoryToCheck { get; }

        public virtual bool ExistsDependency(string basePath)
        {
            return Directory.Exists(Path.Combine(basePath, DirectoryToCheck));
        }

        public string GetBatchCommand(string executablePath)
        {
            string dstPath = Path.Combine(executablePath, DirectoryToCheck);
            return "md \"" + dstPath + "\"";
        }
    }
}