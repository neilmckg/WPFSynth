namespace Synth.Module
{
    public abstract class ModuleCore
    {
        protected ModuleCore()
        {
        }

        private bool _isActive = true;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnIsActiveChanged();
                }
            }
        }

        protected virtual void OnIsActiveChanged()
        {
        }
    }
}
