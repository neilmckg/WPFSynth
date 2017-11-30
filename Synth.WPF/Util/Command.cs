using System;
using System.Windows.Input;

namespace Synth.WPF.Util
{
    public class Command : ICommand
    {
        public virtual event EventHandler CanExecuteChanged;

        private Action<object> _executeAction;

        private Func<object, bool> _canExecuteFunction;

        public Command(Action<object> executeAction, Func<object, bool> canExecuteFunction)
            : this(executeAction)
        {
            _canExecuteFunction = canExecuteFunction;   // null is ok
        }

        public Command(Action<object> executeAction)
        {
            if (executeAction == null)
                throw new ArgumentNullException("executeAction");

            _executeAction = executeAction;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteFunction == null)
                return true;
            else
                return _canExecuteFunction(parameter);
        }

        public void Execute(object parameter)
        {
            if (_executeAction == null)
                throw new InvalidOperationException("ExecuteAction must not be null.");
            _executeAction(parameter);
        }

        public virtual void OnCanExecuteChanged()
        {
            EventHandler del = CanExecuteChanged;
            if (del != null)
                del.Invoke(this, new EventArgs());
        }
    }
}
