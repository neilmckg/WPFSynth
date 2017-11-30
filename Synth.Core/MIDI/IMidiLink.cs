using System;
using Synth.MIDI;

namespace Synth.PerformanceModel
{
    public interface IMidiLink : IDisposable
    {
        void AttachListener(IMidiListener listener);
        void DetachListener(IMidiListener listener);

        void InjectNoteOn(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni);

        void InjectNotePressure(int noteNumber, int value, MidiChannel channel = MidiChannel.Omni);
        void InjectNoteOff(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni);

        void InjectPitchWheel(int value, MidiChannel channel = MidiChannel.Omni);
        void InjectChannelPressure(int value, MidiChannel channel = MidiChannel.Omni);
        void InjectControlChange(int controllerNumber, int value, MidiChannel channel = MidiChannel.Omni);

        void InjectAllNotesOff(MidiChannel channel = MidiChannel.Omni);
        void InjectResetControllers(MidiChannel channel = MidiChannel.Omni);

        void RepublishState(IMidiListener toListener = null);
    }
}
