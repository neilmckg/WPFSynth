using System.Collections.Concurrent;

namespace Synth.WPF.Util
{
    public class RunningAverage
    {
        private ConcurrentQueue<double> _buffer = new ConcurrentQueue<double>();

        public RunningAverage(int sampleSize)
        {
            Limit = sampleSize;
        }

        // TODO enable changing the limit on the fly by changing the buffer size and reinitializing the average.
        public int Limit { get; private set; }

        public void Add(double item)
        {
            double overflow;
            while ((_buffer.Count + 1) > Limit && _buffer.TryDequeue(out overflow))
            {
                if (Limit == 0)
                    Average = 0;
                else
                    Average -= (overflow / Limit);
            }

            _buffer.Enqueue(item);
            Average += (item / Limit);
        }

        public double Average { get; private set; }
    }
}
