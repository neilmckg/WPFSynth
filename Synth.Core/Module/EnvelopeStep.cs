using System;

using Synth.Util;

namespace Synth.Module
{
    public class EnvelopeStep : NotifierBase, IEnvelopeStep
    {
        private static readonly EventArgs _args = new EventArgs();

        public event EventHandler Changed;

        private float _seconds;
        public float Seconds
        {
            get { return _seconds; }
            set
            {
                if (SetField(ref _seconds, value))
                    OnChanged();
            }
        }

        private float _targetValue;
        public float TargetValue
        {
            get { return _targetValue; }
            set
            {
                if (SetField(ref _targetValue, value))
                    OnChanged();
            }
        }

        private void OnChanged()
        {
            EventHandler evt = Changed;
            if (evt != null)
                evt.Invoke(this, _args);
        }
    }
}