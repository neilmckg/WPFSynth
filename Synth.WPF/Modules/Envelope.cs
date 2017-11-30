using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

using Synth.Audio;
using Synth.Module;
using Synth.Util;

namespace Synth.WPF.Modules
{
    public sealed class Envelope : Module, INotifyPropertyChanged
    {
        #region I/O dependency properties

        #region Phase

        private static readonly DependencyPropertyKey PhasePropertyKey = DependencyProperty.RegisterReadOnly("Phase",
                                                                                                              typeof(EnvelopePhase),
                                                                                                              typeof(Envelope),
                                                                                                              new PropertyMetadata(EnvelopePhase.Inactive, BaseControlInputValueChanged));
        public static readonly DependencyProperty PhaseProperty = PhasePropertyKey.DependencyProperty;

        [Bindable(true)]
        public EnvelopePhase Phase
        {
            get { return (EnvelopePhase)GetValue(PhaseProperty); }
            private set { SetValue(PhasePropertyKey, value); }
        }

        #endregion Phase

        #region Value

        private static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly("Value",
                                                                                                              typeof(double),
                                                                                                              typeof(Envelope),
                                                                                                              new PropertyMetadata(0d));
        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;

        [Bindable(true)]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            private set { SetValue(ValuePropertyKey, value); }
        }

        #endregion Value

        #region TimeScale

        public static readonly DependencyProperty TimeScaleProperty = DependencyProperty.Register("TimeScale",
                                                                         typeof(double),
                                                                         typeof(Envelope),
                                                                         new PropertyMetadata(1d, BaseControlInputValueChanged));

        [Bindable(true)]
        public double TimeScale
        {
            get { return (double)GetValue(TimeScaleProperty); }
            set { SetValue(TimeScaleProperty, value); }
        }

        #endregion TimeScale

        #region LevelScale

        public static readonly DependencyProperty LevelScaleProperty = DependencyProperty.Register("LevelScale",
                                                                         typeof(double),
                                                                         typeof(Envelope),
                                                                         new PropertyMetadata(1d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double LevelScale
        {
            get { return (double)GetValue(LevelScaleProperty); }
            set { SetValue(LevelScaleProperty, value); }
        }

        #endregion LevelScale

        #region Trigger

        public static readonly DependencyProperty TriggerProperty = DependencyProperty.Register("Trigger",
                                                                         typeof(bool),
                                                                         typeof(Envelope),
                                                                         new PropertyMetadata(false, BaseControlInputValueChanged));
        [Bindable(true)]
        public bool Trigger
        {
            get { return (bool)GetValue(TriggerProperty); }
            set { SetValue(TriggerProperty, value); }
        }

        #endregion Trigger

        #endregion I/O dependency properties

        #region StepCollections

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EnvelopeCore.StepCollection Attack
        {
            get { return _core.Attack; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EnvelopeCore.StepCollection Loop
        {
            get { return _core.Loop; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EnvelopeCore.StepCollection Release
        {
            get { return _core.Release; }
        }

        #endregion StepCollections
                                
        static Envelope()
        {
            InitializeClassMetadata<Envelope>(WPF.Resources.EnvelopeTemplate);
        }

        private readonly EnvelopeCore _core;

        public event PropertyChangedEventHandler PropertyChanged;

        public Envelope()
            : this(AudioLink.Instance)
        {
        }

        public Envelope(IAudioLink audioLink)
            : base(audioLink)
        {
            _core = new EnvelopeCore(audioLink);

            RegisterChildCollection(_core.Attack);
            RegisterChildCollection(_core.Loop);
            RegisterChildCollection(_core.Release);

            _core.StepsChanged += HandleCoreStepsChanged;
            _core.PhaseChanged += HandleCorePhaseChanged;
            _core.OutputChanged += HandleCoreOutputChanged;

            InputValueChanged(TimeScaleProperty.Name);

            Unloaded += (s, e) => _core.Dispose();
        }

        private void HandleCoreStepsChanged(object sender, EventArgs<EnvelopePhase> e)
        {
            if (e.Value == EnvelopePhase.Attack)
                OnPropertyChanged("Attack");
            else if (e.Value == EnvelopePhase.Loop)
                OnPropertyChanged("Loop");
            else if (e.Value == EnvelopePhase.Release)
                OnPropertyChanged("Release");
        }

        private void HandleCoreOutputChanged(object sender, EventArgs<float> e)
        {
            // use this code to test whether the audio sample pulls are still being done on a non-UI thread
            //bool synthIsRunningOnUiThread = Dispatcher == System.Windows.Threading.Dispatcher.CurrentDispatcher;

            Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => Value = _core.Output));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler evt = PropertyChanged;
            if (evt != null)
                evt.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleCorePhaseChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => Phase = _core.Phase));
        }

        protected override void InputValueChanged(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName) || inputName == LevelScaleProperty.Name)
                _core.LevelScale.Value = (float)LevelScale;
            if (string.IsNullOrWhiteSpace(inputName) || inputName == TimeScaleProperty.Name)
                _core.TimeScale.Value = (float) TimeScale;
            if (string.IsNullOrWhiteSpace(inputName) || inputName == TriggerProperty.Name)
                _core.IsTriggered.Value = Trigger;
        }
    }
}
