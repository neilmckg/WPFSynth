using System;
using System.ComponentModel;

namespace Synth.WPF.Util
{
    // Adds a single read/write value to an object by wrapping it.
    public class ItemExtender<TItem, TValue> : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs _args = new PropertyChangedEventArgs("Value");

        public event PropertyChangedEventHandler PropertyChanged;

        public ItemExtender(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            Item = item;
        }

        public ItemExtender(TItem item, TValue initialValue)
            : this(item)
        {
            _value = initialValue;   // may be null
        }

        public TItem Item { get; private set; }

        private TValue _value;
        public TValue Value
        {
            get { return _value; }
            set 
            {
                if (!Equals(value, _value))
                {
                    _value = value;
                    OnValueChanged();
                }
            }
        }

        public override string ToString()
        {
            return Item.ToString() + ":" + (Value as object ?? "Null").ToString();
        }

        private void OnValueChanged()
        {
            PropertyChangedEventHandler evt = PropertyChanged;
            if (evt != null)
                evt.Invoke(this, _args);
        }
    }
}
