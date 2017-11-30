using System.ComponentModel;
using System.Windows;

using Synth.Audio;
using Synth.Core;
using Synth.Module;

namespace Synth.WPF.Modules
{
    public sealed class Oscillator : Module, IAudioSource
    {
        #region I/O dependency properties

        #region Pitch

        public static readonly DependencyProperty PitchProperty = DependencyProperty.Register("Pitch",
                                                                                     typeof(double),
                                                                                     typeof(Oscillator),
                                                                                     new FrameworkPropertyMetadata((double)Scale.EqualTemperedScale["A4"].Pitch, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Pitch
        {
            get { return (double)GetValue(PitchProperty); }
            set { SetValue(PitchProperty, value); }
        }

        #endregion Pitch

        #region PitchOffsetHalfSteps

        public static readonly DependencyProperty PitchOffsetHalfStepsProperty = DependencyProperty.Register("PitchOffsetHalfSteps",
                                                                                          typeof(double),
                                                                                          typeof(Oscillator), 
                                                                                          new PropertyMetadata(0d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double PitchOffsetHalfSteps
        {
            get { return (double)GetValue(PitchOffsetHalfStepsProperty); }
            set { SetValue(PitchOffsetHalfStepsProperty, value); }
        }

        #endregion PitchOffsetHalfSteps

        #region Wave

        public static readonly DependencyProperty WaveProperty = DependencyProperty.Register("Wave",
                                                                                    typeof(Synth.Core.Wave),
                                                                                    typeof(Oscillator),
                                                                                    new FrameworkPropertyMetadata(Synth.Core.Wave.Triangle, BaseControlInputValueChanged));
        [Bindable(true)]
        public Synth.Core.Wave Wave
        {
            get { return (Synth.Core.Wave)GetValue(WaveProperty); }
            set { SetValue(WaveProperty, value); }
        }

        #endregion Wave

        #region Level

        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level",
                                                                         typeof(double),
                                                                         typeof(Oscillator),
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
                                                                                                              typeof(Oscillator),
                                                                                                              new PropertyMetadata());
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;

        public AudioWire Output
        {
            get { return (AudioWire)GetValue(OutputProperty); }
            private set { SetValue(OutputPropertyKey, value); }
        }

        #endregion Output

        #endregion I/O dependency properties
                        
        static Oscillator()
        {
            InitializeClassMetadata<Oscillator>(WPF.Resources.OscillatorTemplate);
        }

        private readonly OscillatorCore _core;

        public Oscillator()
            : this(AudioLink.Instance)
        {
        }

        public Oscillator(IAudioLink audioLink)
            : base(audioLink)
        {
            _core = new OscillatorCore(audioLink);
            InputValueChanged(null);
            Output = GetSample;
        }

        public AudioSample GetSample(ulong requestId)
        {
            // use this code to test whether the audio sample pulls are still being done on a non-UI thread
            //bool synthIsRunningOnUiThread = Dispatcher == System.Windows.Threading.Dispatcher.CurrentDispatcher;

            return _core.GetSample(requestId);
        }

        protected override void InputValueChanged(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName) || inputName == PitchProperty.Name || inputName == PitchOffsetHalfStepsProperty.Name)
                _core.Pitch.Value = (float)CalculateCompositePitch();
            
            if (string.IsNullOrWhiteSpace(inputName) || inputName == LevelProperty.Name)
                _core.Level.Value = (float)Level;
            
            if (string.IsNullOrWhiteSpace(inputName) || inputName == WaveProperty.Name)
                _core.Wave.Value = Wave;
        }

        private double CalculateCompositePitch()
        {
            return this.Pitch + (this.PitchOffsetHalfSteps * Synth.Core.Pitch.HalfStep);
        }
    }
}
