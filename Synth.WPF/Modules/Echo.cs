using System;
using System.ComponentModel;
using System.Windows;

using Synth.Audio;
using Synth.Module;

namespace Synth.WPF.Modules
{
    public sealed class Echo : Module, IAudioSource
    {
        #region I/O dependency properties

        #region Level

        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level",
                                                                         typeof(double),
                                                                         typeof(Echo),
                                                                         new PropertyMetadata(1d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Level
        {
            get { return (double)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        #endregion Level

        #region Seconds

        public static readonly DependencyProperty SecondsProperty = DependencyProperty.Register("Seconds",
                                                                         typeof(double),
                                                                         typeof(Echo),
                                                                         new PropertyMetadata(0.4d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Seconds
        {
            get { return (double)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }

        #endregion Seconds

        #region Feedback

        public static readonly DependencyProperty FeedbackProperty = DependencyProperty.Register("Feedback",
                                                                         typeof(double),
                                                                         typeof(Echo),
                                                                         new PropertyMetadata(0d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Feedback
        {
            get { return (double)GetValue(FeedbackProperty); }
            set { SetValue(FeedbackProperty, value); }
        }

        #endregion Feedback

        #region Input

        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input",
                                                                                    typeof(AudioWire),
                                                                                    typeof(Echo),
                                                                                    new PropertyMetadata(BaseControlInputValueChanged));
        public AudioWire Input
        {
            get { return (AudioWire)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        #endregion Input

        #region Output

        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly("Output",
                                                                                                              typeof(AudioWire),
                                                                                                              typeof(Echo),
                                                                                                              new PropertyMetadata());
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;

        public AudioWire Output
        {
            get { return (AudioWire)GetValue(OutputProperty); }
            private set { SetValue(OutputPropertyKey, value); }
        }

        #endregion Output

        #endregion I/O dependency properties

        static Echo()
        {
            InitializeClassMetadata<Echo>(WPF.Resources.EchoTemplate);
        }

        private readonly EchoCore _core;

        public Echo()
            : this(AudioLink.Instance)
        {
        }

        public Echo(IAudioLink audioLink)
            : base(audioLink)
        {
            _core = new EchoCore(audioLink);
            InputValueChanged(null);
            Output = GetSample;
        }

        public AudioSample GetSample(ulong requestId)
        {
            return _core.GetSample(requestId);
        }

        protected override void InputValueChanged(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName) || inputName == LevelProperty.Name)
                _core.Level.Value = (float)Level;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == SecondsProperty.Name)
                _core.Seconds.Value = (float)Seconds;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == FeedbackProperty.Name)
                _core.Feedback.Value = (float)Feedback;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == InputProperty.Name)
                _core.SetInput(Input);
        }
    }
}
