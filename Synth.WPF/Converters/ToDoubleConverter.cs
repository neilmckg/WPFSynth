using System;
using System.Globalization;
using System.Windows.Data;

namespace Synth.WPF.Converters
{
    public class ToDoubleConverter : IValueConverter
    {
        private static readonly Lazy<ToDoubleConverter> _instance = new Lazy<ToDoubleConverter>(() => new ToDoubleConverter());

        public static ToDoubleConverter Instance
        {
            get { return _instance.Value; }
        }

        private ToDoubleConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double nullValue;
            if (parameter == null)
                nullValue = 0;
            else if (parameter is double)
                nullValue = (double)parameter;
            else if (!double.TryParse(parameter.ToString(), out nullValue))
                nullValue = 0;

            double dblValue;

            if (value == null)
                return nullValue;
            else if (value is double)
                return (double) value;
            else if (double.TryParse(value.ToString(), out dblValue))
                return dblValue;
            else
                throw new ArgumentException("Value cannot reasonably be converted to a double.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
