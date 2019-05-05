namespace SecondMonitor.ViewModels
{
    using System;
    using System.Windows;

    public interface IWindowService
    {
        Window OpenWindow(IViewModel viewModel, string title);

        Window OpenWindow(IViewModel viewModel, string title, WindowState startState, SizeToContent sizeToContent);

        Window OpenWindow(IViewModel viewModel, string title, WindowState startState, SizeToContent sizeToContent, Action onClose);
    }
}