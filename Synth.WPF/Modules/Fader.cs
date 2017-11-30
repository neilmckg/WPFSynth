using System.ComponentModel;
using System.Windows;

using Synth.Module;
using Synth.Audio;

namespace Synth.WPF.Modules
{
    public sealed class Fader : Module, IAudioSource
    {
        #region I/O dependency properties

        #region Level

        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level",
                                                                                     typeof(double),
                                                                                     typeof(Fader),
                                                                                     new FrameworkPropertyMetadata(1d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Level
        {
            get { return (double)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        #endregion Level

        #region Input

        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input",
                                                                                    typeof(AudioWire),
                                                                                    typeof(Fader),
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
                                                                                                              typeof(Fader),
                                                                                                              new PropertyMetadata());
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;

        public AudioWire Output
        {
            get { return (AudioWire)GetValue(OutputProperty); }
            private set { SetValue(OutputPropertyKey, value); }
        }

        #endregion Output

        #endregion I/O dependency properties
        
        static Fader()
        {
            InitializeClassMetadata<Fader>(WPF.Resources.FaderTemplate);
        }

        private readonly FaderCore _core;

        public Fader()
            : this(AudioLink.Instance)
        {
        }

        public Fader(IAudioLink audioLink)
            : base(audioLink)
        {
            _core = new FaderCore();
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

            if (string.IsNullOrWhiteSpace(inputName) || inputName == InputProperty.Name)
                _core.SetInput(this.Input);
        }
    }
}
