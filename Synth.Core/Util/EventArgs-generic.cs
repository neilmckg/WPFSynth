using System;

namespace Synth.Util
{
    public class EventArgs<TValue> : EventArgs
    {
        public EventArgs(TValue item)
        {
            Value = item;
        }

        public TValue Value { get; private set; }
    }
}
