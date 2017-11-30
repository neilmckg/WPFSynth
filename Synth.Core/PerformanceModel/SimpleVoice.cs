using System;
using Synth.Util;

namespace Synth.PerformanceModel
{
    internal class SimpleVoice : NotifierBase, ISimpleVoice
    {
        public SimpleVoice() 
        {
        }

        public IMidiNote FromMidiNote { get; set; }

        private float _pitch = 0;
        public float Pitch
        {
            get { return _pitch; }
            set { SetField(ref _pitch, value); }
        }

        private float _intensity = 0;
        public float Intensity
        {
            get { return _intensity; }
            set { SetField(ref _intensity, value); }
        }

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (SetField(ref _isActive, value))
                {
                    if (_isActive)
                        StartTime = DateTime.Now;
                    else
                        Intensity = 0;
                }
            }
        }

        private DateTime _startTime = DateTime.MinValue;
        public DateTime StartTime
        {
            get { return _startTime; }
            private set { SetField(ref _startTime, value); }
        }

        public Guid? Id { get; set; }
    }
}
