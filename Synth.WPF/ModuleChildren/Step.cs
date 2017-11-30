using System;
using System.ComponentModel;
using System.Windows;

using Synth.Module;

namespace Synth.WPF.ModuleChildren
{
    // child component of Envelope module
    public sealed class Step : FrameworkContentElement, IEnvelopeStep
    {
        #region I/O dependency properties

        #region Seconds

        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds",
                                                                         typeof(float),
                                                                         typeof(Step),
                                                                         new PropertyMetadata(1f, HandleSecondsChanged));

        private static void HandleSecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Step step = d as Step;
            // storing the value in a class variable allows it to be accessed from any thread.
            step._seconds = (float)e.NewValue;
            step.OnChanged();
        }

        [Bindable(true)]
        public float Seconds
        {
            get { return _seconds; }
            set { SetValue(SecondsProperty, value); }
        }

        #endregion Seconds

        #region TargetValue

        public static readonly DependencyProperty TargetValueProperty = DependencyProperty.Register("TargetValue",
                                                                         typeof(float),
                                                                         typeof(Step),
                                                                         new PropertyMetadata(1f, HandleTargetValueChanged));

        private static void HandleTargetValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Step step = d as Step;
            // storing the value in a class variable allows it to be accessed from any thread.
            step._targetValue = (float)e.NewValue;
            step.OnChanged();
        }

        [Bindable(true)]
        public float TargetValue
        {
            get { return _targetValue; }
            set { SetValue(TargetValueProperty, value); }
        }

        #endregion TargetValue

        #endregion I/O dependency properties

        public event EventHandler Changed;

        private float _targetValue = (float)TargetValueProperty.DefaultMetadata.DefaultValue;
        private float _seconds = (float)SecondsProperty.DefaultMetadata.DefaultValue;

        public Step()
        {
        }

        public Step(float seconds, float targetValue) 
            : this()
        {
            Seconds = seconds;
            TargetValue = targetValue;
        }

        private void OnChanged()
        {
            EventHandler evt = Changed;
            if (evt != null)
                evt.Invoke(this, new EventArgs());
        }
    }
}
