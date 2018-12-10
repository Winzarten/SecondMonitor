using System.Linq;
using System.Windows.Media;
using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.DataModel.DriversPresentation;

namespace SecondMonitor.SimdataManagement.DriverPresentation
{
    public class DriverPresentationsManager
    {
        private readonly DriverPresentationsLoader _driverPresentationsLoader;
        private DriverPresentationsDTO _driverPresentationsDto;


        public DriverPresentationsManager(DriverPresentationsLoader driverPresentationsLoader)
        {
            _driverPresentationsLoader = driverPresentationsLoader;
        }

        private DriverPresentationsDTO DriverPresentationsDto => _driverPresentationsDto ?? LoadDriverPresentations();

        public bool TryGetOutLineColor(string driverName, out Color color)
        {
            DriverPresentationDTO driverPresentation = GetDriverPresentation(driverName);
            color = driverPresentation?.OutLineColor?.ToColor() ?? Colors.Transparent;
            return driverPresentation?.OutLineColor != null;
        }

        public bool IsCustomOutlineEnabled(string driverName)
        {
            DriverPresentationDTO driverPresentation = GetDriverPresentation(driverName);
            return driverPresentation?.CustomOutLineEnabled ?? false;
        }


        public void SetOutLineColorEnabled(string driverName, bool isEnabled)
        {
            DriverPresentationDTO driverPresentation = GetDriverOrCreatePresentation(driverName);
            driverPresentation.CustomOutLineEnabled = isEnabled;
            _driverPresentationsLoader.Save(DriverPresentationsDto);

        }

        public void SetOutLineColor(string driverName, Color color)
        {
            DriverPresentationDTO driverPresentation = GetDriverOrCreatePresentation(driverName);
            driverPresentation.OutLineColor = ColorDTO.FromColor(color);
            _driverPresentationsLoader.Save(DriverPresentationsDto);
        }

        private DriverPresentationDTO GetDriverPresentation(string driverName)
        {
            return DriverPresentationsDto.DriverPresentations.FirstOrDefault(x => x.DriverName == driverName);
        }

        public DriverPresentationDTO GetDriverOrCreatePresentation(string driverName)
        {
            DriverPresentationDTO driverPresentation = GetDriverPresentation(driverName);
            if (driverPresentation == null)
            {
                driverPresentation = new DriverPresentationDTO()
                {
                    DriverName = driverName
                };
                DriverPresentationsDto.DriverPresentations.Add(driverPresentation);
            }

            return driverPresentation;
        }
        private DriverPresentationsDTO LoadDriverPresentations()
        {
            if (!_driverPresentationsLoader.TryLoad(out _driverPresentationsDto))
            {
                _driverPresentationsDto = new DriverPresentationsDTO();
            }

            return _driverPresentationsDto;
        }
    }
}