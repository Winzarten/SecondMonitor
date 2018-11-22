using System;
using System.Collections.Generic;

namespace SecondMonitor.DataModel.DriversPresentation
{
    [Serializable]
    public class DriverPresentationsDTO
    {
        public DriverPresentationsDTO()
        {
            DriverPresentations = new List<DriverPresentationDTO>();
        }

        public List<DriverPresentationDTO> DriverPresentations { get; set; }
    }
}