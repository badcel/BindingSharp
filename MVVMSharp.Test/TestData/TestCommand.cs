using System;
using System.Windows.Input;

namespace MVVMSharp.Test.TestData
{
    public class TestCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CanExecuteChangedWasAdded = true; }
            remove { CanExecuteChangedWasRemoved = true; }
        }

        public bool CanExecuteChangedWasAdded;
        public bool CanExecuteChangedWasRemoved;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
        }
    }
}