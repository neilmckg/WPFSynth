using System;
using System.Globalization;
using System.Windows.Data;

namespace Synth.WPF.Converters
{
    public class WritableDoubleSettingConverter : IValueConverter
    {
        private static readonly Lazy<WritableDoubleSettingConverter> _instance = new Lazy<WritableDoubleSettingConverter>(() => new WritableDoubleSettingConverter());

        public static WritableDoubleSettingConverter Instance
        {
            get { return _instance.Value; }
        }

        private WritableDoubleSettingConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if the source value is a valid double, just pass it through. Otherwise, read it from the settings file.
            if (value != null && value is double && !double.IsNaN((double) value))
                return value;
            else if (parameter == null)
                return value;
            else if (string.IsNullOrWhiteSpace(parameter.ToString()))
                return value;
            else
                return Properties.Settings.Default[parameter.ToString()];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Just pass the value through, but first save it to the settings file.
            if (parameter != null && !string.IsNullOrWhiteSpace(parameter.ToString()))
            {
                Properties.Settings.Default[parameter.ToString()] = value;
                Properties.Settings.Default.Save();
            }

            return value;
        }
    }
}
