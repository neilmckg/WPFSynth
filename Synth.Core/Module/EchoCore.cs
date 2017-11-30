using System;
using System.Linq;

using Synth.Audio;
using Synth.Core;

namespace Synth.Module
{
    public class EchoCore : ModuleCoreWithAudioOutput
    {
        private readonly IAudioLink _audioLink;
        private AudioWire _input;
        private int _delaySamples;
        private AudioSample[] _buffer = new AudioSample[0];
        private int _nextInputIndex = 0;

        public EchoCore(IAudioLink audioLink, float initialSeconds = 0.4f)
        {
            if (audioLink == null)
                throw new ArgumentNullException("audioLink");
            _audioLink = audioLink;

            InitializeInputs(initialSeconds);

            HandleQualityChanged(null, null);
            _audioLink.SampleRateChanged += HandleQualityChanged;
        }

        private void HandleQualityChanged(object sender, EventArgs e)
        {
            UpdateDelaySamples();
        }

        public FloatInput Level { get; private set; }
        public FloatInput Seconds { get; private set; }
        public FloatInput Feedback { get; private set; }

        public void SetInput(AudioWire input)
        {
            _input = input;
        }

        private void UpdateDelaySamples()
        {
            _delaySamples = (int)(_audioLink.SampleRate * Seconds.Value);
            EnsureBufferSize();
        }

        private readonly object _bufferLock = new object();

        protected override AudioSample CalculateNextSample(ulong requestId)
        {
            AudioSample tapValue;
            AudioSample sourceSample = new AudioSample();
            if (_input != null)
                sourceSample = _input(requestId);

            lock (_bufferLock)
            {
                int outputIndex = _nextInputIndex + _delaySamples;
                if (outputIndex >= _buffer.Length)
                    outputIndex -= _buffer.Length;

                tapValue = _buffer[outputIndex];

                if (Feedback.Value > 0)
                    sourceSample += (tapValue*Feedback.Value);

                _buffer[_nextInputIndex] = sourceSample;

                _nextInputIndex++;
                if (_nextInputIndex >= _buffer.Length)
                    _nextInputIndex = 0;
            }

            tapValue *= Level.Value;

            return tapValue;
        }

        private void EnsureBufferSize()
        {
            // TODO throw out buffered data if the buffer is too large.
            // The way it is now, the buffer gets bigger but never shrinks back down.

            lock (_bufferLock)
            {
                if (_buffer.Length < _delaySamples)
                {
                    AudioSample[] oldBuffer = _buffer;
                    AudioSample[] newBuffer = new AudioSample[_delaySamples];

                    if (oldBuffer.Any(s => s.L != 0 || s.R != 0))
                    {
                        // segment 1: from the nextInputIndex to the end of the current buffer (which is always shorter than the current buffer)
                        int segment1Length = oldBuffer.Length - _nextInputIndex;
                        Array.Copy(oldBuffer, _nextInputIndex, newBuffer, _nextInputIndex, segment1Length);

                        // segment 2: take from the beginning of the old buffer and put as much as will fit at the end of the new buffer
                        int segment2Length = newBuffer.Length - oldBuffer.Length;
                        Array.Copy(oldBuffer, 0, newBuffer, oldBuffer.Length, segment2Length);

                        // segment 3: put the remainder of the old buffer at the beginning of the new buffer
                        int segment3Length = newBuffer.Length - (segment1Length + segment2Length);
                        Array.Copy(oldBuffer, segment2Length, newBuffer, 0, segment3Length);

                        // this leaves a block of zeros in the array elements representing the oldest samples, which weren't collected prior to the resize.
                    }

                    _buffer = newBuffer;
                }
            }
        }

        private void InitializeInputs(float initialSeconds)
        {
            Level = new FloatInput("Level", 1, 0, 1, ValueOutOfRangeStrategy.ForceIntoRange);
            Seconds = new FloatInput("Seconds", initialSeconds, 0.1f, 5f, ValueOutOfRangeStrategy.ForceIntoRange, (name, v0, v1) => UpdateDelaySamples());
            Feedback = new FloatInput("Feedback", 0, 0, 1, ValueOutOfRangeStrategy.ForceIntoRange);
        }
    }
}
