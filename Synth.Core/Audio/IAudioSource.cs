namespace Synth.Audio
{
    public interface IAudioSource
    {
        AudioSample GetSample(ulong requestId);
    }
}
