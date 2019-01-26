using System.Linq;
using System.Windows.Media;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.DriversPresentation;

namespace SecondMonitor.SimdataManagement.DriverPresentation
{
    public class DriverPresentationsManager
    {
        private readonly DriverPresentationsLoader _driverPresentationsLoader;
        private DriverPresentationsDto _driverPresentationsDto;


        public DriverPresentationsManager(DriverPresentationsLoader driverPresentationsLoader)
        {
            _driverPresentationsLoader = driverPresentationsLoader;
        }

        private DriverPresentationsDto DriverPresentationsDto => _driverPresentationsDto ?? LoadDriverPresentations();

        public bool TryGetOutLineColor(string driverName, out Color color)
        {
            DriverPresentationDto driverPresentation = GetDriverPresentation(driverName);
            color = driverPresentation?.OutLineColor?.ToColor() ?? Colors.Transparent;
            return driverPresentation?.OutLineColor != null;
        }

        public bool IsCustomOutlineEnabled(string driverName)
        {
            DriverPresentationDto driverPresentation = GetDriverPresentation(driverName);
            return driverPresentation?.CustomOutLineEnabled ?? false;
        }


        public void SetOutLineColorEnabled(string driverName, bool isEnabled)
        {
            DriverPresentationDto driverPresentation = GetDriverOrCreatePresentation(driverName);
            driverPresentation.CustomOutLineEnabled = isEnabled;
            _driverPresentationsLoader.Save(DriverPresentationsDto);

        }

        public void SetOutLineColor(string driverName, Color color)
        {
            DriverPresentationDto driverPresentation = GetDriverOrCreatePresentation(driverName);
            driverPresentation.OutLineColor = ColorDto.FromColor(color);
            _driverPresentationsLoader.Save(DriverPresentationsDto);
        }

        private DriverPresentationDto GetDriverPresentation(string driverName)
        {
            return DriverPresentationsDto.DriverPresentations.FirstOrDefault(x => x.DriverName == driverName);
        }

        public DriverPresentationDto GetDriverOrCreatePresentation(string driverName)
        {
            DriverPresentationDto driverPresentation = GetDriverPresentation(driverName);
            if (driverPresentation == null)
            {
                driverPresentation = new DriverPresentationDto()
                {
                    DriverName = driverName
                };
                DriverPresentationsDto.DriverPresentations.Add(driverPresentation);
            }

            return driverPresentation;
        }
        private DriverPresentationsDto LoadDriverPresentations()
        {
            if (!_driverPresentationsLoader.TryLoad(out _driverPresentationsDto))
            {
                _driverPresentationsDto = new DriverPresentationsDto();
            }

            return _driverPresentationsDto;
        }
    }
}