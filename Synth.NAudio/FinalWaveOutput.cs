using NAudio.Wave;

namespace Synth.NAudio
{
    public class FinalWaveOutput : FinalOutputBase
    {
        public FinalWaveOutput(int sampleRate) 
            : base(sampleRate)
        {
        }

        protected override IWavePlayer GetWavePlayer()
        {
            // TODO could these params use a little more fine-tuning?
            return new WaveOutEvent() { DesiredLatency = 65, NumberOfBuffers = 5 };
        }

        public override bool IsSampleRateValidForDriver(int sampleRate)
        {
            // TODO validate sample rate to be legal for WaveOut
            return true;
        }

    }
}