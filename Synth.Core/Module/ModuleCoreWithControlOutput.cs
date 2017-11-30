using System;
using Synth.Util;

namespace Synth.Module
{
    public abstract class ModuleCoreWithControlOutput : ModuleCore
    {
        public event EventHandler<EventArgs<float>> OutputChanged;

        private float _output = float.NaN;

        protected ModuleCoreWithControlOutput()
        {            
        }

        public float Output
        {
            get { return _output; }
            protected set
            {
                if (_output != value)
                {
                    _output = value;

                    EventHandler<EventArgs<float>> evt = OutputChanged;
                    if (evt != null)
                        evt.Invoke(this, new EventArgs<float>(_output));
                }
            }
        }
    }
}
