namespace SecondMonitor.ViewModels.Factory
{
    using System.Collections.Generic;
    using ViewModels;

    public interface IViewModelFactory
    {
        T Create<T>() where T : IViewModel;

        IEnumerable<T> CreateAll<T>() where T : IViewModel;
    }
}