using System;
using System.Threading;
using System.Threading.Tasks;

namespace Synth.Util
{
    public sealed class Timer : CancellationTokenSource, IDisposable
    {
        private static readonly EventArgs _args = new EventArgs();
        
        public event EventHandler Tick;

        private TimeSpan _interval = TimeSpan.Zero;
        private bool _isPaused = true;

        public Timer(TimeSpan interval)
        {
            Interval = interval;
        }

        public TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                if (value == _interval)
                    return;

                _interval = value;

                if (_isPaused && !ShouldBePaused)
                    StartCycle();
            }
        }

        private void StartCycle()
        {
            if (Token.IsCancellationRequested)
                return;

            _isPaused = ShouldBePaused;

            if (!_isPaused)
                Task.Delay(Interval, Token).ContinueWith((task, o) => OnTick(), null, Token, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        private void OnTick()
        {
            EventHandler evt = Tick;
            if (evt != null && !Token.IsCancellationRequested && !ShouldBePaused)
                evt.Invoke(this, _args);

            StartCycle();
        }

        private bool ShouldBePaused
        {
            get { return Interval == TimeSpan.Zero; }
        }

        public new void Dispose()
        {
            base.Cancel();
        }
    }
}
