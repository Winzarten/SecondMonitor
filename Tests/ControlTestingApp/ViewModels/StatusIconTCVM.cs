namespace ControlTestingApp.ViewModels
{
    using SecondMonitor.ViewModels.StatusIcon;

    public class StatusIconTcVm
    {
        public StatusIconTcVm()
        {
            StatusIconViewModel = new StatusIconViewModel()
            {
                IconState = StatusIconState.Unlit,
                AdditionalText = "0.5",
            };
        }

        private int _iconStatusInt;

        public StatusIconViewModel StatusIconViewModel { get; set; }

        public int IconStatusInt
        {
            get => _iconStatusInt;
            set
            {
                _iconStatusInt = value;
                StatusIconViewModel.AdditionalText = (value * 10).ToString();
                switch (_iconStatusInt)
                {
                    case 1:
                        StatusIconViewModel.IconState = StatusIconState.Ok;
                        break;
                    case 2:
                        StatusIconViewModel.IconState = StatusIconState.Warning;
                        break;
                    case 3:
                        StatusIconViewModel.IconState = StatusIconState.Error;
                        break;
                    case 4:
                        StatusIconViewModel.IconState = StatusIconState.Information;
                        break;
                    default:
                        StatusIconViewModel.IconState = StatusIconState.Unlit;
                        break;
                }
            }
        }
    }
}