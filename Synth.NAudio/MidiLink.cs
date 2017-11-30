using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NAudio.Midi;
using Synth.MIDI;
using Synth.PerformanceModel;
using Synth.Util;

namespace Synth.NAudio
{
    public class MidiLink : NotifierBase, IMidiLink
    {
        #region static members

        private static readonly IReadOnlyCollection<MidiChannel> _channels = Enum.GetValues(typeof(MidiChannel)).Cast<MidiChannel>().ToList();

        public static IReadOnlyCollection<MidiChannel> Channels
        {
            get { return _channels; }
        }

        private static readonly Lazy<MidiLink> _instance = new Lazy<MidiLink>(() => new MidiLink());

        public static MidiLink Instance
        {
            get { return _instance.Value; }
        }

        #endregion static members

        private const double MIDI_DEVICE_SCAN_INTERVAL_SECONDS = 2; // TODO: is this a good interval?

        private readonly List<WeakReference<IMidiListener>> _listeners = new List<WeakReference<IMidiListener>>();
        private readonly ObservableCollectionExtended<MidiIn> _sources = new ObservableCollectionExtended<MidiIn>();
        private readonly ConcurrentDictionary<MidiChannel, MidiPerformance> _localPerformanceCache = new ConcurrentDictionary<MidiChannel, MidiPerformance>();

        private MidiLink()
        {
            LoadSources();
        }

        #region IMidiLink

        public void AttachListener(IMidiListener listener)
        {
            if (listener != null)
                _listeners.Add(new WeakReference<IMidiListener>(listener));
        }

        public void DetachListener(IMidiListener listener)
        {
            foreach (WeakReference<IMidiListener> wr in _listeners.ToArray())
            {
                IMidiListener candidate;
                if (!wr.TryGetTarget(out candidate))
                    _listeners.Remove(wr);
                else if (candidate == listener)
                    _listeners.Remove(wr);
            }
        }

        public void RepublishState(IMidiListener toListener = null)
        {
            // This does not include active notes

            // Messages that came in on the OMNI channel have already been distributed to each cache
            // Messages that came in on individual channels have all been distributed to the omni cache as well.

            IEnumerable<IMidiListener> listeners;
            if (toListener == null)
                listeners = GetListeners();
            else
                listeners = new[] {toListener};

            foreach (IMidiListener listener in listeners)
            {
                MidiPerformance cache;
                if (_localPerformanceCache.TryGetValue(listener.Channel, out cache))
                {
                    listener.HandleChannelPressure((int)cache.Pressure, cache.Channel);
                    listener.HandlePitchWheel((int)cache.PitchBend, cache.Channel);
                    listener.HandleControlChange((int)MidiController.Modulation, (int)cache.Modulation, cache.Channel);

                    listener.HandleControlChange((int)MidiController.Modulation, (int)cache.Modulation, cache.Channel);
                    listener.HandleControlChange((int)MidiController.BreathController, (int)cache.BreathController, cache.Channel);
                    listener.HandleControlChange((int)MidiController.MainVolume, (int)cache.Volume, cache.Channel);
                    listener.HandleControlChange((int)MidiController.Pan, (int)cache.Pan, cache.Channel);
                    listener.HandleControlChange((int)MidiController.Expression, (int)cache.Expression, cache.Channel);

                    listener.HandleControlChange((int)MidiController.Sustain, cache.HoldPedal ? 127 : 0, cache.Channel);
                    listener.HandleControlChange((int)MidiController.Portamento, cache.Portamento ? 127 : 0, cache.Channel);
                    listener.HandleControlChange((int)MidiController.Sostenuto, cache.SustenutoPedal ? 127 : 0, cache.Channel);
                    listener.HandleControlChange((int)MidiController.SoftPedal, cache.SoftPedal ? 127 : 0, cache.Channel);
                    listener.HandleControlChange((int)MidiController.LegatoFootswitch, cache.Legato ? 127 : 0, cache.Channel);
                }
            }
        }

        public void InjectNoteOn(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni)
        {
            DelegateToListeners(l => l.HandleNoteOn(noteNumber, velocity, channel), channel);
        }

        public void InjectNotePressure(int noteNumber, int value, MidiChannel channel = MidiChannel.Omni)
        {
            DelegateToListeners(l => l.HandleNotePressure(noteNumber, value, channel), channel);
        }

        public void InjectNoteOff(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni)
        {
            DelegateToListeners(l => l.HandleNoteOff(noteNumber, velocity, channel), channel);
        }

        public void InjectPitchWheel(int value, MidiChannel channel = MidiChannel.Omni)
        {
            DelegateToListeners(l => l.HandlePitchWheel(value, channel), channel);
        }

        public void InjectChannelPressure(int value, MidiChannel channel = MidiChannel.Omni)
        {
            DelegateToListeners(l => l.HandleChannelPressure(value, channel), channel);
        }

        public void InjectControlChange(int controllerNumber, int value, MidiChannel channel = MidiChannel.Omni)
        {
            DelegateToListeners(l => l.HandleControlChange(controllerNumber, value, channel), channel);
        }

        public void InjectAllNotesOff(MidiChannel channel = MidiChannel.Omni)
        {
            DelegateToListeners(l => l.HandleAllNotesOff(channel), channel);
        }

        public void InjectResetControllers(MidiChannel channel = MidiChannel.Omni)
        {
            DelegateToListeners(l => l.HandleResetControllers(channel), channel);
        }

        #endregion IMidiLink

        private IEnumerable<IMidiListener> GetListeners()
        {
            List<IMidiListener> listeners = new List<IMidiListener>();

            foreach (WeakReference<IMidiListener> wr in _listeners.ToArray())
            {
                IMidiListener listener;
                if (wr.TryGetTarget(out listener))
                    listeners.Add(listener);
                else
                    _listeners.Remove(wr);
            }

            return listeners;
        }

        private void ClearSources()
        {
            foreach (MidiIn source in _sources)
            {
                try
                {
                    source.Stop();
                }
                catch (Exception ex)
                {
                    // do nothing. This will happen if a midi cable is disconnected while the app is running.
                }
                source.MessageReceived -= HandleMidiMessageReceived;
                source.ErrorReceived -= HandleMidiError;
                source.Dispose();
            }
            _sources.Clear();
        }

        private void LoadSources()
        {
            int previousSourceCount = _sources.Count;

            ClearSources();

            // TODO: if a midi message comes in between clearing and reloading, it could theoretically be lost. In practice, there doesn't seem to be enough time for that to happen.

            Enumerable.Range(0, MidiIn.NumberOfDevices).Execute(i => _sources.Add(new MidiIn(i)));
            foreach (MidiIn source in _sources)
            {
                source.MessageReceived += HandleMidiMessageReceived;
                source.ErrorReceived += HandleMidiError;
                source.Start();
            }

            //if (_sources.Count == 0 && previousSourceCount > 0)
            //    DelegateToListeners(listener => listener.HandleAllNotesOff(MidiChannel.Omni), MidiChannel.Omni);
        }

        private void HandleMidiError(object sender, MidiInMessageEventArgs e)
        {
            Debug.WriteLine("MIDI error: " + e.MidiEvent.GetType().Name);
        }

        private void DelegateToListeners(Action<IMidiListener> action, MidiChannel channel)
        {
            IEnumerable<IMidiListener> listeners = GetListeners().Where(l => channel == MidiChannel.Omni || l.Channel == MidiChannel.Omni || l.Channel == channel);
            listeners.Execute(action);

            MidiPerformance localCache = _localPerformanceCache.GetOrAdd(channel, c => new MidiPerformance(this){Channel = c});
            action(localCache);
            if (channel != MidiChannel.Omni)
            {
                // everything that comes in should end up in the omni cache
                MidiPerformance omniCache = _localPerformanceCache.GetOrAdd(MidiChannel.Omni, c => new MidiPerformance(this) {Channel = c});
                action(omniCache);
            }
        }

        private void HandleMidiMessageReceived(object sender, MidiInMessageEventArgs e)
        {
            MidiIn source = sender as MidiIn;
            if (source == null)
                return;

            if (e.MidiEvent == null)
                return;

            MidiChannel channel = (MidiChannel) e.MidiEvent.Channel;

            MidiCommandCode commandCode = e.MidiEvent.CommandCode;
            if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn && (e.MidiEvent as NoteEvent).Velocity == 0)
                commandCode = MidiCommandCode.NoteOff;

            if (commandCode == MidiCommandCode.NoteOff)
            {
                int noteNumber = (e.MidiEvent as NoteEvent).NoteNumber;
                int velocity = (e.MidiEvent as NoteEvent).Velocity;
                DelegateToListeners(listener => listener.HandleNoteOff(noteNumber, velocity, channel), channel);
            }
            else if (commandCode == MidiCommandCode.NoteOn)
            {
                int noteNumber = (e.MidiEvent as NoteEvent).NoteNumber;
                int velocity = (e.MidiEvent as NoteEvent).Velocity;
                DelegateToListeners(listener => listener.HandleNoteOn(noteNumber, velocity, channel), channel);
            }
            else if (commandCode == MidiCommandCode.KeyAfterTouch)
            {
                int noteNumber = (e.MidiEvent as NoteEvent).NoteNumber;
                int velocity = (e.MidiEvent as NoteEvent).Velocity;
                DelegateToListeners(listener => listener.HandleNotePressure(noteNumber, velocity, channel), channel);
            }
            else if (commandCode == MidiCommandCode.ControlChange)
            {
                MidiController controller = (e.MidiEvent as ControlChangeEvent).Controller;
                if (controller == MidiController.AllNotesOff)
                {
                    DelegateToListeners(listener => listener.HandleAllNotesOff(channel), channel);
                }
                else if (controller == MidiController.ResetAllControllers)
                {
                    DelegateToListeners(listener => listener.HandleResetControllers(channel), channel);
                }
                else
                {
                    int controllerNumber = (int) controller;
                    int value = (e.MidiEvent as ControlChangeEvent).ControllerValue;
                    DelegateToListeners(listener => listener.HandleControlChange(controllerNumber, value, channel), channel);
                }
            }
            else if (commandCode == MidiCommandCode.ChannelAfterTouch)
            {
                int value = (e.MidiEvent as ChannelAfterTouchEvent).AfterTouchPressure;
                DelegateToListeners(listener => listener.HandleChannelPressure(value, channel), channel);
            }
            else if (commandCode == MidiCommandCode.PitchWheelChange)
            {
                int value = (e.MidiEvent as PitchWheelChangeEvent).Pitch;
                DelegateToListeners(listener => listener.HandlePitchWheel(value, channel), channel);
            }
        }

        #region cleanup

        public void Dispose()
        {
            try
            {
                ClearSources();

                foreach (IMidiListener listener in GetListeners())
                {
                    listener.HandleAllNotesOff();
                    listener.HandleResetControllers();
                }
            }
            catch (Exception ex)
            {
                // just swallow any errors -- we're disposing!
            }

            _listeners.Clear();
        }

        // Finalizer
        ~MidiLink()
        {
            Dispose();
        }

        #endregion cleanup
    }
}
