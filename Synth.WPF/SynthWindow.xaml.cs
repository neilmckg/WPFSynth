using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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

using Synth.NAudio;
using Synth.Audio;
using Synth.PerformanceModel;
using Synth.WPF.Instrument;
using Synth.WPF.Properties;
using Synth.WPF.Util;
using System.ComponentModel;
using System.Windows.Threading;

namespace Synth.WPF
{
    /// <summary>
    /// Interaction logic for SynthWindow.xaml
    /// </summary>
    public partial class SynthWindow : Window
    {
        #region dependency properties

        #region CurrentInstrument

        public static readonly DependencyProperty CurrentInstrumentProperty = DependencyProperty.Register("CurrentInstrument", typeof(InstrumentToken), typeof(SynthWindow), new PropertyMetadata(HandleCurrentInstrumentChanged));

        private static void HandleCurrentInstrumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SynthWindow synth = d as SynthWindow;
            if (synth != null)
                synth.ApplyToken(e.NewValue as InstrumentToken);
        }

        public InstrumentToken CurrentInstrument
        {
            get { return (InstrumentToken) GetValue(CurrentInstrumentProperty); }
            set { SetValue(CurrentInstrumentProperty, value); }
        }

        #endregion CurrentInstrument

        #region VelocityLabel

        public static readonly DependencyProperty VelocityLabelProperty = DependencyProperty.Register("VelocityLabel",
            typeof(string), typeof(SynthWindow), new PropertyMetadata("Velocity"));

        public string VelocityLabel
        {
            get { return (string)GetValue(VelocityLabelProperty); }
            set { SetValue(VelocityLabelProperty, value); }
        }

        #endregion VelocityLabel

        #region SustainLabel

        public static readonly DependencyProperty SustainLabelProperty = DependencyProperty.Register("SustainLabel",
            typeof(string), typeof(SynthWindow), new PropertyMetadata("Sustain"));

        public string SustainLabel
        {
            get { return (string)GetValue(SustainLabelProperty); }
            set { SetValue(SustainLabelProperty, value); }
        }

        #endregion SustainLabel

        #region IsHeaderVisible

        public static readonly DependencyProperty IsHeaderVisibleProperty = DependencyProperty.Register("IsHeaderVisible", typeof(bool), typeof(SynthWindow), new PropertyMetadata(true));

        public bool IsHeaderVisible
        {
            get { return (bool) GetValue(IsHeaderVisibleProperty); }
            set { SetValue(IsHeaderVisibleProperty, value);}
        }

        #endregion IsHeaderVisible

        #region KeepDetailsHidden

        public static readonly DependencyProperty KeepDetailsHiddenProperty = DependencyProperty.Register("KeepDetailsHidden", typeof(bool), typeof(SynthWindow), new PropertyMetadata(false));

        public bool KeepDetailsHidden
        {
            get { return (bool)GetValue(KeepDetailsHiddenProperty); }
            set { SetValue(KeepDetailsHiddenProperty, value); }
        }

        #endregion KeepDetailsHidden

        public bool IsVerbose
        {
            get { return (bool)GetValue(AttachedProperties.IsVerboseProperty); }
            set { SetValue(AttachedProperties.IsVerboseProperty, value); }
        }

        #endregion dependency properties

        private readonly IAudioLink _audioLink;
        private readonly IMidiLink _midiLink;

        private readonly ObservableCollection<InstrumentToken> _instruments = new ObservableCollection<InstrumentToken>();
        public ObservableCollection<InstrumentToken> Instruments
        {
            get { return _instruments; }
        }

        public SynthWindow()
            : this(AudioLink.Instance, MidiLink.Instance)
        {
        }

        public SynthWindow(IAudioLink audioLink, IMidiLink midiLink)
        {
            if (audioLink == null)
                throw new ArgumentNullException("audioLink");
            _audioLink = audioLink;

            if (midiLink == null)
                throw new ArgumentNullException("midiLink");
            _midiLink = midiLink;

            Loaded += HandleLoaded;

            //SetAlternateColors();

            InitializeComponent();
        }

        private void ApplyToken(InstrumentToken token)
        {
            if (Content is Instrument.InstrumentBase)
                (Content as Instrument.InstrumentBase).IsActive = false;

            if (token == null)
            {
                Content = null;
                ClearValue(VelocityLabelProperty);
                ClearValue(SustainLabelProperty);
            }
            else
            {
                Instrument.InstrumentBase instrument = token.Factory();
                Content = instrument;
                VelocityLabel = instrument.VelocityLabel;
                SustainLabel = instrument.SustainLabel;

                _midiLink.RepublishState();
                instrument.IsActive = true;

                Settings.Default.ActiveInstrument = token.Name;
                Settings.Default.Save();
            }
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            if (Instruments.Any())
            {
                CurrentInstrument = Instruments.FirstOrDefault(t => t.Name == Settings.Default.ActiveInstrument);
                if (CurrentInstrument == null)
                {
                    if (Instruments.Contains(InstrumentFinder.DefaultInstrument))
                        CurrentInstrument = InstrumentFinder.DefaultInstrument;
                    else
                        CurrentInstrument = Instruments.First();
                }
            }
            else
            {
                _instruments.Add(InstrumentFinder.DefaultInstrument);
                CurrentInstrument = InstrumentFinder.DefaultInstrument;
            }

            //WPF.Resources.ForegroundBrush = Brushes.Yellow;
            //WPF.Resources.BackgroundBrush = Brushes.DarkGreen;
        }

        private void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //SetAlternateColors();
        }

        private void SetAlternateColors()
        {
            WPF.Resources.ForegroundBrush = Brushes.Yellow;
            WPF.Resources.BackgroundBrush = Brushes.DarkGreen;
            WPF.Resources.TrackColor = Colors.Pink;
            WPF.Resources.ControlMeterColor = Colors.Red;
            WPF.Resources.AudioMeterColor = Colors.Blue;
            WPF.Resources.PopoutColor = Color.FromArgb(128, 255, 0, 0);
            WPF.Resources.SynthWindowBrush = Brushes.Turquoise;
        }
    }
}
