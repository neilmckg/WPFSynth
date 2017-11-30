using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Synth.Audio;
using Synth.Util;
using Synth.WPF.Util;

namespace Synth.WPF.Instrument
{
    public class InstrumentBase : ItemsControl, IAudioSource
    {
        #region dependency properties

        #region VelocityLabel

        public static readonly DependencyProperty VelocityLabelProperty = DependencyProperty.Register("VelocityLabel",
                                                                                     typeof(string),
                                                                                     typeof(InstrumentBase),
                                                                                     new PropertyMetadata("Velocity"));
        [Bindable(true)]
        public string VelocityLabel
        {
            get { return (string)GetValue(VelocityLabelProperty); }
            set { SetValue(VelocityLabelProperty, value); }
        }

        #endregion VelocityLabel

        #region SustainLabel

        public static readonly DependencyProperty SustainLabelProperty = DependencyProperty.Register("SustainLabel",
                                                                                     typeof(string),
                                                                                     typeof(InstrumentBase),
                                                                                     new PropertyMetadata("Sustain"));
        [Bindable(true)]
        public string SustainLabel
        {
            get { return (string)GetValue(SustainLabelProperty); }
            set { SetValue(SustainLabelProperty, value); }
        }

        #endregion SustainLabel

        #region IsActive

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive",
                                                                                     typeof(bool),
                                                                                     typeof(InstrumentBase),
                                                                                     new PropertyMetadata(true, HandleIsActiveChanged));

        private static void HandleIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as InstrumentBase)._masterLevel.Target = (bool) e.NewValue ? 1 : 0;
        }

        [Bindable(true)]
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        #endregion IsActive

        #region Level

        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level",
                                                                                     typeof(double),
                                                                                     typeof(InstrumentBase),
                                                                                     new PropertyMetadata(1d, HandleLevelChanged));

        private static void HandleLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as InstrumentBase)._level = (double)e.NewValue;
        }

        [Bindable(true)]
        public double Level
        {
            get { return _level; }
            set { SetValue(LevelProperty, value); }
        }

        #endregion Level

        #region OutputSourceName

        public static readonly DependencyProperty OutputSourceNameProperty = DependencyProperty.Register("OutputSourceName",
                                                                                         typeof(string),
                                                                                         typeof(InstrumentBase),
                                                                                         new PropertyMetadata("", HandleOutputSourceChanged));
        private static void HandleOutputSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as InstrumentBase).SelectOutputModule();
        }

        [Bindable(true)]
        public string OutputSourceName
        {
            get { return (string)GetValue(OutputSourceNameProperty); }
            set { SetValue(OutputSourceNameProperty, value); }
        }

        #endregion OutputSourceName

        #region InputTemplate

        public static readonly DependencyProperty InputTemplateProperty = DependencyProperty.Register("InputTemplate",
                                                                                         typeof(ControlTemplate),
                                                                                         typeof(InstrumentBase),
                                                                                         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public ControlTemplate InputTemplate
        {
            get { return (ControlTemplate)GetValue(InputTemplateProperty); }
            set { SetValue(InputTemplateProperty, value); }
        }

        #endregion InputTemplate

        #region OutputTemplate

        public static readonly DependencyProperty OutputTemplateProperty = DependencyProperty.Register("OutputTemplate",
                                                                                         typeof(ControlTemplate),
                                                                                         typeof(InstrumentBase),
                                                                                         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public ControlTemplate OutputTemplate
        {
            get { return (ControlTemplate)GetValue(OutputTemplateProperty); }
            set { SetValue(OutputTemplateProperty, value); }
        }

        #endregion OutputTemplate

        #endregion dependency properties

        #region static members

        static InstrumentBase()
        {
            InitializeClassMetaData<InstrumentBase>();

            TemplateProperty.SetDefaultValue<InstrumentBase>(WPF.Resources.InstrumentTemplate);
            ItemsPanelProperty.SetDefaultValue<InstrumentBase>(WPF.Resources.WrapPanelItemsTemplate);
        }

        protected static void InitializeClassMetaData<T>(string inputTemplateKey, string outputTemplateKey)
            where T : InstrumentBase
        {
            ResourceDictionary dictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/Synth.WPF;component/ModuleTemplates.xaml") };
            InitializeClassMetaData<T>(dictionary[inputTemplateKey] as ControlTemplate, dictionary[outputTemplateKey] as ControlTemplate);
        }

        protected static void InitializeClassMetaData<T>(ControlTemplate inputTemplate, ControlTemplate outputTemplate)
             where T : InstrumentBase
        {
            InitializeClassMetaData<T>();

            InputTemplateProperty.SetDefaultValue<T>(inputTemplate);
            OutputTemplateProperty.SetDefaultValue<T>(outputTemplate);
        }

        protected static void InitializeClassMetaData<T>()
             where T : InstrumentBase
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(typeof(T)));
        }

        protected static void BaseControlInputValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is InstrumentBase)
                (d as InstrumentBase).InputValueChanged(e.Property.Name);
        }

        #endregion static members

        private double _level = (double)LevelProperty.DefaultMetadata.DefaultValue;
        private AudioWire _audioSource;
        private readonly ThrottledFloat _masterLevel = new ThrottledFloat(0.001f, 1f);

        public InstrumentBase()
        {
            // Constructor and class must be public to work with the WPF designer, but we don't want direct instantiation
            if (!this.IsInDesignMode() && GetType() == typeof(InstrumentBase))
                throw new InvalidOperationException(GetType().Name + " cannot be directly instanced at runtime. Derive a class from it, and create an instance of that.");

            LoadDefaultResources();
            DataContext = this;

            Loaded += HandleLoaded;
            Unloaded += HandleUnloaded;
            (Items as INotifyCollectionChanged).CollectionChanged += (s, e) => SelectOutputModule();
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            AudioLink.Instance.DetachSource(this);
        }

        protected virtual void InputValueChanged(string inputName)
        {
            //if (inputName == MidiChannelProperty.Name)
            //    Reset();
        }

        private void SelectOutputModule()
        {
            IAudioSource outputSource;

            if (string.IsNullOrEmpty(OutputSourceName))
                outputSource = Items.OfType<IAudioSource>().LastOrDefault();
            else
                outputSource = this.FindName(OutputSourceName) as IAudioSource;

            if (outputSource == null)
                _audioSource = null;
            else
                _audioSource = outputSource.GetSample;
        }

        private void LoadDefaultResources()
        {
            //ResourceDictionary dictionary = new ResourceDictionary
            //{
            //    Source = new Uri("pack://application:,,,/Synth.WPF;component/ModuleTemplates.xaml")
            //};
            //Resources.MergedDictionaries.Add(dictionary);
        }

        AudioSample IAudioSource.GetSample(ulong requestId)
        {
            AudioSample newSample = new AudioSample();

            if (_audioSource != null)
                newSample = _audioSource(requestId);

            newSample *= (float)Level * _masterLevel.GetNextSample();

            return newSample;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            SelectOutputModule();

            if (_audioSource == null)
                AudioLink.Instance.DetachSource(this);
            else
                AudioLink.Instance.AttachSource(this);
        }
    }
}
