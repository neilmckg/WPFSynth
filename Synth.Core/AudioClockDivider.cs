using System;

using Synth.Audio;

namespace Synth.Core
{
    public class AudioClockDivider : IClockListener, IDisposable
    {
        private readonly int _targetRate;
        private readonly bool _rateMustBeExact;
        private readonly Action<ulong> _tickHandler;

        private IAudioLink _audioLink;
        private int _tickInterval;
        private int _currentTick = 0;

        public AudioClockDivider(IAudioLink audioLink, int targetClockRate, Action<ulong> tickHandler, bool rateMustBeExact = false)
        {
            if (audioLink == null)
                throw new ArgumentNullException("audioLink");
            _audioLink = audioLink;

            if (tickHandler == null)
                throw new ArgumentNullException("tickHandler");
            _tickHandler = tickHandler;

            _rateMustBeExact = rateMustBeExact;
            _targetRate = targetClockRate;

            ApplyTargetRate();
            _audioLink.SampleRateChanged += HandleAudioQualityChanged;
            _audioLink.AttachClockListener(this);
        }

        private void HandleAudioQualityChanged(object sender, EventArgs e)
        {
            ApplyTargetRate();
        }

        private void ApplyTargetRate()
        {
            if (_audioLink.SampleRate == 0)
                return;

            double idealTickInterval = _audioLink.SampleRate/(double) _targetRate;
            double roundedTickInterval = Math.Round(idealTickInterval, 0);
            if (_rateMustBeExact && idealTickInterval != roundedTickInterval)
                throw new InvalidOperationException("The desired clock rate cannot be divided evenly into the audio sample rate.");

            TicksPerSecond = Convert.ToInt32(_audioLink.SampleRate/roundedTickInterval);
            _tickInterval = Convert.ToInt32(roundedTickInterval);
        }

        public void HandleClockTick(ulong requestId)
        {
            _currentTick++;
            if (_currentTick >= _tickInterval)
            {
                _currentTick = 0;
                _tickHandler(requestId);
            }
        }

        public int TicksPerSecond { get; private set; }

        public void Dispose()
        {
            if (_audioLink != null)
                _audioLink.DetachClockListener(this);
            _audioLink = null;
        }

        ~AudioClockDivider()
        {
            Dispose();
        }
    }
}
