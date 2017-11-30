using System.ComponentModel;
using System.Windows;

using Synth.Audio;
using Synth.Module;
using Synth.Util;

namespace Synth.WPF.Modules
{
    public sealed class Translate : Module
    {
        #region I/O dependency properties

        #region SourceValue (input)

        public static readonly DependencyProperty SourceValueProperty = DependencyProperty.Register("SourceValue",
                                                                                           typeof(double),
                                                                                           typeof(Translate),
                                                                                           new FrameworkPropertyMetadata(0d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double SourceValue
        {
            get { return (double)GetValue(SourceValueProperty); }
            set { SetValue(SourceValueProperty, value); }
        }

        #endregion SourceValue (input)

        #region Scale

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", 
                                                                                              typeof(double), 
                                                                                              typeof(Translate),
                                                                                              new PropertyMetadata(1d, BaseControlInputValueChanged));
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        #endregion Scale

        #region Center

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", 
                                                                                               typeof(double), 
                                                                                               typeof(Translate),
                                                                                               new PropertyMetadata(0d, BaseControlInputValueChanged));
        public double Center
        {
            get { return (double)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        #endregion Center

        #region Curvature

        public static readonly DependencyProperty CurvatureProperty = DependencyProperty.Register("Curvature",
                                                                                         typeof(double),
                                                                                         typeof(Translate),
                                                                                         new FrameworkPropertyMetadata(1d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Curvature
        {
            get { return (double)GetValue(CurvatureProperty); }
            set { SetValue(CurvatureProperty, value); }
        }

        #endregion Curvature

        #region Value (output)

        private static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly("Value", 
                                                                                                   typeof(double),
                                                                                                   typeof(Translate), 
                                                                                                   new PropertyMetadata(0d));
        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;

        [Bindable(true)]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            private set { SetValue(ValuePropertyKey, value); }
        }

        #endregion Value (output)

        #endregion I/O dependency properties
        
        static Translate()
        {
            InitializeClassMetadata<Translate>(WPF.Resources.TranslateTemplate);
        }

        private readonly TranslateCore _core;

        public Translate()
            : this(AudioLink.Instance)
        {
        }

        public Translate(IAudioLink audioLink)
            : base(audioLink)
        {
            _core = new TranslateCore();
            _core.OutputChanged += HandleCoreOutputChanged;
            InputValueChanged(null);
        }

        private void HandleCoreOutputChanged(object sender, EventArgs<float> e)
        {
            Value = _core.Output;
        }

        protected override void InputValueChanged(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName) || inputName == SourceValueProperty.Name)
                _core.SourceValue.Value = (float)SourceValue;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == ScaleProperty.Name)
                _core.Scale.Value = (float)Scale;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == CenterProperty.Name)
                _core.Center.Value = (float)Center;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == CurvatureProperty.Name)
                _core.Curvature.Value = (float)Curvature;
        }
    }
}
