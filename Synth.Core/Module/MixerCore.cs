using System.Collections.ObjectModel;

using Synth.Audio;
using Synth.Core;
using Synth.Util;

namespace Synth.Module
{
    public class MixerCore : ModuleCoreWithAudioOutput
    {
        private readonly ObservableCollection<IMixerChannel> _channels = new ObservableCollection<IMixerChannel>();
        private readonly ThrottledFloat _masterLevel = new ThrottledFloat(0.05f, 1);

        public MixerCore()
        {
            MasterLevel = new FloatInput("Master Level", 1, 0, 1, ValueOutOfRangeStrategy.ForceIntoRange, (name, oldVal, newVal) => MasterLevelChanged(newVal));
        }

        public FloatInput MasterLevel{ get; private set; }

        public ObservableCollection<IMixerChannel> Channels
        {
            get { return _channels; }
        }

        private void MasterLevelChanged(float newValue)
        {
            _masterLevel.Target = newValue;
        }

        protected override AudioSample CalculateNextSample(ulong requestId)
        {
            AudioSample result = new AudioSample();
            if (IsActive)
            {
                _channels.Execute(channel => result += channel.GetSample(requestId));
                result *= _masterLevel.GetNextSample();
            }

            return result;
        }
    }
}
