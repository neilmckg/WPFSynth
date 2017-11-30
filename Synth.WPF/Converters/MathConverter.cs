using System;
using System.Globalization;
using System.Windows.Data;

namespace Synth.WPF.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class MathConverter : IValueConverter
    {
        #region static singleton factories

        private static readonly Lazy<MathConverter> _multiply_instance = new Lazy<MathConverter>(() => new MathConverter((value, parameter) => value * parameter));
        public static MathConverter Multiply
        {
            get { return _multiply_instance.Value; }
        }

        private static readonly Lazy<MathConverter> _add_instance = new Lazy<MathConverter>(() => new MathConverter((value, parameter) => value + parameter));
        public static MathConverter Add
        {
            get { return _add_instance.Value; }
        }

        private static readonly Lazy<MathConverter> _subtract_instance = new Lazy<MathConverter>(() => new MathConverter((value, parameter) => value - parameter));
        public static MathConverter Subtract
        {
            get { return _subtract_instance.Value; }
        }

        private static readonly Lazy<MathConverter> _subtractFrom_instance = new Lazy<MathConverter>(() => new MathConverter((value, parameter) => parameter - value));
        public static MathConverter SubtractFrom
        {
            get { return _subtractFrom_instance.Value; }
        }

        private static readonly Lazy<MathConverter> _divideBy_instance = new Lazy<MathConverter>(() => new MathConverter((value, parameter) => value / parameter));
        public static MathConverter DivideBy
        {
            get { return _divideBy_instance.Value; }
        }

        private static readonly Lazy<MathConverter> _divideInto_instance = new Lazy<MathConverter>(() => new MathConverter((value, parameter) => parameter / value));
        public static MathConverter DivideInto
        {
            get { return _divideInto_instance.Value; }
        }

        private static readonly Lazy<MathConverter> _abs_instance = new Lazy<MathConverter>(() => new MathConverter((value, parameter) => Math.Abs(value)));
        public static MathConverter AbsoluteValue
        {
            get { return _abs_instance.Value; }
        }

        // TODO add more operations, binary and unary

        #endregion static singleton factories

        private readonly Func<double, double, double> _operation;

        private MathConverter(Func<double, double, double> operation)
        {
            _operation = operation;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double valueNum;
            double paramNum;

            if (value != null && double.TryParse(value.ToString(), out valueNum))
            {
                if (parameter != null && double.TryParse(parameter.ToString(), out paramNum))
                {
                    double result = _operation(valueNum, paramNum);
                    return result;
                }
                else
                {
                    throw new ArgumentException("parameter should be numeric.");
                }
            }
            else
            {
                throw new ArgumentException("Value should be numeric.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
