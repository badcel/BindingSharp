using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;

namespace Binding.Samples
{
    public partial class ViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public Command ChangeLabelCommand { get; }

        public Command ToggleErrorCommand { get; }

        private string label;
        public string Label
        {
            get { return label; }
            set
            {
                if(label != value)
                {
                    label = value;
                    OnPropertyChanged();
                }
            }
        }

        public ViewModel()
        {
            ChangeLabelCommand = new Command((o) => ChangeLabelText());
            ToggleErrorCommand = new Command((o) => ToggleError());
        }

        private void ToggleError()
        {
            ToggleError(nameof(ToggleErrorCommand), "Error");
        }

        private void ChangeLabelText()
        {
            Label = "New label text";
            ChangeLabelCommand.SetCanExecute(false);
        }
    }
}