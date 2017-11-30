using Synth.Audio;
using Synth.Core;

namespace Synth.Module
{
    public class PanCore : ModuleCoreWithAudioOutput
    {
        private AudioWire _input;

        public PanCore()
        {
            Position = new FloatInput("Position", 0, -1, 1, ValueOutOfRangeStrategy.ForceIntoRange);
            Spread = new FloatInput("Spread", 1, -1, 1, ValueOutOfRangeStrategy.ForceIntoRange);
        }

        public FloatInput Position { get; private set; }
        public FloatInput Spread { get; private set; }

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
                sample = ApplySpread(sample);
                sample = ApplyPosition(sample);
            }

            return sample;
        }

        private AudioSample ApplySpread(AudioSample sample)
        {
            if (Spread.Value == 1)
            {
                // leave as is
                // this is the same result as the algorithm below, but is here as an optimizable special case
            }
            else if (Spread.Value == -1)
            {
                // invert
                // this is the same result as the algorithm below, but is here as an optimizable special case
                sample = new AudioSample(sample.R, sample.L);
            }
            else
            {
                float spreadFactor = (Spread.Value + 1) / 2f;      // 0 = inverted, 0.5 = mono, 1 = normal
                float l = (sample.R * (1 - spreadFactor)) + (sample.L * spreadFactor);
                float r = (sample.L * (1 - spreadFactor)) + (sample.R * spreadFactor);
                sample = new AudioSample(l, r);
            }

            return sample;
        }

        private AudioSample ApplyPosition(AudioSample sample)
        {
            // LEFT CHANNEL:
            //  pos = 1,  gain = 0
            //  pos = 0,  gain = 1
            //  pos = -1, gain = 2
            // RIGHT CHANNEL:
            //  pos = 1,  gain = 2
            //  pos = 0,  gain = 1
            //  pos = -1, gain = 0

            if (Position.Value != 0)
            {
                float leftVal = sample.L * (1 - Position.Value);
                float rightVal = sample.R * (1 + Position.Value);
                sample = new AudioSample(leftVal, rightVal);
            }

            return sample;
        }
    }
}
