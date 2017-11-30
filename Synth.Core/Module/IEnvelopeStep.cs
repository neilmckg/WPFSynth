using System;

namespace Synth.Module
{
    public interface IEnvelopeStep
    {
        event EventHandler Changed;
        
        float Seconds { get; set; }
        float TargetValue { get; set; }
    }
}
