using Synth.Audio;

namespace Synth.Core
{
    public class WaveHost
    {
        private int _sampleRate;

        private float _lastFrequency = -1;
        private float _nextIndex = 0;
        private float _lastDelta = 0;

        private IWave _wave;

        public WaveHost(int sampleRate)
        {
            _sampleRate = sampleRate;
        }

        public IWave Wave
        {
            get { return _wave; }
            set
            {
                if (_wave == value)
                    return;

                _wave = value;
                _lastFrequency = -1;
                _nextIndex = 0;
                _lastDelta = 0;
            }
        }

        public AudioSample GetNextSample(float frequency)
        {
            if (_wave == null)
                return new AudioSample();

            float indexDeltaPerSample;
            if (frequency == _lastFrequency)
            {
                // optimization: only calc the delta when the freq changes
                indexDeltaPerSample = _lastDelta;
            }
            else
            {
                // delta = WaveTable elements per sample at the current frequency
                indexDeltaPerSample = (_wave.Count * frequency) / _sampleRate;
                _lastDelta = indexDeltaPerSample;
                _lastFrequency = frequency;
            }

            // For higher quality use the fractional part of index to interpolate
            //  between WaveTable[(int)index] and WaveTable[(int)index + 1], rather than just
            //  truncating the index and using WaveTable[(int)index] only. Depends on both
            //  WaveTable.Length and required output quality.
            // That would allow the same quality at much smaller table sizes, which would save both memory and time
            int index = (int)_nextIndex;
            if (index == _wave.Count)
                index = 0;
            AudioSample result = _wave[index];    // cast rounds up/down

            _nextIndex = (_nextIndex + indexDeltaPerSample) % _wave.Count;

            return result;
        }
    }
}
