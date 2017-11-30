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

using System.ComponentModel;
using Synth.Audio;
using Synth.WPF.Modules;

namespace Synth.WPF.Demo.HostModules
{
    /// <summary>
    /// Interaction logic for Vibrato.xaml
    /// </summary>
    public partial class Vibrato : HostModule
    {
        public const double MinimumAmount = 0;
        public const double MaximumAmount = 1;

        public const double MinimumDelaySeconds = 0;
        public const double MaximumDelaySeconds = 10;

        public const double MinimumRate = 1;
        public const double MaximumRate = 10;

        public const double MinimumPitchToVolumeBalance = -1;
        public const double MaximumPitchToVolumeBalance = 1;

        public const double MinimumPitchIn = 0;
        public const double MaximumPitchIn = 1;

        public const double MinimumVolumeIn = 0;
        public const double MaximumVolumeIn = 1;

        #region dependency properties

        #region Amount

        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount",
                                                                         typeof(double),
                                                                         typeof(Vibrato),
                                                                         new PropertyMetadata(0d));

        [Bindable(true)]
        public double Amount
        {
            get { return (double)GetValue(AmountProperty); }
            set { SetValue(AmountProperty, value); }
        }

        #endregion Amount

        #region Trigger

        public static readonly DependencyProperty TriggerProperty = DependencyProperty.Register("Trigger",
                                                                         typeof(bool),
                                                                         typeof(Vibrato),
                                                                         new PropertyMetadata(false, BaseControlInputValueChanged));
        [Bindable(true)]
        public bool Trigger
        {
            get { return (bool)GetValue(TriggerProperty); }
            set { SetValue(TriggerProperty, value); }
        }

        #endregion Trigger

        #region DelaySeconds

        public static readonly DependencyProperty DelaySecondsProperty = DependencyProperty.Register("DelaySeconds",
                                                                         typeof(double),
                                                                         typeof(Vibrato),
                                                                         new PropertyMetadata(1d));

        [Bindable(true)]
        public double DelaySeconds
        {
            get { return (double)GetValue(DelaySecondsProperty); }
            set { SetValue(DelaySecondsProperty, value); }
        }

        #endregion DelaySeconds

        #region Rate

        public static readonly DependencyProperty RateProperty = DependencyProperty.Register("Rate",
                                                                         typeof(double),
                                                                         typeof(Vibrato),
                                                                         new PropertyMetadata(4.5d));

        [Bindable(true)]
        public double Rate
        {
            get { return (double)GetValue(RateProperty); }
            set { SetValue(RateProperty, value); }
        }

        #endregion Rate

        #region PitchToVolumeBalance

        public static readonly DependencyProperty PitchToVolumeBalanceProperty = DependencyProperty.Register("PitchToVolumeBalance",
                                                                         typeof(double),
                                                                         typeof(Vibrato),
                                                                         new PropertyMetadata(0d));

        [Bindable(true)]
        public double PitchToVolumeBalance
        {
            get { return (double)GetValue(PitchToVolumeBalanceProperty); }
            set { SetValue(PitchToVolumeBalanceProperty, value); }
        }

        #endregion PitchToVolumeBalance

        #region PitchIn

        public static readonly DependencyProperty PitchInProperty = DependencyProperty.Register("PitchIn",
                                                                         typeof(double),
                                                                         typeof(Vibrato),
                                                                         new PropertyMetadata(0d));

        [Bindable(true)]
        public double PitchIn
        {
            get { return (double)GetValue(PitchInProperty); }
            set { SetValue(PitchInProperty, value); }
        }

        #endregion PitchIn

        #region VolumeIn

        public static readonly DependencyProperty VolumeInProperty = DependencyProperty.Register("VolumeIn",
                                                                         typeof(double),
                                                                         typeof(Vibrato),
                                                                         new PropertyMetadata(0d, BaseControlInputValueChanged));

        [Bindable(true)]
        public double VolumeIn
        {
            get { return (double)GetValue(VolumeInProperty); }
            set { SetValue(VolumeInProperty, value); }
        }

        #endregion VolumeIn

        #region PitchOut

        public static readonly DependencyProperty PitchOutProperty = DependencyProperty.Register("PitchOut", typeof(double), typeof(Vibrato), new PropertyMetadata(0d));

        public double PitchOut
        {
            get { return (double)GetValue(PitchOutProperty); }
            set { SetValue(PitchOutProperty, value); }
        }

        #endregion PitchOut

        #region VolumeOut

        public static readonly DependencyProperty VolumeOutProperty = DependencyProperty.Register("VolumeOut", typeof(double), typeof(Vibrato), new PropertyMetadata(0d));

        public double VolumeOut
        {
            get { return (double)GetValue(VolumeOutProperty); }
            set { SetValue(VolumeOutProperty, value); }
        }

        #endregion VolumeOut

        #region LfoOut

        public static readonly DependencyProperty LfoOutProperty = DependencyProperty.Register("LfoOut", typeof(double), typeof(Vibrato), new PropertyMetadata(0d));

        public double LfoOut
        {
            get { return (double)GetValue(LfoOutProperty); }
            set { SetValue(LfoOutProperty, value); }
        }

        #endregion LfoOut

        #endregion dependency properties

        public Vibrato() 
            : this(AudioLink.Instance)
        {
        }

        public Vibrato(IAudioLink audioLink) 
            : base(audioLink)
        {
            InitializeComponent();
        }
    }
}
