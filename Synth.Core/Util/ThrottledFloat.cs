using System;

namespace Synth.Util
{
    // Encapsulates a value that changes gradually when checked at regular intervals, until it reaches a target value.
    public class ThrottledFloat
    {
        private bool _isReset = true;
        private readonly float _maxChangePerSample;
        private float _lastValue;

        public ThrottledFloat(float maxChangePerSample, float initialTarget)
        {
            _maxChangePerSample = maxChangePerSample;
            ForceValue(initialTarget);
        }

        public float Target { get; set; }

        public float GetNextSample()
        {
            _isReset = false;

            float idealChange = Target - _lastValue;
            // Limiting the change per sample smooths the transition between states
            float throttledChangeAmount = Math.Min(_maxChangePerSample, Math.Abs(idealChange));

            if (throttledChangeAmount < _maxChangePerSample)
                // this ensures we arrive there exactly, which we might never otherwise do by adding tiny floats
                _lastValue = Target;
            else
                _lastValue += (throttledChangeAmount * Math.Sign(idealChange));

            return _lastValue;
        }

        public bool HasNewValue
        {
            get { return _isReset || Target != _lastValue; }
        }

        public void ForceValue(float value)
        {
            _lastValue = value;
            Target = value;
            _isReset = true;
        }
    }
}
