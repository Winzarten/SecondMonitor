namespace SecondMonitor.Rating.Presentation.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using Application.Controller.RaceObserver.States;

    public class SessionPhaseToBrushConverter : IValueConverter
    {
        private static readonly ResourceDictionary ColorResourceDictionary = new ResourceDictionary
        {
            Source = new Uri(@"pack://application:,,,/Rating.Presentation;component/RatingColors.xaml", UriKind.RelativeOrAbsolute)
        };


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SessionPhaseKind sessionPhase)
            {
                switch (sessionPhase)
                {
                    case SessionPhaseKind.None:
                        return ColorResourceDictionary["IdlePhaseBrush"];
                    case SessionPhaseKind.NotStarted:
                        return ColorResourceDictionary["NotStartedPhaseBrush"];
                    case SessionPhaseKind.FreeRestartPeriod:
                        return ColorResourceDictionary["FreeRestartWindowPhaseBrush"];
                    case SessionPhaseKind.InProgress:
                        return ColorResourceDictionary["StartedPhaseBrush"];
                    default:
                        return Brushes.White;
                }
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}