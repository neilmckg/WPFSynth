using System;

using Synth.MIDI;
using Synth.Util;

namespace Synth.PerformanceModel
{
    internal class MidiNote : NotifierBase, IMidiNote
    {
        public MidiNote(MidiChannel channel, int noteNumber, float velocity)
        {
            StartTime = DateTime.Now;

            Channel = channel;
            Number = noteNumber;
            Velocity = velocity;
        }

        public int Number { get; set; }

        private float _velocity;
        public float Velocity
        {
            get { return _velocity; }
            set { SetField(ref _velocity, value); }
        }

        private float _pressure;
        public float Pressure 
        {
            get { return _pressure;}
            set { SetField(ref _pressure, value); }
        }

        public MidiChannel Channel { get; private set; }

        public DateTime StartTime { get; private set; }

        private bool _isReleased = false;
        public bool IsReleased
        {
            get { return _isReleased; }
            set { SetField(ref _isReleased, value); }
        }

    }
}
