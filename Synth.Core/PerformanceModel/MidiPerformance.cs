using System;
using System.Collections.Generic;
using System.Linq;

using Synth.MIDI;
using Synth.Util;

namespace Synth.PerformanceModel
{
    public class MidiPerformance : NotifierBase, IMidiPerformance, IMidiListener
    {
        #region static members

        private static readonly IEnumerable<int> _booleanControllers = new[]
        {
            (int)MidiControllerType.LegatoFootswitch, 
            (int)MidiControllerType.Portamento,
            (int)MidiControllerType.SoftPedal, 
            (int)MidiControllerType.Sostenuto,
            (int)MidiControllerType.Sustain
        };

        private static readonly List<WeakReference<MidiPerformance>> _instances = new List<WeakReference<MidiPerformance>>(); 

        private static IEnumerable<MidiPerformance> GetCurrentInstances()
        {
            List<MidiPerformance> instances = new List<MidiPerformance>();
            foreach (WeakReference<MidiPerformance> wr in _instances.ToArray())
            {
                MidiPerformance instance;
                if (wr.TryGetTarget(out instance))
                    instances.Add(instance);
                else
                    _instances.Remove(wr);
            }

            return instances;
        }

        #region inject eventsd into all midi performances

        public static void InjectAllNotesOff()
        {
            GetCurrentInstances().Execute(mp => mp.HandleAllNotesOff());
        }

        public static void InjectChannelPressure(int value)
        {
            GetCurrentInstances().Execute(mp => mp.HandleChannelPressure(value));
        }

        public static void InjectControlChange(int controllerNumber, int value)
        {
            GetCurrentInstances().Execute(mp => mp.HandleControlChange(controllerNumber, value));
        }

        public static void InjectNoteOff(int noteNumber, int velocity)
        {
            GetCurrentInstances().Execute(mp => mp.HandleNoteOff(noteNumber, velocity));           
        }

        public static void InjectNoteOn(int noteNumber, int velocity)
        {
            GetCurrentInstances().Execute(mp => mp.HandleNoteOn(noteNumber, velocity));
        }

        public static void InjectNotePressure(int noteNumber, int value)
        {
            GetCurrentInstances().Execute(mp => mp.HandleNotePressure(noteNumber, value));
        }

        public static void InjectPitchWheel(int value)
        {
            GetCurrentInstances().Execute(mp => mp.HandlePitchWheel(value));
        }

        public static void InjectResetControllers()
        {
            GetCurrentInstances().Execute(mp => mp.HandleResetControllers());
        }

        #endregion inject eventsd into all midi performances
        
        #endregion static members

        private const float RAW_PITCHBEND_SCALE = 8192f;

        private readonly object _noteLock = new object();
        private readonly IMidiLink _midiLink;

        public MidiPerformance(IMidiLink midiLink, MidiValueStrategy valueStrategy = MidiValueStrategy.Raw)
        {
            // null is ok
            _midiLink = midiLink;
            
            _instances.Add(new WeakReference<MidiPerformance>(this));
            ValueStrategy = valueStrategy;

            _pitchBend = ValueStrategy == MidiValueStrategy.Normalized ? 0f : RAW_PITCHBEND_SCALE;

            // TODO I think I need to supress this attachment in design mode
            _midiLink.AttachListener(this);
        }

        #region IMidiListener

        public MidiValueStrategy ValueStrategy { get; private set; }

        public void HandleNoteOn(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni)
        {
            // By convention, a note on with a zero velocity should be interpreted as a note off
            if (velocity == 0)
            {
                HandleNoteOff(noteNumber, velocity, channel);
                return;
            }

            if (!IsApplicable(channel))
                return;

            float velocityValue = velocity;
            if (ValueStrategy == MidiValueStrategy.Normalized)
                velocityValue /= 127f;

            lock (_noteLock)
            {
                IEnumerable<MidiNote> notes = _activeNotes.Where(n => n.Number == noteNumber && n.Channel == channel).ToArray();
                notes.Execute(n => _activeNotes.Remove(n));

                _activeNotes.Add(new MidiNote(channel, noteNumber, velocityValue));
            }
        }

        public void HandleNotePressure(int noteNumber, int value, MidiChannel channel = MidiChannel.Omni)
        {
            if (!IsApplicable(channel))
                return;

            lock (_noteLock)
            {
                float newValue = value;
                if (ValueStrategy == MidiValueStrategy.Normalized)
                    newValue /= 127f;

                IEnumerable<MidiNote> notes = _activeNotes.Where(n => n.Number == noteNumber && n.Channel == channel).ToArray();
                notes.Execute(n => n.Pressure = newValue);
            }
        }

        public void HandleNoteOff(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni)
        {
            if (!IsApplicable(channel))
                return;

            float velocityValue = velocity;
            if (ValueStrategy == MidiValueStrategy.Normalized)
                velocityValue /= 127f;

            lock (_noteLock)
            {
                IEnumerable<MidiNote> notes = _activeNotes.Where(n => n.Number == noteNumber && n.Channel == channel).ToArray();
                foreach (MidiNote note in notes)
                {
                    note.Velocity = velocityValue;
                    note.IsReleased = true;
                    if (!HoldPedal)
                        _activeNotes.Remove(note);
                }
            }
        }

        public void HandlePitchWheel(int value, MidiChannel channel = MidiChannel.Omni)
        {
            if (!IsApplicable(channel))
                return;

            // per nAudio: 0 is minimum, 0x2000 (8192) is default, 0x4000 (16384) is maximum
            if (ValueStrategy == MidiValueStrategy.Normalized)
                PitchBend = (value / RAW_PITCHBEND_SCALE) - 1;
            else
                PitchBend = value;
        }

        public void HandleChannelPressure(int value, MidiChannel channel = MidiChannel.Omni)
        {
            if (!IsApplicable(channel))
                return;

            if (ValueStrategy == MidiValueStrategy.Normalized)
                Pressure = value/127f;
            else
                Pressure = value;
        }

        public void HandleControlChange(int controllerNumber, int value, MidiChannel channel = MidiChannel.Omni)
        {
            if (!IsApplicable(channel))
                return;

            float valueOut = value;
            if (_booleanControllers.Contains(controllerNumber))
                valueOut = (float)Math.Round(valueOut/127f, 0);
            else if (ValueStrategy == MidiValueStrategy.Normalized)
                valueOut /= 127f;

            if (controllerNumber == (int)MidiControllerType.BreathController)
                BreathController = valueOut;
            else if (controllerNumber == (int)MidiControllerType.Expression)
                Expression = valueOut;
            else if (controllerNumber == (int)MidiControllerType.FootController)
                FootPedal = valueOut;
            else if (controllerNumber == (int)MidiControllerType.MainVolume)
                Volume = valueOut;
            else if (controllerNumber == (int)MidiControllerType.Modulation)
                Modulation = valueOut;
            else if (controllerNumber == (int)MidiControllerType.Pan)
                Pan = valueOut;
            else if (controllerNumber == (int)MidiControllerType.LegatoFootswitch)
                Legato = (valueOut > 0);
            else if (controllerNumber == (int)MidiControllerType.Portamento)
                Portamento = (valueOut > 0);
            else if (controllerNumber == (int)MidiControllerType.SoftPedal)
                SoftPedal = (valueOut > 0);
            else if (controllerNumber == (int)MidiControllerType.Sostenuto)
                SustenutoPedal = (valueOut > 0);
            else if (controllerNumber == (int)MidiControllerType.Sustain)
                HoldPedal = (valueOut > 0);
        }

        public void HandleAllNotesOff(MidiChannel channel = MidiChannel.Omni)
        {
            if (!IsApplicable(channel))
                return;

            bool initHold = HoldPedal;
            HoldPedal = false;
            _activeNotes.ToArray().Execute(n => HandleNoteOff(n.Number, 0, n.Channel));
            HoldPedal = initHold;
        }

        public void HandleResetControllers(MidiChannel channel = MidiChannel.Omni)
        {
            if (!IsApplicable(channel))
                return;

            Pressure = 0;
            PitchBend = 0;
            Modulation = 0;
            BreathController = 0;
            FootPedal = 0;
            Volume = 0;
            Pan = 0;
            Expression = 0;
            HoldPedal = false;
            Portamento = false;
            SustenutoPedal = false;
            SoftPedal = false;
            Legato = false;
        }

        #endregion IMidiListener

        #region IMidiPerformanceSource

        private MidiChannel _channel = MidiChannel.Omni;

        public MidiChannel Channel
        {
            get { return _channel; }
            set
            {
                if (SetField(ref _channel, value))
                {
                    if (value != MidiChannel.Omni)
                    {
                        HandleAllNotesOff();
                    }
                    if (_midiLink != null)
                        _midiLink.RepublishState(this);
                }
            }
        }

        private readonly ObservableCollectionExtended<MidiNote> _activeNotes = new ObservableCollectionExtended<MidiNote>();
        public IReadOnlyObservableCollection<IMidiNote> ActiveNotes
        {
            get { return _activeNotes; }
        }

        private float _channelPressure = 0;
        public float Pressure
        {
            get { return _channelPressure; }
            private set { SetField(ref _channelPressure, value); }
        }

        private float _pitchBend = 0;
        public float PitchBend
        {
            get { return _pitchBend; }
            private set { SetField(ref _pitchBend, value); }
        }

        private float _modulation = 0;
        public float Modulation
        {
            get { return _modulation; }
            private set { SetField(ref _modulation, value); }
        }

        private float _breathController = 0;
        public float BreathController
        {
            get { return _breathController; }
            private set { SetField(ref _breathController, value); }
        }

        private float _footPedal = 0;
        public float FootPedal
        {
            get { return _footPedal; }
            private set { SetField(ref _footPedal, value); }
        }

        private float _volume = 0;
        public float Volume
        {
            get { return _volume; }
            private set { SetField(ref _volume, value); }
        }

        private float _pan = 0;
        public float Pan
        {
            get { return _pan; }
            private set { SetField(ref _pan, value); }
        }

        private float _expression = 0;
        public float Expression
        {
            get { return _expression; }
            private set { SetField(ref _expression, value); }
        }

        private bool _holdPedal = false;
        public bool HoldPedal
        {
            get { return _holdPedal; }
            private set
            {
                if (SetField(ref _holdPedal, value) & !_holdPedal)
                {
                    lock (_noteLock)
                    {
                        foreach (MidiNote note in _activeNotes.Where(n => n.IsReleased).ToArray())
                            _activeNotes.Remove(note);
                    }
                }
            } 
        }

        private bool _portamento = false;
        public bool Portamento
        {
            get { return _portamento; }
            private set { SetField(ref _portamento, value); }
        }

        private bool _sostenuto = false;
        public bool SustenutoPedal
        {
            get { return _sostenuto; }
            private set { SetField(ref _sostenuto, value); }
        }

        private bool _softPedal = false;
        public bool SoftPedal
        {
            get { return _softPedal; }
            private set { SetField(ref _softPedal, value); }
        }

        private bool _legato = false;
        public bool Legato
        {
            get { return _legato; }
            private set { SetField(ref _legato, value); }
        }

        #endregion IMidiPerformanceSource

        private bool IsApplicable(MidiChannel channel)
        {
            if (Channel == MidiChannel.Omni)
                return true;
            else if (channel == MidiChannel.Omni)
                return true;
            else if (Channel == channel)
                return true;
            else
                return false;
        }
    }
}
