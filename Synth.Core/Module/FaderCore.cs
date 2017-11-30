using Synth.Audio;
using Synth.Core;

namespace Synth.Module
{
    public class FaderCore : ModuleCoreWithAudioOutput
    {
        private AudioWire _input;

        public FaderCore()
        {
            Level = new FloatInput("Level", 1, 0, 1, ValueOutOfRangeStrategy.ForceIntoRange);
        }

        public FloatInput Level { get; private set; }

        public void SetInput(AudioWire input)
        {
            _input = input;
        }

        protected override AudioSample CalculateNextSample(ulong requestId)
        {
            AudioSample newSample;

            if (_input != null)
                newSample = _input(requestId) * Level.Value;
            else
                newSample = new AudioSample();

            return newSample;
        }
    }
}
