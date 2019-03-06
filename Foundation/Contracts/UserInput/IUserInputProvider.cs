namespace SecondMonitor.Contracts.UserInput
{
    using System.Threading.Tasks;

    public interface IUserInputProvider
    {
        Task<string> GetUserInput(string queryQuestion, string initialValue);
    }
}