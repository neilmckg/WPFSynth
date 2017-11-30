using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Synth.WPF.Converters
{
    /// <summary>
    /// Hides or collapses a control when a bound value is equal (or not) to a fixed parameter value.
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ValueToVisibilityConverter : IValueConverter
    {
        #region static singleton factories

        private static readonly Lazy<ValueToVisibilityConverter> _collapsedWhenEqual = new Lazy<ValueToVisibilityConverter>(() => new ValueToVisibilityConverter(false, Visibility.Collapsed));
        public static ValueToVisibilityConverter CollapsedWhenEqual
        {
            get { return _collapsedWhenEqual.Value; }
        }

        private static readonly Lazy<ValueToVisibilityConverter> _collapsedWhenNotEqual = new Lazy<ValueToVisibilityConverter>(() => new ValueToVisibilityConverter(true, Visibility.Collapsed));
        public static ValueToVisibilityConverter CollapsedWhenNotEqual
        {
            get { return _collapsedWhenNotEqual.Value; }
        }

        private static readonly Lazy<ValueToVisibilityConverter> _hiddenWhenEqual = new Lazy<ValueToVisibilityConverter>(() => new ValueToVisibilityConverter(false, Visibility.Hidden));
        public static ValueToVisibilityConverter HiddenWhenEqual
        {
            get { return _hiddenWhenEqual.Value; }
        }

        private static readonly Lazy<ValueToVisibilityConverter> _hiddenWhenNotEqual = new Lazy<ValueToVisibilityConverter>(() => new ValueToVisibilityConverter(true, Visibility.Hidden));
        public static ValueToVisibilityConverter HiddenWhenNotEqual
        {
            get { return _hiddenWhenNotEqual.Value; }
        }

        #endregion static singleton factories

        private readonly Visibility _hiddenVisibility;
        private readonly bool _visibleWhenEqual;

        private ValueToVisibilityConverter(bool visibleWhenEqual, Visibility hiddenVisibility)
        {
            _hiddenVisibility = hiddenVisibility;
            _visibleWhenEqual = visibleWhenEqual;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool valuesAreEqual;

            if (value == null)
            {
                valuesAreEqual = (parameter == null);
            }
            else if (parameter == null)
            {
                valuesAreEqual = false;
            }
            else if (string.Compare(value.ToString(), parameter.ToString(), StringComparison.InvariantCulture) == 0)
            {
                valuesAreEqual = true;
            }
            else
            {
                valuesAreEqual = false;
            }

            bool makeVisible;
            if (_visibleWhenEqual)
            {
                makeVisible = valuesAreEqual;
            }
            else
            {
                makeVisible = !valuesAreEqual;
            }

            if (makeVisible)
            {
                return Visibility.Visible;
            }
            else
            {
                return _hiddenVisibility;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack not supported.");
        }
    }
}
