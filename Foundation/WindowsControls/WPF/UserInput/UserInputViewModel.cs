namespace SecondMonitor.WindowsControls.WPF.UserInput
{
    using ViewModels;

    public class UserInputViewModel : AbstractViewModel
    {
        private string _userInput;
        private string _userQuestion;

        public string UserInput
        {
            get => _userInput;
            set => SetProperty(ref _userInput, value);
        }

        public string UserQuestion
        {
            get => _userQuestion;
            set => SetProperty(ref _userQuestion, value);
        }
    }
}