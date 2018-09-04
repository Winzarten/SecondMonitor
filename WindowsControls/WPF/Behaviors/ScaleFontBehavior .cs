namespace SecondMonitor.WindowsControls.WPF.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    using LiveCharts.Wpf;

    ///Source: https://stackoverflow.com/questions/15641473/how-to-automatically-scale-font-size-for-a-group-of-controls
    public class ScaleFontBehavior : Behavior<TextBlock>
    {
        // MaxFontSize
        public static readonly DependencyProperty MaxFontSizeProperty = DependencyProperty.Register(
            "MaxFontSize",
            typeof(double),
            typeof(ScaleFontBehavior),
            new PropertyMetadata(20d));

        private string _oldValue = string.Empty;

        public double MaxFontSize
        {
            get => (double)GetValue(MaxFontSizeProperty);
            set => SetValue(MaxFontSizeProperty, value);
        }

        protected override void OnAttached()
        {

            DependencyPropertyDescriptor pd = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
            pd.AddValueChanged(AssociatedObject, TextBlockValueChanged);

            Control parent = (Control)VisualTreeHelper.GetParent(AssociatedObject);

            if (parent == null)
            {
                parent.SizeChanged += (s, e) => { CalculateFontSize(); };
            }
        }

        private void TextBlockValueChanged(object sender, EventArgs e)
        {
            if (AssociatedObject == null)
            {
                return;
            }

            if (_oldValue.Length == AssociatedObject.Text.Length)
            {
                return;
            }

            CalculateFontSize();
        }

        private void CalculateFontSize()
        {
            if (AssociatedObject == null)
            {
                return;
            }

            double fontSize = this.MaxFontSize;
            TextBlock tb = AssociatedObject;
            Control parent = (Control)VisualTreeHelper.GetParent(AssociatedObject);

            if (parent == null)
            {
                return;
            }

            // get grid height (if limited)
            double gridHeight = parent.ActualHeight;


            // get desired size with fontsize = MaxFontSize
            Size desiredSize = MeasureText(tb);
            double widthMargins = tb.Margin.Left + tb.Margin.Right;
            double heightMargins = tb.Margin.Top + tb.Margin.Bottom;

            double desiredHeight = desiredSize.Height + heightMargins;
            double desiredWidth = desiredSize.Width + widthMargins;

            // adjust fontsize if text would be clipped vertically
            if (gridHeight < desiredHeight)
            {
                double factor = (desiredHeight - heightMargins)
                                / (this.AssociatedObject.ActualHeight - heightMargins);
                fontSize = Math.Min(fontSize, MaxFontSize / factor);
            }

            // get column width (if limited)
            double colWidth = parent.ActualWidth;

            // adjust fontsize if text would be clipped horizontally
            if (colWidth < desiredWidth)
            {
                double factor = (desiredWidth - widthMargins) / (colWidth- widthMargins);
                fontSize = Math.Min(fontSize, MaxFontSize / factor);
            }

            if (fontSize != 0)
            {
                tb.FontSize = fontSize;
            }

        }

        // Measures text size of textblock
        private Size MeasureText(TextBlock tb)
        {
            var formattedText = new FormattedText(
                tb.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                this.MaxFontSize,
                Brushes.Black); // always uses MaxFontSize for desiredSize

            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}