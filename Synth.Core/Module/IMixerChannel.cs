using Synth.Audio;

namespace Synth.Module
{
    public interface IMixerChannel : IAudioSource
    {
        AudioWire Input { get; set; }
        float Level { get; set; }
    }
}
