using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using Synth.WPF.Util;

namespace Synth.WPF.Controls
{
    /// <summary>
    ///  This converter is meant to be used only within the MeteredSlider control, to mediate between its value and its slider position. 
    ///  For other range conversions use the RangeConverter class, as it is used internally by ValueToSliderPositionConverter.
    /// </summary>
    public class ValueToTickLocationConverter : IValueConverter
    {
        private static readonly Lazy<ValueToTickLocationConverter> _instance = new Lazy<ValueToTickLocationConverter>(() => new ValueToTickLocationConverter());
        public static ValueToTickLocationConverter Instance
        {
            get { return _instance.Value; }
        }

        private readonly RangeConverter _converter = new RangeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double dblValue;

            if (value == null)
                return value;
            if (!double.TryParse(value.ToString(), out dblValue))
                return value;

            MeteredSlider ss = parameter as MeteredSlider;
            FrameworkElement fe = parameter as FrameworkElement;
            if (ss == null && fe != null)
            {
                if (fe.DataContext is MeteredSlider)
                    ss = fe.DataContext as MeteredSlider;
                if (fe.Tag is MeteredSlider)
                    ss = fe.Tag as MeteredSlider;
            }

            if (ss == null)
                return 0;

            _converter.SourceMin = ss.Minimum;
            _converter.SourceMax = ss.Maximum;
            _converter.ScaleFactor = ss.ScaleFactor;

            double result = _converter.SourceToTarget(dblValue);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double dblValue;

            if (value == null)
                return value;
            if (!double.TryParse(value.ToString(), out dblValue))
                return value;

            MeteredSlider ss = parameter as MeteredSlider;
            if (ss == null)
                return 0;

            _converter.SourceMin = ss.Minimum;
            _converter.SourceMax = ss.Maximum;
            _converter.ScaleFactor = ss.ScaleFactor;

            return _converter.TargetToSource(dblValue);
        }
    }
}
