using System.Collections.Generic;
using System.Collections.Specialized;

namespace Synth.Util
{
    public interface IReadOnlyObservableCollection<out T> : INotifyCollectionChanged, IReadOnlyList<T>
    {
    }
}