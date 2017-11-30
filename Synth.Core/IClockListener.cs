namespace Synth.Core
{
    public interface IClockListener
    {
        void HandleClockTick(ulong tickId);
    }
}
