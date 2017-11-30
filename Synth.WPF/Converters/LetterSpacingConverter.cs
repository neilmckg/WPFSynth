using System;
using System.Globalization;
using System.Windows.Data;

namespace Synth.WPF.Converters
{
    [ValueConversion(typeof(string), typeof(int))]
    public class LetterSpacingConverter : IValueConverter
    {
        private static readonly Lazy<LetterSpacingConverter> _preserveCase = new Lazy<LetterSpacingConverter>(() => new LetterSpacingConverter(s => s));
        public static LetterSpacingConverter PreserveCase
        {
            get { return _preserveCase.Value; }
        }

        private static readonly Lazy<LetterSpacingConverter> _toUpperCase = new Lazy<LetterSpacingConverter>(() => new LetterSpacingConverter(s => s.ToUpper()));
        public static LetterSpacingConverter ToUpperCase
        {
            get { return _toUpperCase.Value; }
        }

        private static readonly Lazy<LetterSpacingConverter> _toLowerCase = new Lazy<LetterSpacingConverter>(() => new LetterSpacingConverter(s => s.ToLower()));
        public static LetterSpacingConverter ToLowerCase
        {
            get { return _toLowerCase.Value; }
        }

        private static readonly Lazy<LetterSpacingConverter> _toTitleCase = new Lazy<LetterSpacingConverter>(() => new LetterSpacingConverter(s => CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(s)));
        public static LetterSpacingConverter ToTitleCase
        {
            get { return _toTitleCase.Value; }
        }

        private readonly Func<string, string> _stringFormatter; 

        private LetterSpacingConverter(Func<string, string> stringFormatter)
        {
            _stringFormatter = stringFormatter;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            int spaceWidth = 1;
            if (parameter != null && int.TryParse(parameter.ToString(), out spaceWidth))
                spaceWidth = Math.Max(spaceWidth, 0);

            string source = value.ToString();
            source = _stringFormatter(source);
            char[] letters = source.ToCharArray();
            string spacer = new String(' ', spaceWidth);
            string spaced = string.Join(spacer, letters);

            return spaced;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
