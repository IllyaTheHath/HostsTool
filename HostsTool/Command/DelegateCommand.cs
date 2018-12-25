using System;
using System.Windows.Input;

namespace HostsTool.Command
{
    public class DelegateCommand : ICommand
    {
        public Action<Object> ExecuteAction { get; set; }
        public Func<Object, Boolean> CancelExecute { get; set; }

        public event EventHandler CanExecuteChanged;

        public Boolean CanExecute(Object parameter)
        {
            return CancelExecute == null ? true : CancelExecute(parameter);
        }

        public void Execute(Object parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public DelegateCommand() : this(null, null)
        {
        }

        public DelegateCommand(Action<Object> execute) : this(execute, null)
        {
        }

        public DelegateCommand(Action<Object> execute, Func<Object, Boolean> canExecute)
        {
            ExecuteAction = execute;
            CancelExecute = canExecute;
        }
    }
}
