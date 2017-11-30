using System;
using System.Collections.Generic;
using System.Linq;

using Synth.Core;
using Synth.Util;

namespace Synth.Audio
{
    public class AudioLink : NotifierBase, IAudioLink
    {
        #region singleton

        private static readonly Lazy<AudioLink> _instance = new Lazy<AudioLink>(() => new AudioLink());
        public static IAudioLink Instance
        {
            get { return _instance.Value; }
        }

        #endregion singleton

        private IFinalOutput _finalOutput;
        private readonly List<IAudioSource> _sources = new List<IAudioSource>();
        private readonly List<IClockListener> _clockListeners = new List<IClockListener>();

        private AudioLink()
        {
        }

        #region IAudioLink

        public event EventHandler SampleRateChanged;

        public void Initialize(IFinalOutput output)
        {
            Deactivate();

            if (_finalOutput != output)
            {
                if (_finalOutput != null)
                    _finalOutput.Dispose();

                _finalOutput = output;
            }

            if (_finalOutput != null)
            {
                OnQualityChanged();
                _finalOutput.SetAudioSource(GetCombinedSample);
            }
        }

        public void Activate()
        {
            if (IsActive)
                return;

            IsActive = true;
            _finalOutput.Start();
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            IsActive = false;

            if (_finalOutput != null)
                _finalOutput.Stop();
        }

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set { SetField(ref _isActive, value); }
        }

        public void AttachClockListener(IClockListener item)
        {
            if (item != null)
                _clockListeners.Add(item);
        }

        public void AttachSource(IAudioSource item)
        {
            if (item != null)
                _sources.Add(item);
        }

        public void DetachSource(IAudioSource item)
        {
            if (item != null)
                _sources.Remove(item);
        }

        public void DetachClockListener(IClockListener item)
        {
            if (item != null)
                _clockListeners.Remove(item);
        }

        #endregion IAudioLink

        public int SampleRate
        {
            get
            {
                if (_finalOutput == null)
                    return 0;
                else
                    return _finalOutput.SamplesPerSecond;
            }
        }

        private void OnQualityChanged()
        {
            EventHandler evt = SampleRateChanged;
            if (evt != null)
                evt.Invoke(this, new EventArgs());
        }

        private AudioSample GetCombinedSample(ulong requestId)
        {
            AudioSample result = new AudioSample();
            _clockListeners.Execute(listener => listener.HandleClockTick(requestId));
            _sources.Where(s => s != null).ToArray().Execute(source => result += source.GetSample(requestId));
            return result;
        }

        #region cleanup

        public void Dispose()
        {
            Deactivate();
        }

        ~AudioLink()
        {
            Dispose();
            _finalOutput = null;
        }

        #endregion cleanup
    }
}
