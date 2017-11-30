using System.ComponentModel;
using System.Windows;

using Synth.Audio;
using Synth.Module;

namespace Synth.WPF.Modules
{
    public sealed class Pan : Module, IAudioSource
    {
        #region I/O dependency properties

        #region Input

        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input",
                                                                                    typeof(AudioWire),
                                                                                    typeof(Pan),
                                                                                    new PropertyMetadata(BaseControlInputValueChanged));
        public AudioWire Input
        {
            get { return (AudioWire)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        #endregion Input

        #region Position

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position",
                                                                         typeof(double),
                                                                         typeof(Pan),
                                                                         new PropertyMetadata(0d, BaseControlInputValueChanged));
        // -1 = full left, 1 = full right
        [Bindable(true)]
        public double Position
        {
            get { return (double)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        #endregion Position

        #region Spread

        public static readonly DependencyProperty SpreadProperty = DependencyProperty.Register("Spread",
                                                                         typeof(double),
                                                                         typeof(Pan),
                                                                         new PropertyMetadata(1d, BaseControlInputValueChanged));
        // 1 = normal, 0 = mono, -1 = inverted.
        [Bindable(true)]
        public double Spread
        {
            get { return (double)GetValue(SpreadProperty); }
            set { SetValue(SpreadProperty, value); }
        }

        #endregion Spread

        #region Output

        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly("Output",
                                                                                                              typeof(AudioWire),
                                                                                                              typeof(Pan),
                                                                                                              new PropertyMetadata());
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;

        public AudioWire Output
        {
            get { return (AudioWire)GetValue(OutputProperty); }
            private set { SetValue(OutputPropertyKey, value); }
        }

        #endregion Output

        #endregion I/O dependency properties
                
        static Pan()
        {
            InitializeClassMetadata<Pan>(WPF.Resources.PanTemplate);
        }

        private readonly PanCore _core;

        public Pan()
            : this(AudioLink.Instance)
        {
        }

        public Pan(IAudioLink audioLink)
            : base(audioLink)
        {
            _core = new PanCore();
            InputValueChanged(null);
            Output = GetSample;
        }

        public AudioSample GetSample(ulong requestId)
        {
            return _core.GetSample(requestId);
        }

        protected override void InputValueChanged(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName) || inputName == SpreadProperty.Name)
                _core.Spread.Value = (float) Spread;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == PositionProperty.Name)
                _core.Position.Value = (float)Position;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == InputProperty.Name)
                _core.SetInput(Input);
        }
    }
}
