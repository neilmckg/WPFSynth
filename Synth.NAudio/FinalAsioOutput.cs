using System;
using System.Collections.Generic;

using NAudio.Wave;

namespace Synth.NAudio
{
    public class FinalAsioOutput : FinalOutputBase
    {
        private readonly Func<IEnumerable<string>, string> _driverSelector;

        public FinalAsioOutput(int sampleRate, Func<IEnumerable<string>, string> driverSelector)
            : base(sampleRate)
        {
            if (driverSelector == null)
                throw new ArgumentNullException("driverSelector");
            _driverSelector = driverSelector;
        }

        protected override IWavePlayer GetWavePlayer()
        {
            string driverName;
            string[] driverNames = AsioOut.GetDriverNames();

            if (driverNames.Length == 0)
                throw new InvalidOperationException("No ASIO driver found.");
            else if (driverNames.Length == 1)
                driverName = driverNames[0];
            else
                driverName = _driverSelector(driverNames);

            if (string.IsNullOrWhiteSpace(driverName))
                throw new InvalidOperationException("A driverName was not selected.");

            return new AsioOut(driverName);
        }

        public override bool IsSampleRateValidForDriver(int sampleRate)
        {
            // TODO validate sample rate to be legal for ASIO
            return true;
        }
    }
}