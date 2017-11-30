using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Synth.WPF.Converters
{
    public class IsPropertyBoundToBooleanConverter : IValueConverter
    {
        #region static singleton factories

        private static readonly Lazy<IsPropertyBoundToBooleanConverter> _trueIfBound = new Lazy<IsPropertyBoundToBooleanConverter>(() => new IsPropertyBoundToBooleanConverter(true));
        public static IsPropertyBoundToBooleanConverter TrueIfBound
        {
            get { return _trueIfBound.Value; }
        }

        private static readonly Lazy<IsPropertyBoundToBooleanConverter> _falseIfBound = new Lazy<IsPropertyBoundToBooleanConverter>(() => new IsPropertyBoundToBooleanConverter(false));
        public static IsPropertyBoundToBooleanConverter FalseIfBound
        {
            get { return _falseIfBound.Value; }
        }
        
        #endregion static singleton factories

        private readonly bool _valueWhenBound;

        private IsPropertyBoundToBooleanConverter(bool valueWhenBound)
        {
            _valueWhenBound = valueWhenBound;
        }

        /// <summary>
        /// given a dependency object and a dependency property, returns value indicating whether the property is currently bound.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBound = !_valueWhenBound;

            if (value is DependencyObject && parameter is DependencyProperty)
            {
                BindingExpression bindingExpr = BindingOperations.GetBindingExpression(value as DependencyObject, parameter as DependencyProperty);
                if (bindingExpr != null && bindingExpr.ResolvedSource != null)
                    isBound = _valueWhenBound;
            }

            return isBound;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
