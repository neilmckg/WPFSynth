using System;
using System.ComponentModel;

namespace Synth.Core
{
    public class Input<T> : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs _propertyChangedArgs = new PropertyChangedEventArgs("Value");

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Action<string, T, T> _changeHandler;

        private T _value;

        public Input(string propertyName, T initialValue, Action<string, T, T> changeHandler = null)
        {
            PropertyName = propertyName;
            _value = initialValue;
            _changeHandler = changeHandler;
        }

        public string PropertyName { get; private set; }

        public T Value
        {
            get { return _value; }
            set
            {
                T oldValue = Value;
                if (!Equals(oldValue, value))
                    ChangeValue(oldValue, value);
            }
        }

        protected virtual void ChangeValue(T oldValue, T newValue)
        {
            _value = newValue;

            if (_changeHandler != null)
                _changeHandler(PropertyName, oldValue, newValue);

            OnPropertyChanged();
        }

        private void OnPropertyChanged()
        {
            PropertyChangedEventHandler evt = PropertyChanged;
            if (evt != null)
                evt.Invoke(this, _propertyChangedArgs);
        }
    }
}
