namespace Synth.MIDI
{
    public interface IMidiListener
    {
        MidiChannel Channel { get; }

        // The events and data supported are currently limited by the nAudio MIDI implementation.
        // TODO full MIDI performance model, even if most of it will not be used initially

        void HandleNoteOn(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni);

        void HandleNotePressure(int noteNumber, int value, MidiChannel channel = MidiChannel.Omni);
        void HandleNoteOff(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni);

        void HandlePitchWheel(int value, MidiChannel channel = MidiChannel.Omni);
        void HandleChannelPressure(int value, MidiChannel channel = MidiChannel.Omni);
        void HandleControlChange(int controllerNumber, int value, MidiChannel channel = MidiChannel.Omni);

        void HandleAllNotesOff(MidiChannel channel = MidiChannel.Omni);
        void HandleResetControllers(MidiChannel channel = MidiChannel.Omni);
    }
}
