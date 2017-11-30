using System.Collections.Generic;

namespace Synth.Core
{
    public interface IScale : IReadOnlyList<IScaleNote>
    {
        string Name { get; }
        bool ContainsKey(string key);
        IScaleNote this[string key] { get; }
    }
}