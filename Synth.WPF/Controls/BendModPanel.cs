using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using Synth.NAudio;
using Synth.MIDI;
using Synth.PerformanceModel;

namespace Synth.WPF.Controls
{
    public class BendModPanel : Control, IMidiListener
    {
        #region dependency properties

        #region ModulationAmount

        public static readonly DependencyProperty ModulationAmountProperty = DependencyProperty.Register("ModulationAmount",
                                                                                                typeof(double),
                                                                                                typeof(BendModPanel),
                                                                                                new PropertyMetadata(double.NaN, HandleModulationAmountChanged));

        private static void HandleModulationAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BendModPanel)._midiLink.InjectControlChange(MIDI_CONTROLLERNUMBER_MODULATION, (int)((double)e.NewValue*127d));
        }

        [Bindable(true)]
        public double ModulationAmount
        {
            get { return (double)GetValue(ModulationAmountProperty); }
            set { SetValue(ModulationAmountProperty, value); }
        }

        #endregion ModulationAmount

        #region PitchBendAmount

        public static readonly DependencyProperty PitchBendAmountProperty = DependencyProperty.Register("PitchBendAmount",
                                                                                                typeof(double),
                                                                                                typeof(BendModPanel),
                                                                                                new PropertyMetadata(0d, HandlePitchBendChanged));

        private static void HandlePitchBendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int rawPitchBend = Convert.ToInt32(((double) e.NewValue + 1)*8192d);
            (d as BendModPanel)._midiLink.InjectPitchWheel(rawPitchBend);
        }

        [Bindable(true)]
        public double PitchBendAmount
        {
            get { return (double)GetValue(PitchBendAmountProperty); }
            set { SetValue(PitchBendAmountProperty, value); }
        }

        #endregion PitchBendAmount

        #endregion dependency properties

        #region static members

        static BendModPanel()
        {
            ControlTemplate defaultTemplate = GetResource<ControlTemplate>("BendModDefaultTemplate");
            TemplateProperty.OverrideMetadata(typeof(BendModPanel), new FrameworkPropertyMetadata(defaultTemplate));
        }
        
        private static T GetResource<T>(string key)
            where T : class
        {
            ResourceDictionary dictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/Synth.WPF;component/Controls/ControlResources.xaml") };
            return dictionary[key] as T;
        }

        #endregion static members

        private const int MIDI_CONTROLLERNUMBER_MODULATION = 1;
        private readonly IMidiLink _midiLink;
        private readonly IMidiPerformance _midiSource;

        public BendModPanel()
            : this(MidiLink.Instance)
        {
        }

        public BendModPanel(IMidiLink midiLink)
        {
            if (midiLink == null)
                throw new ArgumentNullException("midiLink");
            _midiLink = midiLink;

            // TODO how can I restrict this in design mode?
            _midiSource = new MidiPerformance(_midiLink, MidiValueStrategy.Normalized);
            _midiSource.PropertyChanged += HandleMidiSourcePropertyChanged;
        }

        private void HandleMidiSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PitchBend")
                Dispatcher.BeginInvoke(DispatcherPriority.Send,new Action(() => PitchBendAmount = _midiSource.PitchBend));
            else if (e.PropertyName == "Modulation")
                Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => ModulationAmount = _midiSource.Modulation));
        }

        #region IMidiListener

        public void PublishMidiState()
        {
            _midiLink.InjectControlChange(MIDI_CONTROLLERNUMBER_MODULATION, (int)(ModulationAmount * 127));
            _midiLink.InjectPitchWheel((int)(PitchBendAmount + 1) * 8192);
        }

        public MidiChannel Channel
        {
            get { return MidiChannel.Omni; }
        }

        public void HandleNoteOn(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni)
        {
            // ignore
        }

        public void HandleNotePressure(int noteNumber, int value, MidiChannel channel = MidiChannel.Omni)
        {
            // ignore
        }

        public void HandleNoteOff(int noteNumber, int velocity, MidiChannel channel = MidiChannel.Omni)
        {
            // ignore
        }

        public void HandlePitchWheel(int value, MidiChannel channel = MidiChannel.Omni)
        {
            PitchBendAmount = (value/8192d) - 1;
        }

        public void HandleChannelPressure(int value, MidiChannel channel = MidiChannel.Omni)
        {
            // ignore
        }

        public void HandleControlChange(int controllerNumber, int value, MidiChannel channel = MidiChannel.Omni)
        {
            if (controllerNumber == MIDI_CONTROLLERNUMBER_MODULATION)
                ModulationAmount = value/127d;
        }

        public void HandleAllNotesOff(MidiChannel channel = MidiChannel.Omni)
        {
            // ignore
        }

        public void HandleResetControllers(MidiChannel channel = MidiChannel.Omni)
        {
            PitchBendAmount = 0;
            ModulationAmount = 0;
        }

        #endregion IMidiListener
    }
}
