using System;

namespace Synth.WPF.Util
{
    /// <summary>
    /// Translates a value from one range to another, including linear/log scale conversion.
    /// </summary>
    public class RangeConverter
    {
        public RangeConverter()
        {
            SourceMin = 0;
            SourceMax = 1;
            TargetMin = 0;
            TargetMax = 1;
            ScaleFactor = 1;
            LimitSource = true;
            LimitTarget = true;
        }

        public double SourceMin { get; set; }
        public double SourceMax { get; set; }

        public double TargetMin { get; set; }
        public double TargetMax { get; set; }

        public double ScaleFactor { get; set; }

        public bool LimitSource { get; set; }
        public bool LimitTarget { get; set; }

        public double TargetToSource(double targetValue)
        {
            return Convert(targetValue, TargetMin, TargetMax, SourceMin, SourceMax, ScaleFactor, LimitSource);
        }

        public double SourceToTarget(double sourceValue)
        {
            return Convert(sourceValue, SourceMin, SourceMax, TargetMin, TargetMax, 1d / ScaleFactor, LimitTarget);
        }

        private static double Convert(double fromValue, double fromMin, double fromMax, double toMin, double toMax, double scaleFactor, bool forceIntoRange)
        {
            if (toMin == toMax)
                return toMin;

            if (toMin > toMax)
                throw new InvalidOperationException("Maximum output must be greater than Minimum output.");

            if (fromMin == fromMax)
                return (toMin + toMax)/2d;

            if (fromMin >= fromMax)
                throw new InvalidOperationException("Maximum input must be greater than Minimum input.");

            // scalefrom source range into normalized (i.e. 0 to 1) range
            double normValue = (fromValue - fromMin) / (fromMax - fromMin);

            // apply scale factor to normalized value if appropriate
            if (scaleFactor != 1)
                normValue = Math.Sign(normValue) * Math.Pow(Math.Abs(normValue), scaleFactor);

            // scale from normalized range into target range
            double toValue = toMin + (normValue * (toMax - toMin));

            // force result into range if appropriate
            if (forceIntoRange)
                toValue = ForceIntoRange(toValue, toMin, toMax);

            return toValue;
        }

        public double ForceIntoSourceRange(double value)
        {
            return ForceIntoRange(value, SourceMin, SourceMax);
        }

        public double ForceIntoTargetRange(double value)
        {
            return ForceIntoRange(value, TargetMin, TargetMax);
        }

        private static double ForceIntoRange(double value, double minimum, double maximum)
        {
            if (minimum == maximum)
            {
                value = minimum;
            }
            else
            {
                value = Math.Max(minimum, value);
                value = Math.Min(maximum, value);
            }

            return value;
        }
    }
}
