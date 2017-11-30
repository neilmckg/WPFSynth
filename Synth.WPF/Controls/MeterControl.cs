using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Synth.WPF.Controls
{
    public class MeterControl : Control
    {
        #region dependency properties

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(MeterControl), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HandleRangePropertyChanged));
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static readonly DependencyPropertyKey MeterPositionPropertyKey = DependencyProperty.RegisterReadOnly("MeterPosition", typeof(double), typeof(MeterControl), new PropertyMetadata(0d));
        public static readonly DependencyProperty MeterPositionProperty = MeterPositionPropertyKey.DependencyProperty;
        public double MeterPosition
        {
            get { return (double)GetValue(MeterPositionProperty); }
            private set { SetValue(MeterPositionPropertyKey, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(MeterControl), new PropertyMetadata(0d, HandleRangePropertyChanged));
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(MeterControl), new PropertyMetadata(1d, HandleRangePropertyChanged));
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty OverflowIndicatorProperty = DependencyProperty.Register("OverflowIndicator", typeof(bool), typeof(MeterControl), new PropertyMetadata(false));
        public bool OverflowIndicator
        {
            get { return (bool)GetValue(OverflowIndicatorProperty); }
            set { SetValue(OverflowIndicatorProperty, value); }
        }

        private static readonly DependencyPropertyKey IsOverflowedPropertyKey = DependencyProperty.RegisterReadOnly("IsOverflowed", typeof(bool), typeof(MeterControl), new PropertyMetadata(false));
        public static readonly DependencyProperty IsOverflowedProperty = IsOverflowedPropertyKey.DependencyProperty;
        public bool IsOverflowed
        {
            get { return (bool)GetValue(IsOverflowedProperty); }
            private set { SetValue(IsOverflowedPropertyKey, value); }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(MeterControl), new PropertyMetadata(true));
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty MeterColorProperty = DependencyProperty.Register("MeterColor", typeof(Color), typeof(MeterControl), new PropertyMetadata(Colors.Green));
        public Color MeterColor
        {
            get { return (Color)GetValue(MeterColorProperty); }
            set { SetValue(MeterColorProperty, value); }
        }

        public static readonly DependencyProperty TrackColorProperty = DependencyProperty.Register("TrackColor", typeof(Color), typeof(MeterControl), new PropertyMetadata(Colors.Black));
        public Color TrackColor
        {
            get { return (Color)GetValue(TrackColorProperty); }
            set { SetValue(TrackColorProperty, value); }
        }

        private static void HandleRangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MeterControl mc = d as MeterControl;
            if (mc != null)
                mc.UpdateMeterPosition();
        }

        #endregion dependency properties

        static MeterControl()
        {
            ResourceDictionary dictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/Synth.WPF;component/Controls/ControlResources.xaml") };
            ControlTemplate defaultTemplate = (ControlTemplate)dictionary["MeterControlDefaultTemplate"];
            TemplateProperty.OverrideMetadata(typeof(MeterControl), new FrameworkPropertyMetadata(defaultTemplate));
        }

        private void UpdateMeterPosition()
        {
            // TODO handle if min >= max

            double portionOfRange = (Value - Minimum) / (Maximum - Minimum);
            IsOverflowed = (portionOfRange > 1);
            MeterPosition = Math.Max(Math.Min(portionOfRange, Maximum), Minimum);
        }
    }
}
