using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVM
{
    public class ViewModelControlViewModel : IViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Type View => typeof(ViewModelControl);

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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}