namespace SecondMonitor.SimdataManagement
{
    public class MapsLoaderFactory : IMapsLoaderFactory
    {
        public MapsLoader Create(string repositoryPath)
        {
            return new MapsLoader(repositoryPath);
        }
    }
}