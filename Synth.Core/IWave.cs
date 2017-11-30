using System.Collections.Generic;
using Synth.Audio;

namespace Synth.Core
{
    public interface IWave: IReadOnlyList<AudioSample>
    {
        string Name { get; }
    }
}
