using System;

using NAudio.Wave;
using Synth.Audio;

namespace Synth.NAudio
{
    public abstract class FinalOutputBase : IFinalOutput
    {
        private const int NUMBER_OF_CHANNELS = 2;

        private AudioWire _audioSource;
        private IWavePlayer _wavePlayer;
        private readonly WaveFormat _waveFormat;

        // At 44,100 requests/second, it will take 13.3 million years to get to ulong.MaxValue (vs. 27 hours for a regular uint).
        // If this software crashes after running continuously for 13 million years, I'll call that a win.
        private ulong _requestId = 0;

        protected FinalOutputBase(int sampleRate)
        {
            if (!IsSampleRateValidForDriver(sampleRate))
                throw new ArgumentException("Invalid sample rate: " + sampleRate);

            _waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, NUMBER_OF_CHANNELS);
        }

        #region IFinalOutput

        public void Start()
        {
            _wavePlayer = GetWavePlayer();
            if (_wavePlayer == null)
                throw new InvalidOperationException("Classes derived from FinalOutputBase must return a non-null value from GetWavePlayer().");

            _wavePlayer.Init(new AudioSampleProvider(GetNextOutputValue, _waveFormat));
            _wavePlayer.Play();
        }

        public void SetAudioSource(AudioWire source)
        {
            // null is ok -- disables the output.
            _audioSource = source;
        }

        public void Stop()
        {
            if (_wavePlayer != null)
            {
                _wavePlayer.Stop();
                _wavePlayer.Dispose();
                _wavePlayer = null;
            }
        }

        public int SamplesPerSecond
        {
            get { return _waveFormat.SampleRate; }
        }

        #endregion IFinalOutput

        #region IDisposable

        public void Dispose()
        {
            Stop();
        }

        #endregion IDisposable

        protected abstract IWavePlayer GetWavePlayer();

        public abstract bool IsSampleRateValidForDriver(int sampleRate);

        private AudioSample GetNextOutputValue()
        {
            // TODO move the fading in to the AudioLink? Or is it a workaround for a deficiency of the external audio system?

            AudioSample result = new AudioSample();
            _requestId++;
            if (_audioSource != null)
                result = _audioSource(_requestId);

            // final hard limit
            //  TODO: is this needed? The driver probably takes care of it
            //if (result.L > 1 || result.R > 1)
            //    result = new AudioSample(Math.Max(result.L, 1), Math.Max(result.R, 1));

            return result;
        }

        //private void AddToDictionary(Dictionary<MagicColors, string[]> dict, params MagicColor[] colors)
        //{
        //    MagicColor key = MagColors[0];

        //    foreach (MagicColor color in colors.Skip(1))
        //    {
        //        key = key | color;
        //    }
        //    string sortedColors = colors.Select(c => c.ToString.ToLower().ToLower()).ToArray();

        //    dict[key] = sortedColors;
        //}
    }
}