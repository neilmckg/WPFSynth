namespace Synth.Core
{
    // TODO: is this interface of value? Perhaps I should just reference ScaleNote instances
    public interface IScaleNote
    {
        string Name { get; }
        int MidiNoteNumber { get; }
        float Pitch { get; }
        bool IsWhiteKey { get; }
    }
}
