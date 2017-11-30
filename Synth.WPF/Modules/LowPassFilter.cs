using System.ComponentModel;
using System.Windows;

using Synth.Audio;
using Synth.Module;

namespace Synth.WPF.Modules
{
    public sealed class LowPassFilter : Module, IAudioSource
    {
        private const float DEFAULT_CUTOFFPITCH = 0.5f;
        private const float DEFAULT_RESO = 0.2f;

        #region I/O dependency properties

        #region CutoffPitch

        public static readonly DependencyProperty CutoffPitchProperty = DependencyProperty.Register("CutoffPitch",
                                                                                     typeof(double),
                                                                                     typeof(LowPassFilter),
                                                                                     new FrameworkPropertyMetadata((double)DEFAULT_CUTOFFPITCH, BaseControlInputValueChanged));

        [Bindable(true)]
        public double CutoffPitch
        {
            get { return (double)GetValue(CutoffPitchProperty); }
            set { SetValue(CutoffPitchProperty, value); }
        }

        #endregion CutoffPitch

        #region Resonance

        public static readonly DependencyProperty ResonanceProperty = DependencyProperty.Register("Resonance",
                                                                                     typeof(double),
                                                                                     typeof(LowPassFilter),
                                                                                     new FrameworkPropertyMetadata((double)DEFAULT_RESO, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Resonance
        {
            get { return (double)GetValue(ResonanceProperty); }
            set { SetValue(ResonanceProperty, value); }
        }

        #endregion Resonance

        #region Input

        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input",
                                                                                    typeof(AudioWire),
                                                                                    typeof(LowPassFilter),
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
                                                                                                              typeof(LowPassFilter),
                                                                                                              new PropertyMetadata());
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;

        public AudioWire Output
        {
            get { return (AudioWire)GetValue(OutputProperty); }
            private set { SetValue(OutputPropertyKey, value); }
        }

        #endregion Output

        #endregion I/O dependency properties

        static LowPassFilter()
        {
            InitializeClassMetadata<LowPassFilter>(WPF.Resources.LPFTemplate);
        }

        private readonly LowPassFilterCore _core;

        public LowPassFilter()
            : this(AudioLink.Instance)
        {
        }

        public LowPassFilter(IAudioLink audioLink)
            : base(audioLink)
        {
            SetCurrentValue(DescriptionProperty, "LP Filter");

            _core = new LowPassFilterCore(audioLink);
            InputValueChanged(null);
            Output = GetSample;
        }

        public AudioSample GetSample(ulong requestId)
        {
            return _core.GetSample(requestId);
        }

        protected override void InputValueChanged(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName) || inputName == CutoffPitchProperty.Name)
                _core.CutoffPitch.Value = (float)CutoffPitch;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == ResonanceProperty.Name)
                _core.Resonance.Value = (float)Resonance;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == InputProperty.Name)
                _core.SetInput(Input);
        }
    }
}
