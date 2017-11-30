using System;
using System.Collections.Generic;
using System.ComponentModel;

using Synth.MIDI;

namespace Synth.PerformanceModel
{
    public interface ISimplePerformance : INotifyPropertyChanged
    {
        #region configuration

        MidiChannel Channel { get; set; }

        float PitchBendRange { get; set; }
        int NumberOfVoices { get; set; }
        bool IsLegato { get; set; }

        ExpressionSources IntensitySource { get; set; }
        SwitchSources SwitchSource { get; set; }

        bool ApplyHoldPedalToSustain { get; set; }
        
        #endregion configuration

        #region performance state
        
        bool Switch { get; }
        float ModulationAmount { get; }
        IReadOnlyCollection<ISimpleVoice> Voices { get; }

        #endregion performance state

        #region native performance injection

        Guid? InjectNote(float pitch, float intensity);
        void ReleaseNote(Guid id);
        void UpdateNote(Guid id, float pitch, float intensity);
        void InjectSwitch(bool state);
        void InjectModulation(float amount);

        #endregion local performance injection
    }

    // TODO is there a better place for these enums?

    [Flags]
    public enum ExpressionSources : int
    {
        None = 0,
        ChannelPressure = 1,
        BreathController = 2,
        FootPedal = 4,
        Expression = 8
    }

    [Flags]
    public enum SwitchSources : int
    {
        None = 0,
        HoldPedal = 1,
        SustenutoPedal = 2,
        SoftPedal = 4,
        PortamentoSwitch = 8,
        LegatoSwitch = 16
    }
}
