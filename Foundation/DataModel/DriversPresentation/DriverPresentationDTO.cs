using System;
using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.DataModel.DriversPresentation
{
    [Serializable]
    public sealed class DriverPresentationDto
    {
        public string DriverName { get; set; }
        public bool CustomOutLineEnabled { get; set; }
        public ColorDto OutLineColor { get; set; }
    }
}