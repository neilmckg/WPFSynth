using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using Synth.Core;
using Synth.MIDI;
using Synth.Util;

namespace Synth.PerformanceModel
{
    public class SimplePerformance : NotifierBase, ISimplePerformance
    {
        private readonly object _voiceLock = new object();
        private readonly IMidiPerformance _midiSource;
        private readonly IScale _scale;
        private IEnumerable<IMidiNote> _lastNoteList = new IMidiNote[0];

        public SimplePerformance(IMidiPerformance midiSource, IScale scale, int initialNumberOfVoices)
        {
            if (midiSource == null)
                throw new ArgumentNullException("midiSource");

            if (scale == null)
                throw new ArgumentNullException("scale");

            if (midiSource.ValueStrategy != MidiValueStrategy.Normalized)
                throw new ArgumentException("The MIDI Performance source must have been initialized with a Normalized value strategy.");

            _scale = scale;

            NumberOfVoices = initialNumberOfVoices;

            _midiSource = midiSource;
            Channel = _midiSource.Channel;
            _midiSource.PropertyChanged += HandleMidiSourcePropertyChanged;
            _midiSource.ActiveNotes.CollectionChanged += HandleMidiNotesChanged;
        }

        #region ISimplePerformanceSource
        
        private MidiChannel _channel = MidiChannel.Omni;
        public MidiChannel Channel
        {
            get { return _channel; }
            set
            {
                if (SetField(ref _channel, value) && value != MidiChannel.Omni)
                {
                    Reset();
                    _midiSource.Channel = value;
                    // TODO: reapply _midiSource state 
                }
            }
        }

        private float _pitchBendRange = 0;
        public float PitchBendRange
        {
            get { return _pitchBendRange; }
            set
            {
                float oldRange = _pitchBendRange;

                if (SetField(ref _pitchBendRange, value))
                    _voices.Execute(SetPitch);
            }
        }

        private int _numberOfVoices = 0;
        public int NumberOfVoices
        {
            get { return _numberOfVoices; } 
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("NumberOfVoices", "There must be at least one voice.");

                if (SetField(ref _numberOfVoices, value))
                {
                    lock (_voiceLock)
                    {
                        RemoveVoices();
                        AddVoices();
                    }
                }
            }
        }

        private SimpleVoice GetLowestPriorityVoice()
        {
            SimpleVoice lowestPriority = _voices.OrderBy(v => !v.IsActive).ThenBy(v => v.StartTime).FirstOrDefault();
            return lowestPriority;
        }

        private readonly ObservableCollection<SimpleVoice> _voices = new ObservableCollection<SimpleVoice>();
        public IReadOnlyCollection<ISimpleVoice> Voices
        {
            get { return _voices; }
        }

        private bool _isLegato = false;
        public bool IsLegato
        {
            get { return _isLegato; }
            set { SetField(ref _isLegato, value); }
        }

        private bool _switch = false;
        public bool Switch
        {
            get { return _switch; }
            private set { SetField(ref _switch, value); }
        }

        private float _modulationAmount = 0;
        public float ModulationAmount
        {
            get { return _modulationAmount; }
            private set { SetField(ref _modulationAmount, value); }
        }

        private ExpressionSources _intensitySource = ExpressionSources.ChannelPressure;
        public ExpressionSources IntensitySource
        {
            get { return _intensitySource; }
            set { SetField(ref _intensitySource, value); }
        }

        private SwitchSources _switchSource = SwitchSources.LegatoSwitch;
        public SwitchSources SwitchSource
        {
            get { return _switchSource; }
            set { SetField(ref _switchSource, value); }
        }

        private bool _applyHoldPedalToSustain = true;
        public bool ApplyHoldPedalToSustain
        {
            get { return _applyHoldPedalToSustain; }
            set
            {
                if (SetField(ref _applyHoldPedalToSustain, value) && !_applyHoldPedalToSustain)
                    KillSustainedNotes();
            }
        }

        public Guid? InjectNote(float pitch, float intensity)
        {
            SimpleVoice voice = GetLowestPriorityVoice();
            voice.IsActive = false;
            voice.FromMidiNote = null;
            voice.Pitch = pitch;
            voice.Intensity = intensity;
            voice.Id = Guid.NewGuid();

            return voice.Id;
        }

        public void ReleaseNote(Guid id)
        {
            lock (_voiceLock)
            {

                foreach (SimpleVoice voice in _voices.Where(v => v.Id == id).ToArray())
                {
                    voice.IsActive = false;
                    voice.Id = null;
                }
            }
        }

        public void UpdateNote(Guid id, float pitch, float intensity)
        {
            if (intensity == 0)
            {
                ReleaseNote(id);
            }
            else
            {
                lock (_voiceLock)
                {
                    foreach (SimpleVoice voice in _voices.Where(v => v.Id == id))
                    {
                        voice.Pitch = pitch;
                        voice.Intensity = intensity;
                    }
                }
            }
        }

        public void InjectSwitch(bool state)
        {
            Switch = state;
        }

        public void InjectModulation(float amount)
        {
            ModulationAmount = amount;
        }

        #endregion ISimplePerformanceSource

        private void HandleMidiNotesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IMidiNote[] itemsToRemove;
            IMidiNote[] itemsToAdd;

            lock (_voiceLock)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    // itemsToRemove is everything that used to be in the list but is not in the list now
                    itemsToRemove =
                        _lastNoteList.Where(on => !FindMatchingVoices(on).Any()).ToArray();
                    // itemsToAdd is evertying that is in the list now that wasn't the list before
                    itemsToAdd = _midiSource.ActiveNotes.Where( on => !_lastNoteList.Any(an => an.Number == on.Number)).ToArray();
                }
                else
                {
                    itemsToRemove = (e.OldItems ?? new IMidiNote[0]).OfType<IMidiNote>().ToArray();
                    itemsToAdd = (e.NewItems ?? new IMidiNote[0]).OfType<IMidiNote>().ToArray();
                }

                foreach (int i in Enumerable.Range(0, itemsToRemove.Length))
                {
                    IMidiNote note = itemsToRemove[i];
                    IEnumerable<SimpleVoice> toKill = FindMatchingVoices(note);
                    toKill.Execute(KillNote);
                }

                foreach (IMidiNote note in itemsToAdd)
                {
                    SimpleVoice voice = GetLowestPriorityVoice();
                    // this could only be null if there are zero voices, which is not allowed
                    if (voice == null)
                        throw new InvalidOperationException("No voices found.");

                    voice.FromMidiNote = note;

                    if (NumberOfVoices > 1 || !IsLegato)
                        voice.Intensity = 0;

                    SetPitch(voice);

                    if (!voice.IsActive || NumberOfVoices > 1 || !IsLegato)
                        voice.Intensity = note.Velocity;

                    voice.IsActive = true;
                }

                _lastNoteList = _midiSource.ActiveNotes;
            }
        }

        private IEnumerable<SimpleVoice> FindMatchingVoices(IMidiNote note)
        {
            SimpleVoice[] voices = _voices.Where(v => v.FromMidiNote != null && v.FromMidiNote.Number == note.Number).ToArray();
            return voices;
        }

        private void KillNote(SimpleVoice voice)
        {
            voice.FromMidiNote.PropertyChanged -= HandleMidiNotePropertyChanged;
            voice.IsActive = false;
        }

        private void HandleMidiNotePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null)
                return;

            IMidiNote sourceNote = sender as IMidiNote;
            if (sourceNote == null)
                throw new ArgumentException("HandleMidiNotePropertyChanged expects an IMidiNote sender, but got a " + sender.GetType());

            if (e.PropertyName == "Pressure")
            {
                lock (_voiceLock)
                {
                    IEnumerable<SimpleVoice> voices = FindMatchingVoices(sourceNote);
                    voices.Where(v => v.IsActive).Execute(v => v.Intensity = v.FromMidiNote.Pressure);
                }
            }
            if (e.PropertyName == "IsReleased" && sourceNote.IsReleased)
            {
                if (!ApplyHoldPedalToSustain || !_midiSource.HoldPedal)
                {
                    lock (_voiceLock)
                    {
                        IEnumerable<SimpleVoice> voices = FindMatchingVoices(sourceNote);
                        _voices.Execute(KillNote);
                    }
                }
            }
        }

        private void SetPitch(SimpleVoice voice)
        {
            if (voice.FromMidiNote != null)
            {
                // TODO this search could be optimized
                IScaleNote scaleNote = _scale.FirstOrDefault(n => n.MidiNoteNumber == voice.FromMidiNote.Number);
                if (scaleNote != null)
                    voice.Pitch = scaleNote.Pitch + (Pitch.HalfStep*PitchBendRange*_midiSource.PitchBend);
            }
        }

        private void HandleMidiSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HoldPedal")
            {
                if (ApplyHoldPedalToSustain)
                    KillSustainedNotes();
                if (SwitchSource.HasFlag(SwitchSources.HoldPedal))
                    Switch = _midiSource.HoldPedal;
            }

            if (e.PropertyName == "PitchBend")
                _voices.Where(v => v.FromMidiNote != null).Execute(SetPitch);
            else if (e.PropertyName == "Modulation")
                ModulationAmount = _midiSource.Modulation;

            if (e.PropertyName == "Portamento" && SwitchSource.HasFlag(SwitchSources.PortamentoSwitch))
                Switch = _midiSource.Portamento;
            else if (e.PropertyName == "SustenutoPedal" && SwitchSource.HasFlag(SwitchSources.SustenutoPedal))
                Switch = _midiSource.SustenutoPedal;
            else if (e.PropertyName == "SoftPedal" && SwitchSource.HasFlag(SwitchSources.SoftPedal))
                Switch = _midiSource.SoftPedal;
            else if (e.PropertyName == "Legato" && SwitchSource.HasFlag(SwitchSources.LegatoSwitch))
                Switch = _midiSource.Legato;

            float? newIntensity = null;

            if (e.PropertyName == "Pressure" && IntensitySource.HasFlag(ExpressionSources.ChannelPressure))
                newIntensity = _midiSource.Pressure;
            else if (e.PropertyName == "BreathController" && IntensitySource.HasFlag(ExpressionSources.BreathController))
                newIntensity = _midiSource.BreathController;
            else if (e.PropertyName == "FootPedal" && IntensitySource.HasFlag(ExpressionSources.FootPedal))
                newIntensity = _midiSource.FootPedal;
            else if (e.PropertyName == "Expression" && IntensitySource.HasFlag(ExpressionSources.Expression))
                newIntensity = _midiSource.Expression;

            if (newIntensity.HasValue)
                _voices.Where(v => v.IsActive && v.FromMidiNote != null).Execute(v => v.Intensity = newIntensity.Value);
        }

        private void KillSustainedNotes()
        {
            lock (_voiceLock)
            {
                foreach (SimpleVoice voice in _voices)
                {
                    if (voice.FromMidiNote != null && voice.FromMidiNote.IsReleased)
                    {
                        voice.FromMidiNote = null;
                        voice.Intensity = 0;
                    }
                }
            }
        }

        private void Reset()
        {
            lock (_voiceLock)
            {
                _voices.Execute(v => v.Intensity = 0);
                _voices.Clear();
                Switch = false;
                ModulationAmount = 0;
                AddVoices();
            }
        }

        private void RemoveVoices()
        {
            lock (_voiceLock)
            {
                while (_voices.Count > _numberOfVoices)
                {
                    SimpleVoice voice = GetLowestPriorityVoice();
                    if (voice != null)
                    {
                        voice.IsActive = false;
                        _voices.Remove(voice);
                    }
                }
            }
        }

        private void AddVoices()
        {
            lock (_voiceLock)
            {
                while (_voices.Count < _numberOfVoices)
                    _voices.Add(new SimpleVoice());
            }
        }
    }
}
