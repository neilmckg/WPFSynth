using System;
using Synth.Audio;
using Synth.Core;

namespace Synth.Module
{
    public class NoiseCore : ModuleCoreWithAudioOutput
    {
        private readonly Random _random = new Random();

        public NoiseCore()
        {
            Level = new FloatInput("Level", 1, 0, 1, ValueOutOfRangeStrategy.ForceIntoRange);
        }

        public FloatInput Level { get; private set; }

        protected override AudioSample CalculateNextSample(ulong requestId)
        {
            float valueL = (float)(2 * _random.NextDouble()) - 1;
            float valueR = (float)(2 * _random.NextDouble()) - 1;

            return new AudioSample(valueL, valueR) * Level.Value;
        }
    }
}
