using Synth.Audio;

namespace Synth.Module
{
    public class MixerChannel : IMixerChannel
    {
        public AudioWire Input { get; set; }

        public float Level { get; set; }

        public AudioSample GetSample(ulong requestId)
        {
            AudioSample sample = new AudioSample();

            if (Input != null)
                sample = Input(requestId);

            return sample;
        }
    }
}
