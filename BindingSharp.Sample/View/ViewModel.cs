using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;

namespace Binding.Samples
{
    public class ViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private List<string> errors;

        private Command myCommand;
        public ICommand MyCommand
        {
            get { return myCommand; }    
        }

        private Command myCommand2;
        public ICommand MyCommand2
        {
            get { return myCommand2; }    
        }

        private string label = "MEIN LABEL NEU";
        public string Label
        {
            get { return label; }
            set
            {
                if(label != value)
                {
                    label = value;
                    Console.WriteLine("VIEWMODEL: " + value);

                    label = value + value;
                    Console.WriteLine("CHanged " + label);
                    OnPropertyChanged();
                }

            }
        }

        public bool HasErrors{ get; private set;} 

        public ViewModel()
        {
            myCommand = new Command((o) => ButtonAction());
            myCommand2 = new Command((o) => ButtonAction2());

            errors = new List<string>();
        }

        private void ButtonAction2()
        {
            AddError(nameof(MyCommand2), "Label defekt");
        }

        private void AddError(string property, string error)
        {
            if(HasErrors)
            {
                errors.Clear();
            }
            else
            {
                errors.Add(error);
            }
            HasErrors = !HasErrors;
            
            OnErrorsChanged(property);
        }

        private void ButtonAction()
        {
            Console.WriteLine("ViewModel: Button clicked");
            Label = "Test";
            myCommand.SetCanExecute(false);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return errors;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}