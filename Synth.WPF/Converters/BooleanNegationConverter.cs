using System;
using System.Globalization;
using System.Windows.Data;

namespace Synth.WPF.Converters
{
    public class BooleanNegationConverter : IValueConverter
    {
        private static readonly Lazy<BooleanNegationConverter> _instance = new Lazy<BooleanNegationConverter>(() => new BooleanNegationConverter());

        public static BooleanNegationConverter Instance
        {
            get { return _instance.Value; }
        }

        private BooleanNegationConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue;
            if (value is bool)
                boolValue = (bool) value;
            else if (!bool.TryParse(value.ToString(), out boolValue))
                throw new ArgumentException("BooleanNegationConverter requires a boolean value.");

            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
