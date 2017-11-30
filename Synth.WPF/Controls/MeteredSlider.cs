using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

using Synth.WPF.Util;
using Synth.Util;

namespace Synth.WPF.Controls
{
    public class SliderTick
    {
        public string Label { get; set; }
        public double Value { get; set; }
    }

    [ContentProperty("Ticks")]
    public class MeteredSlider : Control
    {
        public enum IndentRule
        {
            Always,
            Never,
            ShiftKey
        }

        #region dependency properties

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(MeteredSlider), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HandleValueChanged));

        private static void HandleValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MeteredSlider ss = d as MeteredSlider;
            //if (ss != null)
            //{
            //    if (ss.Minimum == -1)
            //        Console.WriteLine("Hello");
            //}

            if (ss != null && !ss._isUpdating)
            {
                ss._isUpdating = true;

                if (ss.Indenting == IndentRule.Always || (ss.Indenting == IndentRule.ShiftKey && (System.Windows.Input.Keyboard.Modifiers == ModifierKeys.Shift || System.Windows.Input.Keyboard.IsKeyToggled(Key.CapsLock))))
                {
                    if (ss.IsEnabled && ss.IsVisible && ss.ActualHeight > 0 && Mouse.LeftButton == MouseButtonState.Pressed && ss.IsMouseOver)
                        ss.SnapIfWithinIndentRange();
                }

                ss.SliderPosition = ss._rangeConverter.SourceToTarget(ss.Value);
                //if (ss.Minimum == -1)
                //    Console.WriteLine(ss.Value.ToString("0.0000") + " --> " + ss.SliderPosition.ToString("0.0000"));

                //if (ss.Name == "Test")
                //    Console.WriteLine(ss.SliderPosition);

                ss._isUpdating = false;

                ss.OnValueChanged();
            }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty SliderPositionProperty = DependencyProperty.Register("SliderPosition", typeof(double), typeof(MeteredSlider), new PropertyMetadata(0d, HandleSliderPositionChanged));

        private static void HandleSliderPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MeteredSlider ss = d as MeteredSlider;
            if (ss != null && !ss._isUpdating)
                ss.Value = ss._rangeConverter.TargetToSource(ss.SliderPosition);
        }

        private bool _isUpdating = false;

        public double SliderPosition
        {
            get { return (double)GetValue(SliderPositionProperty); }
            set { SetValue(SliderPositionProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(MeteredSlider), new PropertyMetadata(0d, HandleRangePropertyChanged));

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(MeteredSlider), new PropertyMetadata(1d, HandleRangePropertyChanged));
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty ScaleFactorProperty = DependencyProperty.Register("ScaleFactor", typeof(double), typeof(MeteredSlider), new PropertyMetadata(1d, HandleRangePropertyChanged));
        public double ScaleFactor
        {
            get { return (double)GetValue(ScaleFactorProperty); }
            set { SetValue(ScaleFactorProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(MeteredSlider), new PropertyMetadata(Orientation.Vertical));
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty ThumbTemplateProperty = DependencyProperty.Register("ThumbTemplate", typeof(ControlTemplate), typeof(MeteredSlider), new PropertyMetadata(GetResource<ControlTemplate>("MeteredSliderDefaultThumbTemplate")));
        public ControlTemplate ThumbTemplate
        {
            get { return (ControlTemplate)GetValue(ThumbTemplateProperty); }
            set { SetValue(ThumbTemplateProperty, value); }
        }

        public static readonly DependencyProperty TrackTemplateProperty = DependencyProperty.Register("TrackTemplate", typeof(ControlTemplate), typeof(MeteredSlider), new PropertyMetadata(GetResource<ControlTemplate>("MeteredSliderDefaultTrackTemplate")));
        public ControlTemplate TrackTemplate
        {
            get { return (ControlTemplate)GetValue(TrackTemplateProperty); }
            set { SetValue(TrackTemplateProperty, value); }
        }

        public static readonly DependencyProperty TickTemplateProperty = DependencyProperty.Register("TickTemplate", typeof(DataTemplate), typeof(MeteredSlider), new PropertyMetadata(null));
        public DataTemplate TickTemplate
        {
            get { return (DataTemplate)GetValue(TickTemplateProperty); }
            set { SetValue(TickTemplateProperty, value); }
        }


        public static readonly DependencyProperty IsMeterWhenDisabledProperty = DependencyProperty.Register("IsMeterWhenDisabled", typeof(bool), typeof(MeteredSlider), new PropertyMetadata(false));
        public bool IsMeterWhenDisabled
        {
            get { return (bool)GetValue(IsMeterWhenDisabledProperty); }
            set { SetValue(IsMeterWhenDisabledProperty, value); }
        }

        public static readonly DependencyProperty IndentingProperty = DependencyProperty.Register("Indenting", typeof(IndentRule), typeof(MeteredSlider), new PropertyMetadata(IndentRule.ShiftKey));
        public IndentRule Indenting
        {
            get { return (IndentRule)GetValue(IndentingProperty); }
            set { SetValue(IndentingProperty, value); }
        }

        public static readonly DependencyProperty TickBrushProperty = DependencyProperty.Register("TickBrush", typeof(Brush), typeof(MeteredSlider), new PropertyMetadata(Brushes.Gray));
        public Brush TickBrush
        {
            get { return (Brush)GetValue(TickBrushProperty); }
            set { SetValue(TickBrushProperty, value); }
        }

        public static readonly DependencyProperty IndentedTickBrushProperty = DependencyProperty.Register("IndentedTickBrush", typeof(Brush), typeof(MeteredSlider), new PropertyMetadata(Brushes.White));
        public Brush IndentedTickBrush
        {
            get { return (Brush)GetValue(IndentedTickBrushProperty); }
            set { SetValue(IndentedTickBrushProperty, value); }
        }

        private static readonly DependencyPropertyKey ActiveTickBrushPropertyKey = DependencyProperty.RegisterReadOnly("ActiveTickBrush", typeof(Brush), typeof(MeteredSlider), new PropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty ActiveTickBrushProperty = ActiveTickBrushPropertyKey.DependencyProperty;
        public Brush ActiveTickBrush
        {
            get { return (Brush)GetValue(ActiveTickBrushProperty); }
            private set { SetValue(ActiveTickBrushPropertyKey, value); }
        }

        private static void HandleRangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MeteredSlider ss = d as MeteredSlider;
            if (ss != null)
                ss.UpdateRangeProperties();
        }

        public static readonly DependencyProperty TicksSourceProperty = DependencyProperty.Register("TicksSource", typeof(SliderTick[]), typeof(MeteredSlider),
                                                                                                    new FrameworkPropertyMetadata((SliderTick[])null, 
                                                                                                    new PropertyChangedCallback(OnTicksSourceChanged)));

        public SliderTick[] TicksSource
        {
            get { return GetValue(TicksSourceProperty) as SliderTick[]; }
            set { SetValue(TicksSourceProperty, value); }
        }

        private static void OnTicksSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MeteredSlider ss = d as MeteredSlider;
            if (ss == null)
                return;

            BindingExpressionBase beb = BindingOperations.GetBindingExpressionBase(d, TicksSourceProperty);
            if (beb != null)
                throw new InvalidOperationException("The TicksSource property doesn't support data binding. Use StaticResource or DynamicResource or x:Static instead.");


            ss._ticks.Clear();

            SliderTick[] newTicks = e.NewValue as SliderTick[];
            
            if (newTicks != null)
                newTicks.Execute(ss._ticks.Add);
        }

        private static readonly DependencyPropertyKey AreTicksIndentedPropertyKey =
            DependencyProperty.RegisterReadOnly("AreTicksIndented", typeof (bool), typeof (MeteredSlider),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty AreTicksIndentedProperty =
            AreTicksIndentedPropertyKey.DependencyProperty;

        public bool AreTicksIndented
        {
            get { return (bool) GetValue(AreTicksIndentedProperty); }
            private set { SetValue(AreTicksIndentedPropertyKey, value);}
        }

        #endregion dependency properties

        static MeteredSlider()
        {
            ControlTemplate defaultTemplate = GetResource<ControlTemplate>("MeteredSliderDefaultTemplate");
            TemplateProperty.OverrideMetadata(typeof(MeteredSlider), new FrameworkPropertyMetadata(defaultTemplate));
        }

        private static T GetResource<T>(string key)
            where T : class
        {
            ResourceDictionary dictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/Synth.WPF;component/Controls/ControlResources.xaml") };
            return dictionary[key] as T;
        }

        public event EventHandler ValueChanged;

        private const double MIN_INDENT_RANGE_PIXELS = 4;
        private const double IDEAL_INDENT_RANGE_PERCENT = 0.05;

        private readonly RangeConverter _rangeConverter = new RangeConverter();

        public MeteredSlider()
        {
            //UpdateRangeProperties();
            Loaded += HandleLoaded;
        }

        #region event support for AreTicksIndented property

        private void HandleLoaded(object sender, RoutedEventArgs args)
        {
            Window parentWindow = this.GetRootParent() as Window;
            if (parentWindow != null)
            {
                parentWindow.Activated -= HandleCheckIndentation;
                parentWindow.PreviewKeyDown -= HandleCheckIndentation;
                parentWindow.PreviewKeyUp -= HandleCheckIndentation;

                parentWindow.Activated += HandleCheckIndentation;
                parentWindow.PreviewKeyDown += HandleCheckIndentation;
                parentWindow.PreviewKeyUp += HandleCheckIndentation;
            }

            IsEnabledChanged += (s,e) => HandleCheckIndentation(s, null);

            HandleCheckIndentation(sender, null);
        }

        private void HandleCheckIndentation(object sender, EventArgs e)
        {
            if (!IsEnabled)
                AreTicksIndented = false;
            else if (Indenting == IndentRule.Never)
                AreTicksIndented = false;
            else if (Indenting == IndentRule.Always)
                AreTicksIndented = true;
            else if (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftShift))
                AreTicksIndented = true;
            else if (System.Windows.Input.Keyboard.IsKeyDown(Key.RightShift))
                AreTicksIndented = true;
            else if (System.Windows.Input.Keyboard.IsKeyToggled(Key.CapsLock))
                AreTicksIndented = true;
            else
                AreTicksIndented = false;

            ActiveTickBrush = AreTicksIndented ? IndentedTickBrush : TickBrush;
        }

        #endregion event support for AreTicksIndented property

        private void OnValueChanged()
        {
            EventHandler evt = ValueChanged;
            if (evt != null)
                evt.Invoke(this, new EventArgs());
        }

        private void UpdateRangeProperties()
        {
            _rangeConverter.SourceMin = Minimum;
            _rangeConverter.SourceMax = Maximum;
            _rangeConverter.ScaleFactor = ScaleFactor;

            Value = _rangeConverter.ForceIntoSourceRange(Value);
            SliderPosition = _rangeConverter.SourceToTarget(Value);
            //if (Minimum == -1)
            //{
            //    Console.WriteLine(_rangeConverter.SourceToTarget(-1));
            //    Console.WriteLine(_rangeConverter.SourceToTarget(-0.5));
            //    Console.WriteLine(_rangeConverter.SourceToTarget(0));
            //    Console.WriteLine(_rangeConverter.SourceToTarget(0.5));
            //    Console.WriteLine(_rangeConverter.SourceToTarget(1));
            //}
        }

        private void SnapIfWithinIndentRange()
        {
            // TODO: calc the minIndentRangePercent using the scalefactor. This code works only when the ScaleFactor is one.

            double minIndentRangePercent = MIN_INDENT_RANGE_PIXELS / ActualHeight;
            double indentRangePercent = Math.Max(minIndentRangePercent, IDEAL_INDENT_RANGE_PERCENT);
            double indentRange = indentRangePercent * (Maximum - Minimum);

            double value = Value;

            SliderTick nearestTick = Ticks.Where(t => Math.Abs(t.Value - value) < indentRange).OrderBy(t => Math.Abs(t.Value - value)).FirstOrDefault();
            if (nearestTick != null && nearestTick.Value != value)
                Value = nearestTick.Value;
        }

        private readonly ObservableCollection<SliderTick> _ticks = new ObservableCollection<SliderTick>();
        public ObservableCollection<SliderTick> Ticks
        {
            get { return _ticks; }
        }
    }
}
