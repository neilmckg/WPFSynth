using System;

using NAudio.Wave;
using Synth.Audio;

namespace Synth.NAudio
{
    public class AudioSampleProvider : ISampleProvider
    {
        private readonly Func<AudioSample> _sampleFactory;
        private readonly WaveFormat _format;

        public AudioSampleProvider(Func<AudioSample> sampleFactory, WaveFormat format)
        {
            if (sampleFactory == null)
                throw new ArgumentNullException("sampleFactory");
            if (format == null)
                throw new ArgumentNullException("format");

            _sampleFactory = sampleFactory;
            _format = format;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRequired = count / _format.Channels;

            for (int i = 0; i < samplesRequired; i++)
            {
                int indexOut = i * _format.Channels;

                AudioSample nextSample = _sampleFactory();

                buffer[indexOut] = (float)nextSample.L;
                buffer[indexOut + 1] = (float)nextSample.R;
            }

            return count;
        }

        public WaveFormat WaveFormat
        {
            get { return _format; }
        }
    }
}