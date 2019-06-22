using System;
using System.Windows.Input;

namespace MVVM 
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action<object> execute;
        private bool canExecute;

        public Command(Action<object> execute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            canExecute = true;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }
        public void SetCanExecute(bool value)
        {
            if(value != canExecute)
            {
                canExecute = value;
                OnCanExecuteChanged();
            }
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}