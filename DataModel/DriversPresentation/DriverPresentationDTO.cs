using System;
using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.DataModel.DriversPresentation
{
    [Serializable]
    public class DriverPresentationDTO
    {
        public string DriverName { get; set; }
        public bool CustomOutLineEnabled { get; set; }
        public ColorDTO OutLineColor { get; set; }
    }
}