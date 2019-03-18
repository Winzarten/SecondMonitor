namespace SecondMonitor.ViewModels.StatusIcon
{
    public class StatusIconViewModel : AbstractViewModel
    {
        private StatusIconState _iconState;
        private string _additionalText;

        public StatusIconState IconState
        {
            get => _iconState;
            set => SetProperty(ref _iconState, value);
        }


        public string AdditionalText
        {
            get => _additionalText;
            set => SetProperty(ref _additionalText, value);
        }
    }
}