using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

using Synth.Core;
using Synth.NAudio;
using Synth.MIDI;
using Synth.PerformanceModel;
using Synth.Util;
using Synth.WPF.Util;

namespace Synth.WPF.Controls
{
    /// <summary>
    /// Interaction logic for Keyboard.xaml
    /// </summary>
    public partial class Keyboard : UserControl, IMidiListener
    {
        private const int MIDI_CONTROLLERNUMBER_SUSTAIN = 64;
        private const double KEY_WIDTH = 40;

        #region dependency properties

        #region BottomNote

        public static readonly DependencyProperty BottomNoteProperty = DependencyProperty.Register("BottomNote",
                                                                                                    typeof(IScaleNote),
                                                                                                    typeof(Keyboard),
                                                                                                    new PropertyMetadata(Scale.EqualTemperedScale["C3"], HandleBottomNoteChanged));

        private static void HandleBottomNoteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO: This is disabled until a bug in LoadNotes() is resolved
            //Keyboard keyboard = d as Keyboard;
            //keyboard.LoadNotes(keyboard.Notes.Count);
        }

        [Bindable(true)]
        public IScaleNote BottomNote
        {
            get { return (IScaleNote)GetValue(BottomNoteProperty); }
            set { SetValue(BottomNoteProperty, value); }
        }

        #endregion BottomNote

        #region IsSustained

        public static readonly DependencyProperty IsSustainedProperty = DependencyProperty.Register("IsSustained",
                                                                                                    typeof(bool),
                                                                                                    typeof(Keyboard),
                                                                                                    new PropertyMetadata(false, HandleIsSustainedChanged));
        private static void HandleIsSustainedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Keyboard)._midiLink.InjectControlChange(MIDI_CONTROLLERNUMBER_SUSTAIN, (bool)e.NewValue ? 127 : 0);
            if ((bool)e.NewValue == false)
                (d as Keyboard).StopAllNotes();
        }

        public bool IsSustained
        {
            get { return (bool)GetValue(IsSustainedProperty); }
            set { SetValue(IsSustainedProperty, value); }
        }

        #endregion IsSustained

        #region Intensity

        private double _velocity = (double) VelocityProperty.DefaultMetadata.DefaultValue;

        public static readonly DependencyProperty VelocityProperty = DependencyProperty.Register("Velocity",
                                                                                                    typeof(double),
                                                                                                    typeof(Keyboard),
                                                                                                    new PropertyMetadata(double.NaN, HandleVelocityChanged));

        private static void HandleVelocityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Keyboard)._velocity = (double) e.NewValue;
        }

        [Bindable(true)]
        public double Velocity
        {
            get { return _velocity; }
            set { SetValue(VelocityProperty, value); }
        }

        #endregion Intensity

        #region ActiveNote

        public static readonly DependencyProperty ActiveNoteProperty = DependencyProperty.Register("ActiveNote",
                                                                                                        typeof(IScaleNote),
                                                                                                        typeof(Keyboard),
                                                                                                        new PropertyMetadata(null));
        [Bindable(true)]
        public IScaleNote ActiveNote
        {
            get { return (IScaleNote)GetValue(ActiveNoteProperty); }
            set { SetValue(ActiveNoteProperty, value); }
        }

        #endregion ActiveNote

        #region IsConfigBarVisible

        public static readonly DependencyProperty IsConfigBarVisibleProperty = DependencyProperty.Register("IsConfigBarVisible",
                                                                                                    typeof(bool),
                                                                                                    typeof(Keyboard),
                                                                                                    new PropertyMetadata(true));


        public bool IsConfigBarVisible
        {
            get { return (bool)GetValue(IsConfigBarVisibleProperty); }
            set { SetValue(IsConfigBarVisibleProperty, value); }
        }

        #endregion IsConfigBarVisible

        #region VelocityLabel

        public static readonly DependencyProperty VelocityLabelProperty = DependencyProperty.Register("VelocityLabel",
            typeof (string), typeof (Keyboard), new PropertyMetadata("VELOCITY"));

        public string VelocityLabel
        {
            get { return (string)GetValue(VelocityLabelProperty); }
            set { SetValue(VelocityLabelProperty, value); }
        }

        #endregion VelocityLabel(

        #region SustainLabel

        public static readonly DependencyProperty SustainLabelProperty = DependencyProperty.Register("SustainLabel",
            typeof(string), typeof(Keyboard), new PropertyMetadata("SUSTAIN"));

        public string SustainLabel
        {
            get { return (string)GetValue(SustainLabelProperty); }
            set { SetValue(SustainLabelProperty, value); }
        }

        #endregion SustainLabel

        #endregion dependency properties

        // TODO change it use an IMidiPerformance

        private readonly IMidiLink _midiLink;

        public Keyboard()
            : this(MidiLink.Instance)
        {
        }

        public Keyboard(IMidiLink midiLink)
        {
            if (midiLink == null)
                throw new ArgumentNullException("midiLink");
            _midiLink = midiLink;

            InitializeComponent();
            SizeChanged += HandleSizeChanged;

            if (!this.IsInDesignMode())
                _midiLink.AttachListener(this);
        }

        private void HandleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // how many white keys can fit at the current keyboard size? Min = 1, Max = scale size
            int howManyKeysFit = (int)Math.Floor((ActualWidth - KeyPanel.Margin.Left - KeyPanel.Margin.Right) / KEY_WIDTH);
            howManyKeysFit = Math.Min(howManyKeysFit, Scale.EqualTemperedScale.Count);
            howManyKeysFit = Math.Max(howManyKeysFit, 1);

            if (howManyKeysFit != Notes.Count)
                LoadNotes(howManyKeysFit);
        }

        private ICommand _shiftKeysCommand;
        public ICommand ShiftKeysCommand
        {
            get { return _shiftKeysCommand ?? (_shiftKeysCommand = new Command(p => ShiftKeys(int.Parse(p.ToString())))); }
        }

        private void ShiftKeys(int shiftAmount)
        {
            int targetNoteNumber;

            if (shiftAmount < -12)
                targetNoteNumber = Scale.EqualTemperedScale.First().MidiNoteNumber;
            else if (shiftAmount > 12)
                targetNoteNumber = Scale.EqualTemperedScale.Last().MidiNoteNumber - Notes.Count + 1;
            else
                targetNoteNumber = BottomNote.MidiNoteNumber + shiftAmount;

            IScaleNote newNote = Scale.EqualTemperedScale.FirstOrDefault(n => n.MidiNoteNumber == targetNoteNumber);
            while (newNote != null && !newNote.IsWhiteKey)
            {
                targetNoteNumber += Math.Sign(shiftAmount);
                newNote = Scale.EqualTemperedScale.FirstOrDefault(n => n.MidiNoteNumber == targetNoteNumber);
            }

            if (newNote != null)
            {
                BottomNote = newNote;
                LoadNotes(Notes.Count(n => n.Item.IsWhiteKey));
            }
        }

        private void LoadNotes(int numberOfKeys)
        {
            if (ActualWidth == 0)
                return;

            // Find the last white note that fits. If there aren't enough notes above the bottom note in the scale, drop the bottom 
            //  note so we get the right number of notes.
            IScaleNote[] whiteNotesToUse = WhiteKeyNotes.SkipWhile(n => n.MidiNoteNumber < BottomNote.MidiNoteNumber).ToArray();
            if (whiteNotesToUse.Length > numberOfKeys)
            {
                whiteNotesToUse = whiteNotesToUse.Take(numberOfKeys).ToArray();
            }
            else if (whiteNotesToUse.Length < numberOfKeys)
            {
                BottomNote = WhiteKeyNotes.ElementAt(WhiteKeyNotes.Count - numberOfKeys);
                whiteNotesToUse = WhiteKeyNotes.SkipWhile(n => n.MidiNoteNumber < BottomNote.MidiNoteNumber).ToArray();
            }
            IScaleNote topNote = whiteNotesToUse.Last();

            // locate the top and bottom note in the master scale, and use everything between them, inclusive.
            int startIndex = Scale.EqualTemperedScale.ToList().IndexOf(BottomNote);
            int endIndex = Scale.EqualTemperedScale.ToList().IndexOf(topNote);

            ClearNotes();
            AddNotes(startIndex, endIndex);
        }

        private void ClearNotes()
        {
            Notes.Execute(item => item.PropertyChanged -= HandleNoteSelectionChanged);
            _notes.Clear();   
        }

        private void AddNotes(int bottomNoteIndex, int topNoteIndex)
        {
            for (int i = bottomNoteIndex; i <= topNoteIndex; i++)
            {
                ItemExtender<IScaleNote, bool> item = new ItemExtender<IScaleNote, bool>(Scale.EqualTemperedScale[i], false);
                item.PropertyChanged += HandleNoteSelectionChanged;
                _notes.Add(item);
            }
        }

        private void HandleNoteSelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemExtender<IScaleNote, bool> source = sender as ItemExtender<IScaleNote, bool>;
            if (source == null)
                return;

            // TODO remove this debug code once I get the velocity initialized correctly
            if (double.IsNaN(Velocity))
                Dispatcher.Invoke(DispatcherPriority.Send, new Action(() => Velocity = 0.8f));

            if (source.Value == true)
            {
                _midiLink.InjectNoteOn(source.Item.MidiNoteNumber, (int)(Velocity * 127));
                _notes.Where(item => !Equals(sender, item)).Execute(item => item.Value = false);
            }
            else
            {
                _midiLink.InjectNoteOff(source.Item.MidiNoteNumber, 0);
            }
        }

        private List<IScaleNote> _whiteKeyNotes;
        public IReadOnlyCollection<IScaleNote> WhiteKeyNotes
        {
            get { return _whiteKeyNotes ?? (_whiteKeyNotes = Scale.EqualTemperedScale.Where(n => n.IsWhiteKey).ToList()); }
        }

        private readonly ObservableCollection<ItemExtender<IScaleNote, bool>> _notes = new ObservableCollection<ItemExtender<IScaleNote, bool>>();
        public IReadOnlyCollection<ItemExtender<IScaleNote, bool>> Notes
        {
            get { return _notes; }
        }

        private void StopAllNotes()
        {
            Notes.Where(n => n.Value).Execute(item => item.Value = false);
        }

        #region IMidiListener

        public void HandleNoteOn(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni)
        {
            ItemExtender<IScaleNote, bool> note = Notes.FirstOrDefault(n => n.Item.MidiNoteNumber == noteNumber);
            if (note != null && !note.Value)
            {
                //Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => Velocity = velocity / 127d));
                note.Value = true;
            }
        }

        public void HandleNotePressure(int noteNumber, int value, MidiChannel channel = MidiChannel.Omni)
        {
            // do nothing
        }

        public void HandleNoteOff(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni)
        {
            ItemExtender<IScaleNote, bool> note = Notes.FirstOrDefault(n => n.Item.MidiNoteNumber == noteNumber);
            if (note != null && note.Value)
                note.Value = false;
        }

        public void HandlePitchWheel(int value, MidiChannel channel = MidiChannel.Omni)
        {
            // do nothing
        }

        public void HandleChannelPressure(int value, MidiChannel channel = MidiChannel.Omni)
        {
            // do nothing
        }

        public void HandleControlChange(int controllerNumber, int value, MidiChannel channel = MidiChannel.Omni)
        {
            if (controllerNumber == MIDI_CONTROLLERNUMBER_SUSTAIN)
                Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => IsSustained = Math.Round(value / 127f, 0) > 0));
        }

        public void HandleAllNotesOff(MidiChannel channel = MidiChannel.Omni)
        {
            StopAllNotes();
        }

        public void HandleResetControllers(MidiChannel channel = MidiChannel.Omni)
        {
            IsSustained = false;
        }

        public MidiChannel Channel
        {
            get { return MidiChannel.Omni; }
        }

        #endregion IMidiListener
    }
}
