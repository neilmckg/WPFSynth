using System.ComponentModel;
using System.Windows;

using Synth.Audio;
using Synth.Module;

namespace Synth.WPF.Modules
{
    public sealed class Noise : Module, IAudioSource
    {
        #region I/O dependency properties

        #region Level

        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level",
                                                                         typeof(double),
                                                                         typeof(Noise),
                                                                         new PropertyMetadata(1d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Level
        {
            get { return (double)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        #endregion Level

        #region Output

        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly("Output",
                                                                                                              typeof(AudioWire),
                                                                                                              typeof(Noise),
                                                                                                              new PropertyMetadata());
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;

        public AudioWire Output
        {
            get { return (AudioWire)GetValue(OutputProperty); }
            private set { SetValue(OutputPropertyKey, value); }
        }

        #endregion Output

        #endregion I/O dependency properties
                        
        static Noise()
        {
            InitializeClassMetadata<Noise>(WPF.Resources.NoiseTemplate);
        }

        private readonly NoiseCore _core;

        public Noise()
            : this(AudioLink.Instance)
        {
        }

        public Noise(IAudioLink audioLink)
            : base(audioLink)
        {
            _core = new NoiseCore();
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
                _core.Level.Value = (float) Level;
        }
    }
}
