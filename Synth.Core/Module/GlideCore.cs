using System;

using Synth.Audio;
using Synth.Core;
using Synth.Util;

namespace Synth.Module
{
    public class GlideCore : ModuleCoreWithControlOutput, IDisposable
    {
        private const float MAX_GLIDE_SECONDS = 10;

        private readonly IAudioLink _audioLink;
        private ThrottledFloat _throttledValue = new ThrottledFloat(float.MaxValue, 0);
        private readonly AudioClockDivider _clockSource;

        public GlideCore(IAudioLink audioLink)
        {
            if (audioLink == null)
                throw new ArgumentNullException("audioLink");
            _audioLink = audioLink;

            SourceValue = new FloatInput("Source Value", 0, float.MinValue, float.MaxValue, ValueOutOfRangeStrategy.Accept, (name, v0, v1) => HandleSourceValueChanged(v1));
            Rate = new FloatInput("Rate", 1, 0, 1, ValueOutOfRangeStrategy.ForceIntoRange, (name, v0, v1) => HandleRateChanged());

            HandleQualityChanged(null, null);
            _audioLink.SampleRateChanged += HandleQualityChanged;

            _clockSource = new AudioClockDivider(_audioLink, 100, id => HandleControlClockTick());
        }

        private void HandleQualityChanged(object sender, EventArgs e)
        {
            HandleRateChanged();
        }

        public FloatInput SourceValue { get; private set; }

        // 0 = Min rate = slow glide
        // 1 = max rate = instant transition
        public FloatInput Rate { get; private set; }

        private void HandleSourceValueChanged(float newValue)
        {
            if (!IsActive)
            {
                _throttledValue.ForceValue(newValue);
            }
            else
            {
                _throttledValue.Target = newValue;
            }
        }

        private void HandleRateChanged()
        {
                float changePerSample;
                if (Rate.Value == 1)
                    changePerSample = float.MaxValue;
                else
                    changePerSample = 1f/(_clockSource.TicksPerSecond*MAX_GLIDE_SECONDS*(1f - Rate.Value));

                _throttledValue = new ThrottledFloat(changePerSample, SourceValue.Value);
        }

        public void HandleControlClockTick()
        {
            if (_throttledValue.HasNewValue)
                Output = _throttledValue.GetNextSample();
        }

        public void Dispose()
        {
            if (_clockSource != null)
                _clockSource.Dispose();
        }
    }
}
