namespace Synth.MIDI
{
    // These are all of the MIDI messages we care about. Some others may be mapped into these, like BreathControl -> Pressure
    // TODO implement full MIDI performance model, even though most of it won't be used for now.
    public enum MidiAction
    {
        NoteOn,
        NoteOff,
        Pressure,
        Modulation,
        PitchBend,
        Sustain,
        Panic
    }
}
