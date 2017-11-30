using System;

namespace Synth.Core
{
    public class FloatInput : Input<float>
    {
        private const string UNDERMIN_EXCEPTION = "{0} must be greater than or equal to {1}.";
        private const string OVERMAX_EXCEPTION = "{0} must be less than or equal to {1}.";

        public FloatInput(string propertyName, float initialValue, float minValue, float maxValue, ValueOutOfRangeStrategy rangeStrategy, Action<string, float, float> changeHandler = null)
            : base(propertyName, initialValue, changeHandler)
        {
            if (minValue > maxValue)
                throw new ArgumentException("minValue must be less than or equal to maxValue.");

            Minimum = minValue;
            Maximum = maxValue;

            ValueOutOfRangeStrategy = rangeStrategy;

            ApplyRangeRule(ref initialValue);
            Value = initialValue;   // will trigger changeHandler if initial value was forced into range
        }

        public float Minimum { get; private set; }

        public float Maximum { get; private set; }

        public ValueOutOfRangeStrategy ValueOutOfRangeStrategy { get; private set; }

        protected override void ChangeValue(float oldValue, float newValue)
        {
            ApplyRangeRule(ref newValue, oldValue);   // if out of range, will throw or handle as appropriate
            base.ChangeValue(oldValue, newValue);
        }

        private void ApplyRangeRule(ref float newValue, float? oldValue = null)
        {
            if (this.ValueOutOfRangeStrategy != ValueOutOfRangeStrategy.Accept)
            {
                if (newValue < Minimum)
                {
                    if (this.ValueOutOfRangeStrategy == ValueOutOfRangeStrategy.Reject)
                        throw new ArgumentOutOfRangeException(string.Format(UNDERMIN_EXCEPTION, PropertyName, Minimum));
                    if (this.ValueOutOfRangeStrategy == ValueOutOfRangeStrategy.ForceIntoRange)
                        newValue = Minimum;
                    else if (oldValue.HasValue)
                        newValue = oldValue.Value;      // ignore new value
                    else
                        throw new ArgumentOutOfRangeException(string.Format(UNDERMIN_EXCEPTION, PropertyName, Minimum));
                }
                else if (newValue > Maximum)
                {
                    if (this.ValueOutOfRangeStrategy == ValueOutOfRangeStrategy.Reject)
                        throw new ArgumentOutOfRangeException(string.Format(OVERMAX_EXCEPTION, PropertyName, Maximum));
                    if (this.ValueOutOfRangeStrategy == ValueOutOfRangeStrategy.ForceIntoRange)
                        newValue = Maximum;
                    else if (oldValue.HasValue)
                        newValue = oldValue.Value;      // ignore new value
                    else
                        throw new ArgumentOutOfRangeException(string.Format(OVERMAX_EXCEPTION, PropertyName, Maximum));

                }
            }
        }
    }
}
