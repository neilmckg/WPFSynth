using Synth.Audio;

namespace Synth.Module
{
    public abstract class ModuleCoreWithAudioOutput : ModuleCore, IAudioSource
    {
        private ulong _lastRequestId = 0;
        private AudioSample _currentSample = new AudioSample();

        protected ModuleCoreWithAudioOutput()
        {
        }

        public AudioSample GetSample(ulong requestId)
        {
            if (requestId != _lastRequestId)
            {
                _currentSample = CalculateNextSample(requestId);
                _lastRequestId = requestId;
            }

            if (IsActive)
                return _currentSample;
            else
                return new AudioSample();
        }

        protected abstract AudioSample CalculateNextSample(ulong requestId);
    }
}
