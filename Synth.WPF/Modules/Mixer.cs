using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

using Synth.Audio;
using Synth.Module;

namespace Synth.WPF.Modules
{
    [ContentProperty("Channels")]
    public sealed class Mixer : Module, IAudioSource
    {
        #region I/O dependency properties

        #region MasterLevel

        public static readonly DependencyProperty MasterLevelProperty = DependencyProperty.Register("MasterLevel",
                                                                         typeof(double),
                                                                         typeof(Mixer),
                                                                         new PropertyMetadata(1d, BaseControlInputValueChanged));

        [Bindable(true)]
        public double MasterLevel
        {
            get { return (double)GetValue(MasterLevelProperty); }
            set { SetValue(MasterLevelProperty, value); }
        }

        #endregion MasterLevel

        #region Output

        private static readonly DependencyPropertyKey OutputPropertyKey = DependencyProperty.RegisterReadOnly("Output",
                                                                                                              typeof(AudioWire),
                                                                                                              typeof(Mixer),
                                                                                                              new PropertyMetadata());
        public static readonly DependencyProperty OutputProperty = OutputPropertyKey.DependencyProperty;

        public AudioWire Output
        {
            get { return (AudioWire)GetValue(OutputProperty); }
            private set { SetValue(OutputPropertyKey, value); }
        }

        #endregion Output

        #endregion I/O dependency properties

        static Mixer()
        {
            InitializeClassMetadata<Mixer>(WPF.Resources.MixerTemplate);
        }

        private readonly MixerCore _core;

        public Mixer()
            : this(AudioLink.Instance)
        {
        }

        public Mixer(IAudioLink audioLink)
            : base(audioLink)
        {
            _core = new MixerCore();
            RegisterChildCollection(_core.Channels);

            Output = GetSample;
        }

        public AudioSample GetSample(ulong requestId)
        {
            return _core.GetSample(requestId);
        }

        protected override void InputValueChanged(string inputName)
        {
            if (inputName == MasterLevelProperty.Name)
                _core.MasterLevel.Value = (float)MasterLevel;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<IMixerChannel> Channels
        {
            get { return _core.Channels; }
        }
    }
}
