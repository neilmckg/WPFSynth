using System;

namespace Synth.Core
{
    public static class Pitch
    {
        // for an equal-tempered scale where A = 440hz:

        // C0 = pitch 0.0 =     16.35hz = MIDI NOTE 12
        // C1 = pitch 0.1 =     32.7 hz = MIDI NOTE 24
        // C2 = pitch 0.2 =     65.4 hz = MIDI NOTE 36
        // C3 = pitch 0.3 =    130.8 hz = MIDI NOTE 48
        // C4 = pitch 0.4 =    261.6 hz = MIDI NOTE 60 = "middle C"
        // C5 = pitch 0.5 =    523.2 hz = MIDI NOTE 72
        // C6 = pitch 0.6 =  1,046.4 hz = MIDI NOTE 84
        // C7 = pitch 0.7 =  2,092.8 hz = MIDI NOTE 96
        // C8 = pitch 0.8 =  4,185.6 hz = MIDI NOTE 108
        // C9 = pitch 0.9 =  8,371.2 hz = MIDI NOTE 120
        // C10= pitch 1.0 = 16,742.4 hz = MIDI NOTE 132

        // A4 = pitch 0.475 = 440 hz = MIDI NOTE 69

        private const float HALFSTEP_FREQ = 1.0594630943592952645618252949463f;
        private const float OCTAVE_PITCH = 0.1f;
        private const float HALFSTEP_PITCH = 0.008333333333333333333333333333f;
        private const float CENT_PITCH = 0.000083333333333333333333333333f;
        //private const float MIN_FREQ = 016.35160d;    // C0

        private static float _zeroFreq;

        public static void SetBaseFrequency(float freq)
        {
            if (freq <= 0)
                throw new ArgumentOutOfRangeException("freq");
            _zeroFreq = freq;
        }

        static Pitch()
        {
            // each 0.1 of pitch = 1 octave
            // 0.0 pitch = C0
            // 1.0 pitch = C10

            // For an equal tempered scale where C0 is pitch zero, A4 (traditional tuning note) is pitch 0.475
            // This puts pitch 0.475 at 440 Hz
            SetBaseFrequency(016.35160f);
        }

        public static float ToFreq(float pitch)
        {
            // TO DO:
            // reference to 1/2 step should not be necessary
            // this conversion should be completely calculable using only something like basefreq * (2 ^ (pitch*10))

            //float test1 = _zeroFreq * Math.Pow(2, pitch * 10d);
            //float test2 = _zeroFreq * Math.Pow(HALFSTEP_FREQ, pitch * 120d);
            //if (Math.Round(test1, 6) != Math.Round(test2, 6))
            //{
                
            //}

            return _zeroFreq * (float)Math.Pow(HALFSTEP_FREQ, pitch * 120f);
        }

        public static float FromFreq(float frequency)
        {
            // TO DO:
            // reference to 1/2 step should not be necessary
            // this conversion should be completely calculable using only basefreq and powers of 2, divided by 10

            return (1f / 120f) * 12 * (float)Math.Log(frequency / _zeroFreq) / (float)Math.Log(2);
        }

        public static float Octave
        {
            get { return OCTAVE_PITCH; }
        }

        public static float HalfStep
        {
            get { return HALFSTEP_PITCH; }
        }

        public static float Cent
        {
            get { return CENT_PITCH; }
        }
    }
}
