using System;
using System.ComponentModel;
using System.Windows;

using Synth.Core;
using Synth.NAudio;
using Synth.MIDI;
using Synth.PerformanceModel;
using Synth.WPF.Util;

namespace Synth.WPF.Instrument
{
    public class SimpleInstrumentBase : InstrumentBase
    {
        #region dependency properties

        #region Pitch

        private static readonly DependencyPropertyKey PitchPropertyKey = DependencyProperty.RegisterReadOnly("Pitch",
                                                                                                              typeof(double),
                                                                                                              typeof(SimpleInstrumentBase),
                                                                                                              new PropertyMetadata((double)Scale.EqualTemperedScale["A4"].Pitch));
        public static readonly DependencyProperty PitchProperty = PitchPropertyKey.DependencyProperty;

        [Bindable(true)]
        public double Pitch
        {
            get { return (double)GetValue(PitchProperty); }
            protected set { SetValue(PitchPropertyKey, value); }
        }

        #endregion Pitch

        #region Intensity

        private static readonly DependencyPropertyKey IntensityPropertyKey = DependencyProperty.RegisterReadOnly("Intensity",
                                                                                                              typeof(double),
                                                                                                              typeof(SimpleInstrumentBase),
                                                                                                              new PropertyMetadata(0d, BaseControlInputValueChanged));
        public static readonly DependencyProperty IntensityProperty = IntensityPropertyKey.DependencyProperty;

        [Bindable(true)]
        public double Intensity
        {
            get { return (double)GetValue(IntensityProperty); }
            protected set { SetValue(IntensityPropertyKey, value); }
        }

        #endregion Intensity

        #region ModulationAmount

        private static readonly DependencyPropertyKey ModulationAmountPropertyKey = DependencyProperty.RegisterReadOnly("ModulationAmount",
                                                                                                              typeof(double),
                                                                                                              typeof(SimpleInstrumentBase),
                                                                                                              new PropertyMetadata(0d));
        public static readonly DependencyProperty ModulationAmountProperty = ModulationAmountPropertyKey.DependencyProperty;

        [Bindable(true)]
        public double ModulationAmount
        {
            get { return (double)GetValue(ModulationAmountProperty); }
            protected set { SetValue(ModulationAmountPropertyKey, value); }
        }

        #endregion ModulationAmount

        #region PitchPendRange

        public static readonly DependencyProperty PitchBendRangeProperty = DependencyProperty.Register("PitchBendRange",
                                                                                         typeof(double),
                                                                                         typeof(SimpleInstrumentBase),
                                                                                         new PropertyMetadata(3d, BaseControlInputValueChanged));
        [Bindable(true)]
        public double PitchBendRange
        {
            get { return (double)GetValue(PitchBendRangeProperty); }
            set { SetValue(PitchBendRangeProperty, value); }
        }

        #endregion PitchPendRange

        #region Switch

        private static readonly DependencyPropertyKey SwitchPropertyKey = DependencyProperty.RegisterReadOnly("Switch",
                                                                                                              typeof(bool),
                                                                                                              typeof(SimpleInstrumentBase),
                                                                                                              new PropertyMetadata(false, BaseControlInputValueChanged));
        public static readonly DependencyProperty SwitchProperty = SwitchPropertyKey.DependencyProperty;

        [Bindable(true)]
        public bool Switch
        {
            get { return (bool)GetValue(SwitchProperty); }
            protected set { SetValue(SwitchPropertyKey, value); }
        }

        #endregion Switch

        #endregion dependency properties

        public const double MinimumBendRange = 0;
        public const double MaximumBendRange = 3;

        static SimpleInstrumentBase()
        {
            InitializeClassMetaData<SimpleInstrumentBase>();
        }

        protected static ISimplePerformance GetDefaultPerformance()
        {
            int numberOfVoices = 1;
            IMidiPerformance midiPerformance = new MidiPerformance(MidiLink.Instance, MidiValueStrategy.Normalized);
            ISimplePerformance defaultPerformance = new SimplePerformance(midiPerformance, Scale.EqualTemperedScale, numberOfVoices);
            return defaultPerformance;
        }

        public SimpleInstrumentBase()
        {
            // Constructor and class must be public to work with the WPF designer, but we don't want direct instantiation
            if (!this.IsInDesignMode() && GetType() == typeof(SimpleInstrumentBase))
                throw new InvalidOperationException(GetType().Name + " cannot be directly instanced at runtime. Derive a class from it, and create an instance of that.");

        }
    }
}
