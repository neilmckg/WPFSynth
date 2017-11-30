using System.Collections.ObjectModel;

namespace Synth.Util
{
    public class ObservableCollectionExtended<T> : ObservableCollection<T>, IReadOnlyObservableCollection<T>
    {
        // this class exists to enable the covariant readonly interface, the underlying collection can be of an 
        //  editable instance type, but the readonly collection can be of a restricted interface type.
    }
}
