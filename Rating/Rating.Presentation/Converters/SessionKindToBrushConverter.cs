namespace SecondMonitor.Rating.Presentation.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using Application.Controller.RaceObserver.States;

    public class SessionKindToBrushConverter : IValueConverter
    {
        private static readonly  ResourceDictionary ColorResourceDictionary = new ResourceDictionary
        {
            Source = new Uri(@"pack://application:,,,/Rating.Presentation;component/RatingColors.xaml",UriKind.RelativeOrAbsolute)
        };


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SessionKind sessionKind)
            {
                switch (sessionKind)
                {
                    case SessionKind.Idle:
                        return ColorResourceDictionary["IdleStateBrush"];
                    case SessionKind.Qualification:
                        return ColorResourceDictionary["QualificationStateBrush"];
                    case SessionKind.RaceWithQualification:
                        return ColorResourceDictionary["RaceWQualificationStateBrush"];
                    case SessionKind.RaceWithoutQualification:
                        return ColorResourceDictionary["RaceWithoutQualificationStateBrush"];
                    case SessionKind.OtherSession:
                        return ColorResourceDictionary["AnyOtherSessionBrush"];
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