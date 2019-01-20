using System;
using System.Collections.Generic;

namespace SecondMonitor.DataModel.DriversPresentation
{
    [Serializable]
    public sealed class DriverPresentationsDto
    {
        public DriverPresentationsDto()
        {
            DriverPresentations = new List<DriverPresentationDto>();
        }

        public List<DriverPresentationDto> DriverPresentations { get; set; }
    }
}