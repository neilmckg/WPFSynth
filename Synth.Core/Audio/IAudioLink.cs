using System;
using Synth.Core;

namespace Synth.Audio
{
    public interface IAudioLink : IDisposable
    {
        event EventHandler SampleRateChanged;

        void Initialize(IFinalOutput output);

        void AttachClockListener(IClockListener item);
        void DetachClockListener(IClockListener item);
        void AttachSource(IAudioSource item);
        void DetachSource(IAudioSource item);

        void Activate();
        void Deactivate();
        bool IsActive { get; }

        int SampleRate { get; }
    }
}

