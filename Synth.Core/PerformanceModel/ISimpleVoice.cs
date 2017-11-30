using System.ComponentModel;

namespace Synth.PerformanceModel
{
    public interface ISimpleVoice : INotifyPropertyChanged
    {
        float Pitch { get; }
        float Intensity { get; }
        bool IsActive { get; }
    }
}
