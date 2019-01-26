namespace SecondMonitor.SimdataManagement
{
    public interface IMapsLoaderFactory
    {
        MapsLoader Create(string repositoryPath);
    }
}