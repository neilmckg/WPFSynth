using System;

using Synth.Audio;
using Synth.Core;
using Synth.Util;

namespace Synth.Module
{
    public class OscillatorCore : ModuleCoreWithAudioOutput
    {
        // TODO is this constant helping?
        private const float MAX_PITCH_CHANGE_PER_SAMPLE = 0.002f;
        private const float DEFAULT_PITCH = 0.475f;     // A440

        private readonly ThrottledFloat _actualPitch = new ThrottledFloat(MAX_PITCH_CHANGE_PER_SAMPLE, DEFAULT_PITCH);
        private WaveHost _waveHost;
        private float _frequency;
        private readonly IAudioLink _audioLink;

        public OscillatorCore(IAudioLink audioLink, float initialPitch = DEFAULT_PITCH)
        {
            if (audioLink == null)
                throw new ArgumentNullException("audioLink");
            _audioLink = audioLink;

            InitializeInputs(initialPitch);

            HandleQualityChanged(null, null);
            _audioLink.SampleRateChanged += HandleQualityChanged;
        }

        private void HandleQualityChanged(object sender, EventArgs e)
        {
            _waveHost = new WaveHost(_audioLink.SampleRate);
            _waveHost.Wave = Wave.Value;
            _actualPitch.ForceValue(CalculateCompositePitch());     // jump to initial pitch with no smoothing
        }

        public FloatInput Pitch { get; private set; }

        public FloatInput PitchOffsetHalfSteps { get; private set; }

        public Input<Wave> Wave { get; private set; }

        public FloatInput Level { get; private set; }

        protected override AudioSample CalculateNextSample(ulong requestId)
        {  
            if (_actualPitch.HasNewValue)
            {
                float newPitch = _actualPitch.GetNextSample();
                _frequency = Synth.Core.Pitch.ToFreq(newPitch);
            }

            AudioSample sample = _waveHost.GetNextSample(_frequency);
            sample *= Level.Value;

            return sample;
        }

        private float CalculateCompositePitch()
        {
            return this.Pitch.Value + (PitchOffsetHalfSteps.Value * Synth.Core.Pitch.HalfStep);
        }

        private void InitializeInputs(float initialPitch)
        {
            Pitch = new FloatInput("Pitch", initialPitch, 0, 1, ValueOutOfRangeStrategy.Accept, (name, v0, v1) => _actualPitch.Target = CalculateCompositePitch());
            PitchOffsetHalfSteps = new FloatInput("PitchOffsetHalfSteps", 0, -24, 24, ValueOutOfRangeStrategy.ForceIntoRange, (name, v0, v1) => _actualPitch.Target = CalculateCompositePitch());
            Wave = new Input<Wave>("Wave", Synth.Core.Wave.Sine, (name, v0, v1) => _waveHost.Wave = v1);
            Level = new FloatInput("Level", 1, 0, 1, ValueOutOfRangeStrategy.ForceIntoRange);
        }
    }
}
