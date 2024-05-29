namespace SecondMonitor.DataModel
{
    using System;

    public static class SimulatorsNameMap
    {
        public static bool IsR3ESimulator(string simulatorName)
        {
            return simulatorName.Equals("R3E", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsNotConnected(string simulatorName)
        {
            return simulatorName.Equals("Not connected", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}