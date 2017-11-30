using System;

namespace Synth.Audio
{
    public struct AudioSample
    {
        public AudioSample(float l, float r)
        {
            L = l;
            R = r;
        }

        public readonly float L;
        public readonly float R;

        public static AudioSample operator +(AudioSample x, AudioSample y)
        {
            return new AudioSample(x.L + y.L, x.R + y.R);
        }

        public static AudioSample operator -(AudioSample x, AudioSample y)
        {
            return new AudioSample(x.L - y.L, x.R - y.R);
        }

        public static AudioSample operator *(AudioSample x, float y)
        {
            return new AudioSample(x.L * y, x.R * y);
        }

        public static AudioSample operator *(float y, AudioSample x)
        {
            return x * y;
        }

        public static AudioSample operator /(AudioSample x, float y)
        {
            if (y == 0)
                throw new DivideByZeroException();

            return new AudioSample(x.L / y, x.R / y);
        }

        public override string ToString()
        {
            return GetType().Name + ": " + L + ", " + R;
        }
    }
}
