using System;

using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

using Synth.Audio;
using Synth.Core;
using Synth.WPF.Util;

namespace Synth.WPF.Modules
{
    public sealed class AudioMeter : Module, IClockListener
    {
        #region I/O dependency properties

        #region Input

        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input",
                                                                                    typeof(AudioWire),
                                                                                    typeof(AudioMeter),
                                                                                    new PropertyMetadata(HandleInputChanged));
        private static void HandleInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // storing the value in a class variable allows it to be accessed from any thread.
            (d as AudioMeter)._input = e.NewValue as AudioWire;
        }

        private AudioWire _input;

        public AudioWire Input
        {
            get { return _input; }
            set { SetValue(InputProperty, value); }
        }

        #endregion Input

        #region Level

        private static readonly DependencyPropertyKey LevelPropertyKey = DependencyProperty.RegisterReadOnly("Level",
                                                                                                              typeof(double),
                                                                                                              typeof(AudioMeter),
                                                                                                              new PropertyMetadata(0d));
        public static readonly DependencyProperty LevelProperty = LevelPropertyKey.DependencyProperty;

        [Bindable(true)]
        public double Level
        {
            get { return (double)GetValue(LevelProperty); }
            private set { SetValue(LevelPropertyKey, value); }
        }

        #endregion Level

        #endregion I/O dependency properties
                                
        static AudioMeter()
        {
            InitializeClassMetadata<AudioMeter>(WPF.Resources.AudioMeterTemplate);
        }

        private readonly IAudioLink _audioLink;
        private readonly RunningAverage _averager = new RunningAverage(2205);       // 1/20 sec #44.1Khz
        private const int METER_REFRESH_FREQ = 25;

        private bool _isEnabled = true;
        private ulong _lastRequestId;
        private AudioClockDivider _clockSource;

        public AudioMeter()
            : this(AudioLink.Instance)
        {
        }

        public AudioMeter(IAudioLink audioLink)
            : base(audioLink)
        {
            if (audioLink == null)
                throw new ArgumentNullException("audioLink");
            _audioLink = audioLink;

            IsEnabledChanged += HandleIsEnabledChanged;

            Loaded += HandleLoaded;
            Unloaded += HandleUnloaded;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            if (_clockSource != null)
                _clockSource.Dispose();

            _clockSource = null;

            if (!this.IsInDesignMode())
                _clockSource = new AudioClockDivider(_audioLink, METER_REFRESH_FREQ, id => HandleControlClockTick());
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            if (_clockSource != null)
                _clockSource.Dispose();

            _clockSource = null;
        }

        private void HandleIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _isEnabled = (bool) e.NewValue;
        }

        void IClockListener.HandleClockTick(ulong tickId)
        {
            ForceSampleRecalc(tickId);
        }

        private void HandleControlClockTick()
        {
            double newValue = _isEnabled ? _averager.Average : 0d;
            // Invoked at a lower priority because it's just for updating graphical meters in the UI, so it doesn't need to slow things down.
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => Level = newValue));
        }

        private void ForceSampleRecalc(ulong requestId)
        {
            if (requestId != _lastRequestId)
            {
                AudioSample newSample = new AudioSample();
                if (Input != null)
                    newSample = Input(requestId);

                _averager.Add(Math.Abs(newSample.L) + Math.Abs(newSample.R));

                _lastRequestId = requestId;
            }
        }
    }
}
