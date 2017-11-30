using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Synth.PerformanceModel;
using Synth.WPF.Util;

namespace Synth.WPF.Instrument
{
    public class MonoSimpleInstrument : SimpleInstrumentBase
    {
        // dependency properties:
        // Input Configuration
        // Performance Properties (need switch)
        // Mode (single/multi)?
        // voice count

        #region dependency properties

        #region IsLegatoEnabled

        public static readonly DependencyProperty IsLegatoEnabledProperty = DependencyProperty.Register("IsLegatoEnabled",
                                                                                         typeof(bool),
                                                                                         typeof(MonoSimpleInstrument),
                                                                                         new PropertyMetadata(true));
        [Bindable(true)]
        public bool IsLegatoEnabled
        {
            get { return (bool)GetValue(IsLegatoEnabledProperty); }
            set { SetValue(IsLegatoEnabledProperty, value); }
        }

        #endregion IsLegatoEnabled

        #endregion dependency properties

        //public const double MinimumBendRange = 0;
        //public const double MaximumBendRange = 3;

        static MonoSimpleInstrument()
        {
            InitializeClassMetaData<MonoSimpleInstrument>(WPF.Resources.MonoSimpleInstrumentInputTemplate, WPF.Resources.MonoSimpleInstrumentOutputTemplate);
        }

        private readonly ISimplePerformance _performance;

        public MonoSimpleInstrument()
        {
            if (!this.IsInDesignMode())    
                throw new InvalidOperationException("MonoSimpleInstrument's default constructor is available only for runtime use.");
        }

        public MonoSimpleInstrument(ISimplePerformance performanceSource)
        {
            if (performanceSource == null)
                throw new ArgumentNullException("performanceSource");
            _performance = performanceSource;

            _performance.IsLegato = true;

            // TODO support SimplePeformance configuration in the instrument properties
            _performance.ApplyHoldPedalToSustain = true;
            _performance.IntensitySource = ExpressionSources.ChannelPressure | ExpressionSources.BreathController | ExpressionSources.FootPedal;
            _performance.PitchBendRange = (float)PitchBendRange;

            _performance.PropertyChanged += HandlePerformancePropertyChanged;
            _performance.Voices.First().PropertyChanged += HandleVoicePropertyChanged;
        }

        private void Reset()
        {
            Intensity = 0;
            ModulationAmount = 0;
        }

        private ICommand _panicCommand;
        public ICommand PanicCommand
        {
            get { return _panicCommand ?? (_panicCommand = new Command(p => Reset())); }
        }

        #region support for wireable control inputs in derived classes

        //protected static void BaseControlInputValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is MonoSimpleInstrument)
        //        (d as MonoSimpleInstrument).InputValueChanged(e.Property.Name);
        //}

        protected override void InputValueChanged(string inputName)
        {
            //if (inputName == MidiChannelProperty.Name)
            //    Reset();
            //else 
            if (inputName == PitchBendRangeProperty.Name)
                _performance.PitchBendRange = (float)PitchBendRange;
            else
                base.InputValueChanged(inputName);
            //if (inputName == OutputSourceNameProperty.Name)
            //    SelectOutputModule();
        }

        #endregion support for wireable control inputs in derived classes

        private void HandleVoicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Pitch")
                Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => Pitch = _performance.Voices.First().Pitch));
            if (e.PropertyName == "Intensity")
                Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => Intensity = _performance.Voices.First().Intensity));
        }

        private void HandlePerformancePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Switch is not currently supported
            //if (e.PropertyName == "Switch")
            //    Switch = _performance.Switch;
            if (e.PropertyName == "ModulationAmount")
                Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() => ModulationAmount = _performance.ModulationAmount));
        }

    }
}
