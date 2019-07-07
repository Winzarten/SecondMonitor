namespace SecondMonitor.WindowsControls.Extension
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    public static class DependencyObjectExtension
    {
        public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject dependencyObject)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                yield return VisualTreeHelper.GetChild(dependencyObject, i);
            }

            if (dependencyObject is ContentPresenter contentPresenter && contentPresenter.Content is DependencyObject contentDependencyObject)
            {
                yield return contentDependencyObject; ;
            }
        }



        public static IEnumerable<DependencyObject> EnumerateVisualDescendents(this DependencyObject dependencyObject)
        {
            yield return dependencyObject;

            foreach (DependencyObject child in dependencyObject.EnumerateVisualChildren())
            {
                foreach (DependencyObject descendent in child.EnumerateVisualDescendents())
                {
                    yield return descendent;
                }
            }
        }

        public static void UpdateBindingSources(this DependencyObject dependencyObject)
        {
            foreach (DependencyObject element in dependencyObject.EnumerateVisualDescendents())
            {
                LocalValueEnumerator localValueEnumerator = element.GetLocalValueEnumerator();
                while (localValueEnumerator.MoveNext())
                {
                    BindingExpressionBase bindingExpression = BindingOperations.GetBindingExpressionBase(element, localValueEnumerator.Current.Property);
                    if (bindingExpression != null)
                    {
                        bindingExpression.UpdateTarget();
                    }
                }
            }
        }

        public static IEnumerable<DependencyObject> GetAllDescendantWithProperty(this DependencyObject dependencyObject, string propertyName)
        {
            foreach (DependencyObject element in dependencyObject.EnumerateVisualDescendents())
            {
                LocalValueEnumerator localValueEnumerator = element.GetLocalValueEnumerator();
                while (localValueEnumerator.MoveNext())
                {
                    if (localValueEnumerator.Current.Property.Name == propertyName)
                    {
                        yield return element;
                    }
                }
            }
        }
    }
}