using System.ComponentModel;
using Synth.MIDI;
using Synth.Util;

namespace Synth.PerformanceModel
{
    /// <summary>
    /// A stateful representation of the main performance events of a MIDI stream.
    /// </summary>
    public interface IMidiPerformance : INotifyPropertyChanged
    {
        // The controllers supported are currently limited by the nAudio MIDI implementation

        /// <summary>
        /// Gets a value that indicates whether the MidiPerformance will report continuous 
        /// values using MIDI's raw 0-127 values, or values normalized into the 0 - 1 range.
        /// </summary>
        MidiValueStrategy ValueStrategy { get; }

        /// <summary>
        /// The MIDI Channel from which events will be reported. The default is MidiChannel.Omni.
        /// </summary>
        MidiChannel Channel { get; set; }

        /// <summary>
        /// Notes corresponding to currently pressed keys. When the corresponding key is released, notes 
        /// are immediately removed from the list, regardless of whether the hold pedal is active or the 
        /// note has a long decay time.
        /// </summary>
        IReadOnlyObservableCollection<IMidiNote> ActiveNotes { get; }

        float Pressure { get; }
        float PitchBend { get; }

        float Modulation { get; }
        float BreathController { get; }
        float FootPedal { get; }
        float Volume { get; }
        float Pan { get; }
        float Expression { get; }
        bool HoldPedal { get; }
        bool Portamento { get; }
        bool SustenutoPedal { get; }
        bool SoftPedal { get; }
        bool Legato { get; }
    }
}
