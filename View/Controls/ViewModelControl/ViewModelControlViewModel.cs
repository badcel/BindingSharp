using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MVVM
{
    public class ViewModelControlViewModel : IViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Type View => typeof(ViewModelControl);

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

        public ViewModelControlViewModel()
        {
            myCommand = new Command((o) => ButtonAction());
            myCommand2 = new Command((o) => ButtonAction2());
        }

        private void ButtonAction2()
        {
            Console.WriteLine("Action2");
        }

        private void ButtonAction()
        {
            Console.WriteLine("ViewModel: Button clicked");
            Label = "Test";
            myCommand.SetCanExecute(false);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}