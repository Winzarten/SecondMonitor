using System;

namespace SecondMonitor.SimdataManagement.SimSettings
{
    public class SimSettingsException : Exception
    {
        public SimSettingsException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}