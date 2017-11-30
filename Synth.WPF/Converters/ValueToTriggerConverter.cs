using System;
using System.Globalization;
using System.Windows.Data;

namespace Synth.WPF.Converters
{
    public class ValueToTriggerConverter : IValueConverter
    {
        #region static singleton factories

        private static readonly Lazy<ValueToTriggerConverter> _triggerWhenEqual = new Lazy<ValueToTriggerConverter>(() => new ValueToTriggerConverter((val, thresh) => val == thresh));
        public static ValueToTriggerConverter TriggerWhenEqual
        {
            get { return _triggerWhenEqual.Value; }
        }

        private static readonly Lazy<ValueToTriggerConverter> _triggerWhenNotEqual = new Lazy<ValueToTriggerConverter>(() => new ValueToTriggerConverter((val, thresh) => val != thresh));
        public static ValueToTriggerConverter TriggerWhenNotEqual
        {
            get { return _triggerWhenNotEqual.Value; }
        }

        private static readonly Lazy<ValueToTriggerConverter> _triggerWhenLessThan = new Lazy<ValueToTriggerConverter>(() => new ValueToTriggerConverter((val, thresh) => val < thresh));
        public static ValueToTriggerConverter TriggerWhenLessThan
        {
            get { return _triggerWhenLessThan.Value; }
        }

        private static readonly Lazy<ValueToTriggerConverter> _triggerWhenGreaterThan = new Lazy<ValueToTriggerConverter>(() => new ValueToTriggerConverter((val, thresh) => val > thresh));
        public static ValueToTriggerConverter TriggerWhenGreaterThan
        {
            get { return _triggerWhenGreaterThan.Value; }
        }

        private static readonly Lazy<ValueToTriggerConverter> _triggerWhenLessThanAbs = new Lazy<ValueToTriggerConverter>(() => new ValueToTriggerConverter((val, thresh) => Math.Abs(val) < Math.Abs(thresh)));
        public static ValueToTriggerConverter TriggerWhenLessThanAbsoluteValue
        {
            get { return _triggerWhenLessThanAbs.Value; }
        }

        private static readonly Lazy<ValueToTriggerConverter> _triggerWhenGreaterThanAbs = new Lazy<ValueToTriggerConverter>(() => new ValueToTriggerConverter((val, thresh) => Math.Abs(val) > Math.Abs(thresh)));
        public static ValueToTriggerConverter TriggerWhenGreaterThanAbsoluteValue
        {
            get { return _triggerWhenGreaterThanAbs.Value; }
        }

        #endregion static singleton factories

        private const int COMPARISON_PRECISION = 6;
        private readonly Func<double, double, bool> _triggerFunction;

        private ValueToTriggerConverter(Func<double, double, bool> triggerFunction)
        {
            _triggerFunction = triggerFunction;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double dblValue;
            double threshold;

            if (value == null)
                return false;
            else if (value is double)
                dblValue = (double)value;
            else if (!double.TryParse(value.ToString(), out dblValue))
                return false;

            if (parameter == null)
                threshold = 0;
            else if (parameter is double)
                threshold = (double)parameter;
            else if (!double.TryParse(parameter.ToString(), out threshold))
                return false;

            dblValue = Math.Round(dblValue, COMPARISON_PRECISION);
            threshold = Math.Round(threshold, COMPARISON_PRECISION);

            return _triggerFunction(dblValue, threshold);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
