using System;
using System.Globalization;
using System.Windows.Data;

namespace Synth.WPF.Converters
{
    public class TextToBooleanConverter : IValueConverter
    {
        #region static singleton factories

        private static TextToBooleanConverter _stringContains;
        public static TextToBooleanConverter StringContains
        {
            get { return _stringContains ?? (_stringContains = new TextToBooleanConverter((s, p) => s.Contains(p))); }
        }

        #endregion static singleton factories

        private readonly Func<string, string, bool> _textFunc;

        private TextToBooleanConverter(Func<string, string, bool> textFunc)
        {
            _textFunc = textFunc;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            if (parameter == null)
                return false;

            return _textFunc(value.ToString(), parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
