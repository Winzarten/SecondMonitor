namespace SecondMonitor.RFactorConnector
{
    using System;
    public class RFInvalidPackageException : Exception
    {
        public RFInvalidPackageException(string msg)
            : base(msg)
        {

        }
    }
}