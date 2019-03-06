namespace SecondMonitor.WindowsControls.WPF.UserInput
{
    using System.Threading.Tasks;
    using Contracts.UserInput;

    public class DialogUserInputProvider : IUserInputProvider
    {
        public Task<string> GetUserInput(string queryQuestion, string initialValue)
        {
            UserInputViewModel userInputViewModel = new UserInputViewModel()
            {
                UserQuestion = queryQuestion,
                UserInput = initialValue,
            };

            UserInputWindow userInputWindow = new UserInputWindow();
            userInputWindow.DataContext = userInputViewModel;
            return Task.FromResult(userInputWindow.ShowDialog() == true ? userInputViewModel.UserInput : initialValue);
        }
    }
}