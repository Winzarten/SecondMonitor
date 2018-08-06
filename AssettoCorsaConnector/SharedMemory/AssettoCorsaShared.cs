namespace SecondMonitor.AssettoCorsaConnector.SharedMemory
{
    public class AssettoCorsaShared
    {
        public const string SharedMemoryNamePhysics = "Local\\acpmf_physics";
        public const string SharedMemoryNameGraphic = "Local\\acpmf_graphics";
        public const string SharedMemoryNameStatic = "Local\\acpmf_static";
        public const string SharedMemoryNameSecondMonitor = "Local\\acpmf_secondMonitor";

        public SPageFilePhysics AcsPhysics { get; set; }
        public SPageFileGraphic AcsGraphic { get; set; }
        public SPageFileStatic AcsStatic { get; set; }
        public SPageFileSecondMonitor AcsSecondMonitor { get; set; }
    }
}