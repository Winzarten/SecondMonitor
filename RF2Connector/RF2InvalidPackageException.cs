using System;

namespace SecondMonitor.RF2Connector
{
    public class RF2InvalidPackageException : Exception
    {
        public RF2InvalidPackageException(string msg)
            : base(msg)
        {

        }
    }
}