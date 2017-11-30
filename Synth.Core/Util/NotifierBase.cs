using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Synth.Util
{
    public abstract class NotifierBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler evt = PropertyChanged;
            if (evt != null)
                evt.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T fieldVar, T value, [CallerMemberName] string propertyName = "")
        {
            bool wasChanged = false;

            if (!Equals(value, fieldVar))
            {
                wasChanged = true;
                fieldVar = value;
                OnPropertyChanged(propertyName);
            }

            return wasChanged;
        }
    }
}
