namespace ControlTestingApp.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using SecondMonitor.DataModel.BasicProperties;

    public class WheelStatusTestVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _tyreCondition = 50.0;

        public double TyreCondition
        {
            get => _tyreCondition;
            set
            {
                _tyreCondition = value;
                NotifyPropertyChanged();
            }
        }

        public OptimalQuantity<Temperature> TyreCoreTemperature { get; private set; }

        public OptimalQuantity<Temperature> BrakeTemperature { get; private set; }

        public double TyreCoreRawTemperature
        {
            set
            {
                TyreCoreTemperature = new OptimalQuantity<Temperature>()
                                          {
                                              ActualQuantity =
                                                  Temperature.FromCelsius(value),
                                              IdealQuantity =
                                                  Temperature.FromCelsius(80),
                                              IdealQuantityWindow =
                                                  Temperature.FromCelsius(50)
                                          };
                NotifyPropertyChanged(nameof(TyreCoreTemperature));
            }
        }

        public double BrakeRawTemperature
        {
            set
            {
                BrakeTemperature = new OptimalQuantity<Temperature>()
                                          {
                                              ActualQuantity =
                                                  Temperature.FromCelsius(value),
                                              IdealQuantity =
                                                  Temperature.FromCelsius(350),
                                              IdealQuantityWindow =
                                                  Temperature.FromCelsius(150)
                                          };
                NotifyPropertyChanged(nameof(BrakeTemperature));
            }
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}