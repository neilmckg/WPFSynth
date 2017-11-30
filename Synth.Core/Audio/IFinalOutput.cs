using System;

namespace Synth.Audio
{
    public interface IFinalOutput : IDisposable
    {
        void SetAudioSource(AudioWire source);
        void Start();
        void Stop();
        int SamplesPerSecond { get; }
    }
}