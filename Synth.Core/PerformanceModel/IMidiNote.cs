using System;
using System.ComponentModel;

using Synth.MIDI;

namespace Synth.PerformanceModel
{
    public interface IMidiNote : INotifyPropertyChanged
    {
        int Number { get; }
        float Velocity { get; }
        float Pressure { get; }
        MidiChannel Channel { get; }
        bool IsReleased { get; }
        DateTime StartTime { get; }
    }
}
