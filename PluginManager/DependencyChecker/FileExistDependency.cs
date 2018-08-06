namespace SecondMonitor.PluginManager.DependencyChecker
{
    using System.IO;
    public class FileExistDependency : IDependency
    {

        public FileExistDependency(string fileToCheck, string fileToInstall)
        {
            FileToCheck = fileToCheck;
            FileToInstall = fileToInstall;
        }

        public string FileToCheck { get; }
        public string FileToInstall { get; }

        public virtual bool ExistsDependency(string basePath)
        {
            return File.Exists(Path.Combine(basePath, FileToCheck));
        }

        public string GetBatchCommand(string executablePath)
        {
            string sourcePath = Path.Combine(Directory.GetCurrentDirectory(), FileToInstall);
            string dstPath = Path.Combine(executablePath, FileToCheck);
            return "copy \"" + sourcePath + "\" \"" + dstPath + "\"";
        }
    }
}