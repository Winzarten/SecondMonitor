namespace SecondMonitor.PluginManager.DependencyChecker
{
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    public class FileExistsAndMatchDependency : FileExistDependency
    {
        public FileExistsAndMatchDependency(string fileToCheck, string fileToInstall)
            : base(fileToCheck, fileToInstall)
        {

        }

        public override bool ExistsDependency(string basePath)
        {
            bool fileExists = base.ExistsDependency(basePath);
            if (!fileExists)
            {
                return false;
            }

            string fullPathToFileToCheck = Path.Combine(basePath, FileToCheck);
            string fullPathToFileToInstall = Path.Combine(Directory.GetCurrentDirectory(), FileToInstall);

            using (MD5 fileToCheckMd5 = MD5.Create())
            using (MD5 fileToInstallMd5 = MD5.Create())
            using (FileStream fileToCheckStream = File.OpenRead(fullPathToFileToCheck))
            using (FileStream fileToInstallStream = File.OpenRead(fullPathToFileToInstall))
            {
                byte[] fileToCheckHash = fileToCheckMd5.ComputeHash(fileToCheckStream);
                byte[] fileToInstallHash = fileToInstallMd5.ComputeHash(fileToInstallStream);
                return fileToCheckHash.SequenceEqual(fileToInstallHash);
            }
        }

    }
}