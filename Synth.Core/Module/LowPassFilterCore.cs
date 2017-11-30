using System;

using Synth.Audio;
using Synth.Core;

namespace Synth.Module
{
    public class LowPassFilterCore : ModuleCoreWithAudioOutput
    {
        private readonly IAudioLink _audioLink;
        private readonly LpfChannel _channelL = new LpfChannel();
        private readonly LpfChannel _channelR = new LpfChannel();

        private AudioWire _input;

        public LowPassFilterCore(IAudioLink audioLink, float initialCutoffPitch = 0.5f, float initialResonance = 0.2f)
        {
            if (audioLink == null)
                throw new ArgumentNullException("audioLink");
            _audioLink = audioLink;

            CutoffPitch = new FloatInput("Cutoff Pitch", initialCutoffPitch, 0, 0.85f, ValueOutOfRangeStrategy.ForceIntoRange, (name, v0, v1) => SetFilterCoefficients());
            Resonance = new FloatInput("Resonance", initialResonance, 0, 0.85f, ValueOutOfRangeStrategy.ForceIntoRange, (name, v0, v1) => SetFilterCoefficients());

            SetFilterCoefficients();
            _audioLink.SampleRateChanged += HandleQualityChanged;
        }

        private void HandleQualityChanged(object sender, EventArgs e)
        {
            SetFilterCoefficients();
        }

        public FloatInput CutoffPitch { get; private set; }
        public FloatInput Resonance { get; private set; }

        public void SetInput(AudioWire input)
        {
            _input = input;
        }

        protected override AudioSample CalculateNextSample(ulong requestId)
        {
            AudioSample sample = new AudioSample();

            if (_input != null)
            {
                sample = _input(requestId);
                float filteredL = _channelL.FilterSample(sample.L);
                float filteredR = _channelR.FilterSample(sample.R);
                sample = new AudioSample(filteredL, filteredR);
            }

            return sample;
        }

        private void SetFilterCoefficients()
        {
            float freq = Pitch.ToFreq(CutoffPitch.Value);
            float freqAsProportionOfRange = 2f * freq / _audioLink.SampleRate;        // range = 1/2 of sample rate (nyquist limit)

            _channelL.CutoffFrequency = freqAsProportionOfRange;
            _channelL.Resonance = Resonance.Value;

            _channelR.CutoffFrequency = freqAsProportionOfRange;
            _channelR.Resonance = Resonance.Value;
        }

        private class LpfChannel
        {
            // Filter algorithm from http://www.musicdsp.org/showArchiveComment.php?ArchiveID=26, with freq and reso constants adjusted

            // TODO use an oscilloscope to validate the freqFactor
            private const float _resonanceFactor = 4.7f;   // experimentally-derived multiplier to make it sound as dramatic as possible without breaking.
            private const float _freqFactor = 3.5f;        // experimentally=derived multiplier to make it sound as close to orig when fully open but not when it's just a little closed

            private float _in1 = 0;
            private float _in2 = 0;
            private float _in3 = 0;
            private float _in4 = 0;

            private float _out1 = 0;
            private float _out2 = 0;
            private float _out3 = 0;
            private float _out4 = 0;

            public float CutoffFrequency { get; set; }        // 0 - 1, proportion of frequency range (samplerate/2)
            public float Resonance { get; set; }              // 0 - 1, none to self-oscillation

            public float FilterSample(float input)
            {
                float res = _resonanceFactor * Resonance;
                float f = _freqFactor * CutoffFrequency;
                float fb = res * (1 - (0.15f * f * f));

                input -= (_out4 * fb);
                input *= (0.35013f * f * f * f * f);
                _out1 = input + 0.3f * _in1 + (1 - f) * _out1; // Pole 1
                _in1 = input;
                _out2 = _out1 + 0.3f * _in2 + (1 - f) * _out2; // Pole 2
                _in2 = _out1;
                _out3 = _out2 + 0.3f * _in3 + (1 - f) * _out3; // Pole 3
                _in3 = _out2;
                _out4 = _out3 + 0.3f * _in4 + (1 - f) * _out4; // Pole 4
                _in4 = _out3;

                return _out4;                
            }
        }
    }
}
