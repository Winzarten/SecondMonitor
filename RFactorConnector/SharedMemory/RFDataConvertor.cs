namespace SecondMonitor.RFactorConnector.SharedMemory
{
    using SecondMonitor.DataModel.Snapshot;

    internal class RFDataConvertor
    {

        public SimulatorDataSet CreateSimulatorDataSet(RfShared rfData)
        {
            SimulatorDataSet dataSet = new SimulatorDataSet("RFactor");

            return dataSet;
        }
    }
}