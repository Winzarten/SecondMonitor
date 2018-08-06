namespace SecondMonitor.PluginManager.DependencyChecker
{
    public interface IDependency
    {
        bool ExistsDependency(string executablePath);

        string GetBatchCommand(string executablePath);
    }
}