using System;

namespace SecondMonitor.RF2Connector
{
    public class RF2InvalidPackageException : Exception
    {
        public RF2InvalidPackageException(Exception innerException)
            : base(string.Empty, innerException)
        {

        }

        public RF2InvalidPackageException(string msg)
            : base(msg)
        {

        }
    }
}