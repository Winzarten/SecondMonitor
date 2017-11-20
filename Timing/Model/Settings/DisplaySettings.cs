using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SecondMonitor.DataModel;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.Timing.Annotations;

namespace SecondMonitor.Timing.Model.Settings
{
    public class DisplaySettings : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private TemperatureUnits _temperatureUnits = TemperatureUnits.Celsius;
        public TemperatureUnits TemperatureUnits { get => _temperatureUnits;
            set
            {
                _temperatureUnits = value;
                OnPropertyChanged(); 
            }
        }

        private PressureUnits _pressureUnits = PressureUnits.Kpa;
        public PressureUnits PressureUnits
        {
            get => _pressureUnits;
            set
            {
                _pressureUnits = value;
                OnPropertyChanged(); ;
            }
        }

        private VolumeUnits _volumeUnits = VolumeUnits.Liters;
        public VolumeUnits VolumeUnits
        {
            get => _volumeUnits;
            set
            {
                _volumeUnits = value;
                OnPropertyChanged();;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
