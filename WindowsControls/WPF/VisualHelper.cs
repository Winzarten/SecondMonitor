namespace SecondMonitor.WindowsControls.WPF
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// The visual helper.
    /// </summary>
    /// Source: https://stackoverflow.com/questions/15641473/how-to-automatically-scale-font-size-for-a-group-of-controls
    public class VisualHelper
    {
        public static List<T> FindVisualChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            List<T> children = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var o = VisualTreeHelper.GetChild(obj, i);
                if (o is T variable)
                {
                    children.Add(variable);
                }

                children.AddRange(FindVisualChildren<T>(o)); // recursive
            }
            return children;
        }

        public static T FindUpVisualTree<T>(DependencyObject initial) where T : DependencyObject
        {
            DependencyObject current = initial;

            while (current != null && current.GetType() != typeof(T))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as T;
        }
    }
}