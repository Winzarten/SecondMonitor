using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecondMonitor.DataModel;
using SecondMonitor.DataModel.BasicProperties;

namespace SecondMonitor.Timing.Model.Settings
{
    public class DisplaySettings
    {
        public class DisplaySettingsChangedArgs : EventArgs
        {
            public DisplaySettings Settings { get; private set; }
            public DisplaySettingsChangedArgs(DisplaySettings settings)
            {
                Settings = settings;
            }
        }

        public event EventHandler<DisplaySettingsChangedArgs> DisplaySettingsChanged;

        private TemperatureUnits temperatureUnits = TemperatureUnits.Celsius;
        public TemperatureUnits TemperatureUnits { get => temperatureUnits;
            set
            {
                temperatureUnits = value;
                RaiseSettingsChangedEvent();
            }
        }

        private PressureUnits pressureUnits = PressureUnits.Kpa;
        public PressureUnits PressureUnits
        {
            get => pressureUnits;
            set
            {
                pressureUnits = value;
                RaiseSettingsChangedEvent();
            }
        }

        private VolumeUnits volumeUnits = VolumeUnits.Liters;
        public VolumeUnits VolumeUnits
        {
            get => volumeUnits;
            set
            {
                volumeUnits = value;
                RaiseSettingsChangedEvent();
            }
        }

        private void RaiseSettingsChangedEvent()
        {
            DisplaySettingsChanged?.Invoke(this, new DisplaySettingsChangedArgs(this));
        }
    }
}
