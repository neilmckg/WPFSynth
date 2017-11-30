using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

using Synth.Audio;
using Synth.Module;
using Synth.Util;

namespace Synth.WPF.Modules
{
    public sealed class LFO : Module
    {
        #region I/O dependency properties

        #region Frequency

        public static readonly DependencyProperty FrequencyProperty = DependencyProperty.Register("Frequency",
                                                                                     typeof(double),
                                                                                     typeof(LFO),
                                                                                     new FrameworkPropertyMetadata(2.5d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Frequency
        {
            get { return (double)GetValue(FrequencyProperty); }
            set { SetValue(FrequencyProperty, value); }
        }

        #endregion Frequency

        #region Amount

        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount",
                                                                                     typeof(double),
                                                                                     typeof(LFO),
                                                                                     new FrameworkPropertyMetadata(0d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double Amount
        {
            get { return (double)GetValue(AmountProperty); }
            set { SetValue(AmountProperty, value); }
        }

        #endregion Amount

        #region Wave

        public static readonly DependencyProperty WaveProperty = DependencyProperty.Register("Wave",
                                                                                    typeof(Synth.Core.Wave),
                                                                                    typeof(LFO),
                                                                                    new FrameworkPropertyMetadata(Synth.Core.Wave.Sine, BaseControlInputValueChanged));
        [Bindable(true)]
        public Synth.Core.Wave Wave
        {
            get { return (Synth.Core.Wave)GetValue(WaveProperty); }
            set { SetValue(WaveProperty, value); }
        }

        #endregion Wave

        #region Value (output)

        private static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly("Value",
                                                                                     typeof(double),
                                                                                     typeof(LFO),
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

        static LFO()
        {
            InitializeClassMetadata<LFO>(WPF.Resources.LFOTemplate);
        }

        private readonly LfoCore _core;

        public LFO()
            : this(AudioLink.Instance)
        {
        }

        public LFO(IAudioLink audioLink)
            : base(audioLink)
        {
            // TODO: supress tick handling until loading is complete?

            _core = new LfoCore(audioLink);
            _core.OutputChanged += HandleCoreOutputChanged;
            InputValueChanged(null);
            Unloaded += (s, e) => _core.Dispose();
        }

        private void HandleCoreOutputChanged(object sender, EventArgs<float> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => Value = _core.Output));
        }

        protected override void InputValueChanged(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName) || inputName == FrequencyProperty.Name)
                _core.Frequency.Value = (float)Frequency;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == AmountProperty.Name)
                _core.Amount.Value = (float)Amount;

            if (string.IsNullOrWhiteSpace(inputName) || inputName == WaveProperty.Name)
                _core.Wave.Value = Wave;
        }
    }
}
