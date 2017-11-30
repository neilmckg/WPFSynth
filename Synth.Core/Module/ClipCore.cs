using System;
using Synth.Core;
using Synth.Audio;

namespace Synth.Module
{
    public class ClipCore : ModuleCoreWithAudioOutput
    {
        private AudioWire _input;

        public ClipCore(float initialAmount = 0)
        {
            Amount = new FloatInput("Amount", initialAmount, 0, 1, ValueOutOfRangeStrategy.ForceIntoRange);
        }

        public FloatInput Amount { get; private set; }

        public void SetInput(AudioWire input)
        {
            _input = input;
        }

        protected override AudioSample CalculateNextSample(ulong requestId)
        {
            AudioSample result = new AudioSample();

            if (_input != null)
            {
                result = _input(requestId);

                float left = ShapeWave(result.L, Amount.Value);
                float right = ShapeWave(result.R, Amount.Value);

                result = new AudioSample(left, right);
            }

            return result;
        }

        private float ShapeWave(float value, float clippingAmount)
        {
            // Clipping algorithm from http://dsp.stackexchange.com/questions/13142/digital-distortion-effect-algorithm 
            // The output of this formula always falls between -1 and 1.
            // The amount of clipping is determined by the amplitude of the input value.
            // Values close to zero are almost unchanged, and high values approach a square wave.
            value *= (1f + (clippingAmount * 20f));

            value = Math.Sign(value) * (1f - (float)Math.Pow(Math.E, -Math.Abs(value)));

            // Custom curve to compensate for clipping's effect on perceived loudness
            // TODO: I don't thing the curve is a perfect fit -- there's a bump around 0.1. You can hear it, and see it on an oscilloscope
            float loudnessCompensation = 0.1f + (1.3f / (float)Math.Pow(clippingAmount + 1f, 3.3f));
            value *= loudnessCompensation;

            return value;
        }
    }
}
