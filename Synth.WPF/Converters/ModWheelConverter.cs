using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Synth.WPF.Controls;

namespace Synth.WPF.Converters
{
    public class ModWheelConverter : IValueConverter
    {
        private const double FullThumbHeight = 30;

        #region singletons

        private static readonly Lazy<ModWheelConverter> _thumbSizeInstance = new Lazy<ModWheelConverter>(() => new ModWheelConverter(CalculateThumbSize));
        public static ModWheelConverter ThumbSize => _thumbSizeInstance.Value;

        private static readonly Lazy<ModWheelConverter> _trackOffsetInstance = new Lazy<ModWheelConverter>(() => new ModWheelConverter(CalculateTrackOffset));
        public static ModWheelConverter TrackOffset => _trackOffsetInstance.Value;

        #endregion singletons

        #region static calculation methods

        private static double CalculateThumbSize(MeteredSlider slider)
        {
            double distanceFromCenter = Math.Abs(slider.SliderPosition - 0.5);
            double angleInRadians = Math.Asin(2 * distanceFromCenter * 0.8);
            double targetSize = Math.Cos(angleInRadians) * FullThumbHeight;
            return targetSize;
        }

        private static double CalculateTrackOffset(MeteredSlider slider)
        {
            double arbitraryScalingFactor = 1.4;        // don't like this!
            double targetOffset = -slider.ActualHeight * slider.SliderPosition / arbitraryScalingFactor;
            return targetOffset;
        }
  
        #endregion static calculation methods

        private readonly Func<MeteredSlider, double> _conversion; 

        private ModWheelConverter(Func<MeteredSlider, double> conversion)
        {
            _conversion = conversion;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // The value input is not actually used (because the conversion uses properties of the slider, which comes in as the parameter 
            //  input), but it should be the slider's Value property, which will force this calculation to be rerun at the right moments.

            // The parameter input is a MeteredSlider control, or a FrameworkElement whose datacontext is a reference to the whole slider, 
            //  either of which must be passed in using x:Reference because binding doesn't work on the parameter input.

            MeteredSlider slider = (parameter as MeteredSlider) ?? (parameter as FrameworkElement)?.DataContext as MeteredSlider;

            if (slider == null)
            {
                Console.WriteLine("The ModWheelConverters require, as a parameter, a FrameworkElement whose datacontext is a reference to the MeteredSlider control.");
                return FullThumbHeight;
            }

            double result = _conversion(slider);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
