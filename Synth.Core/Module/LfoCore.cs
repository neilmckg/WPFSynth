using System;
using Synth.Audio;
using Synth.Core;

namespace Synth.Module
{
    public class LfoCore : ModuleCoreWithControlOutput, IDisposable
    {
        private readonly IAudioLink _audioLink;
        private WaveHost _waveHost;
        private readonly AudioClockDivider _clockSource;

        public LfoCore(IAudioLink audioLink, float initialFrequency = 5, float initialAmount = 0)
        {
            if (audioLink == null)
                throw new ArgumentNullException("audioLink");
            _audioLink = audioLink;

            InitializeInputs(initialFrequency, initialAmount);

            _clockSource = new AudioClockDivider(_audioLink, 100, id => HandleControlClockTick());

            HandleQualityChanged(null, null);
            _audioLink.SampleRateChanged += HandleQualityChanged;
        }

        public FloatInput Frequency { get; private set; }
        public Input<Wave> Wave { get; private set; }
        public FloatInput Amount { get; private set; }

        private void HandleQualityChanged(object sender, EventArgs e)
        {
            _waveHost = new WaveHost(_clockSource.TicksPerSecond);
            _waveHost.Wave = Wave.Value;
        }

        private void HandleControlClockTick()
        {
            float newValue = _waveHost.GetNextSample(Frequency.Value).L;

            if (IsActive)
                newValue *= Amount.Value;
            else
                newValue = 0;

            Output = newValue;
        }

        private void InitializeInputs(float initialFrequency, float initialAmount)
        {
            Frequency = new FloatInput("Frequency", initialFrequency, 0.1f, 10, ValueOutOfRangeStrategy.ForceIntoRange);
            Wave = new Input<Wave>("Wave", Synth.Core.Wave.Sine, (name, v0, v1) => this._waveHost.Wave = v1);
            Amount = new FloatInput("Amount", initialAmount, 0, 1, ValueOutOfRangeStrategy.ForceIntoRange);
        }

        public void Dispose()
        {
            if (_clockSource != null)
                _clockSource.Dispose();
        }
    }
}
