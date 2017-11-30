using System.ComponentModel;
using System.Windows;

using Synth.Audio;
using Synth.Module;

namespace Synth.WPF.ModuleChildren
{
    // child component of Mixer module
    public sealed class Channel : FrameworkContentElement, IMixerChannel
    {
        #region I/O dependency properties

        #region Input

        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input",
                                                                                    typeof(AudioWire),
                                                                                    typeof(Channel),
                                                                                    new PropertyMetadata(HandleInputChanged));
        private static void HandleInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Channel channel = (d as Channel);
            AudioWire input = (e.NewValue) as AudioWire;

            // storing the value in a class variable allows it to be accessed from any thread.
            channel._input = input;

            if (input == null)
            {
                channel.ChannelName = "Unassigned";
            }
            else
            {
                Modules.Module source = input.Target as Modules.Module;
                if (source == null)
                    channel.ChannelName = "Unknown";
                else
                    channel.ChannelName = source.Description;
            }
        } 

        private AudioWire _input;

        public AudioWire Input
        {
            get { return _input; }
            set { SetValue(InputProperty, value); }
        }

        #endregion Input

        #region Level

        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level",
                                                                         typeof(float),
                                                                         typeof(Channel),
                                                                         new PropertyMetadata(1f, HandleLevelChanged));
        private static void HandleLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // storing the value in a class variable allows it to be accessed from any thread.
            (d as Channel)._level = (float)e.NewValue;
        }

        private float _level = (float)LevelProperty.DefaultMetadata.DefaultValue;

        [Bindable(true)]
        public float Level
        {
            get { return _level; }
            set { SetValue(LevelProperty, value); }
        }

        #endregion Level

        #region ChannelName

        private static readonly DependencyPropertyKey ChannelNameKey = DependencyProperty.RegisterReadOnly("ChannelName", typeof (string), typeof (Channel), null);
        public static readonly DependencyProperty ChannelNameProperty = ChannelNameKey.DependencyProperty;

        public string ChannelName
        {
            get { return (string) GetValue(ChannelNameProperty); }
            set { SetValue(ChannelNameKey, value); }
        }

        #endregion ChannelName

        #endregion I/O dependency properties

        public const double MinimumLevel = 0;
        public const double MaximumLevel = 1;

        private ulong _lastRequestId = 0;
        private AudioSample _lastSample = new AudioSample();

        public AudioSample GetSample(ulong requestId)
        {
            if (requestId != _lastRequestId)
            {
                if (Input == null)
                    _lastSample = new AudioSample();
                else
                    _lastSample = Input(requestId) * (float)Level;

                _lastRequestId = requestId;
            }

            return _lastSample;
        }
    }
}
