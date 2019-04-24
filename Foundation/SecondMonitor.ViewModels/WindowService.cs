namespace SecondMonitor.ViewModels
{
    using System;
    using System.Windows;

    public class WindowService : IWindowService
    {
        public Window OpenWindow(IViewModel viewModel, string title) => OpenWindow(viewModel, title, WindowState.Normal, SizeToContent.WidthAndHeight);

        public Window OpenWindow(IViewModel viewModel, string title, WindowState startState, SizeToContent sizeToContent)
        {
            Window window = new Window() {WindowState = startState, Title = title,  Content = viewModel, SizeToContent = sizeToContent };
            window.Closed += WindowOnClosed;
            window.Show();
            return window;
        }

        public Window OpenWindow(IViewModel viewModel, string title, WindowState startState, SizeToContent sizeToContent, Action onClose)
        {
            Window window = new Window() { WindowState = startState, Title = title, Content = viewModel, SizeToContent = sizeToContent };
            window.Closed += (sender, e) =>
            {
                if (!(sender is Window sWindow))
                {
                    return;
                }
                onClose();
                sWindow.Content = null;

            };
            window.Show();
            return window;
        }

        private void WindowOnClosed(object sender, EventArgs e)
        {
            if (!(sender is Window window))
            {
                return;
            }

            window.Content = null;
            window.Closed -= WindowOnClosed;
        }
    }
}