using System;

namespace Synth.Core
{
    public class ScaleNote : IScaleNote
    {
        public ScaleNote(string name, int midiNoteNumber, float pitch)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Each note must have a name.");

            Name = name;
            MidiNoteNumber = midiNoteNumber;
            Pitch = pitch;
            IsWhiteKey = !Name.Contains("#");
            HasValue = true;
        }

        public string Name { get; }
        public int MidiNoteNumber { get; }
        public float Pitch { get; }
        public bool IsWhiteKey { get; }
        public bool HasValue { get; }
    }
}
