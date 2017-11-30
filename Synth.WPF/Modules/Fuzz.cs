using System.ComponentModel;
using System.Windows;

using Synth.Module;
using Synth.Audio;

namespace Synth.WPF.Modules
{
    public sealed class Fuzz : Module, IAudioSource
    {
        #region I/O dependency properties

        #region Amount

        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount",
                                                                                     typeof(double),
                                                                                     typeof(Fuzz),
                                                                                     new FrameworkPropertyMetadata(0.3d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Amount
        {
            get { return (double)GetValue(AmountProperty); }
            set { SetValue(AmountProperty, value); }
        }

        #endregion Amount

        #region Input

        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input",
                                                                                    typeof(AudioWire),
                                                                                    typeof(Fuzz),
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
                                                                                                              typeof(Fuzz),
                                                                                                              new PropertyMetadata());
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;

        public AudioWire Output
        {
            get { return (AudioWire)GetValue(OutputProperty); }
            private set { SetValue(OutputPropertyKey, value); }
        }

        #endregion Output

        #endregion I/O dependency properties
        
        static Fuzz()
        {
            InitializeClassMetadata<Fuzz>(WPF.Resources.FuzzTemplate);
        }

        private readonly ClipCore _core;

        public Fuzz() 
            : this(AudioLink.Instance)
        {
        }

        public Fuzz(IAudioLink audioLink)
            : base(audioLink)
        {
            _core = new ClipCore();
            InputValueChanged(null);
            Output = GetSample;
        }

        public AudioSample GetSample(ulong requestId)
        {
            return _core.GetSample(requestId);
        }

        protected override void InputValueChanged(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName) || inputName == AmountProperty.Name)
                _core.Amount.Value = (float)Amount;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == InputProperty.Name)
                _core.SetInput(this.Input);
        }
    }
}
