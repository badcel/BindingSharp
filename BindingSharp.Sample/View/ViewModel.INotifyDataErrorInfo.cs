using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;

namespace Binding.Samples
{
    public partial class ViewModel : INotifyDataErrorInfo
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private List<string> errors = new List<string>();

        public bool HasErrors{ get; private set;} 

        private void ToggleError(string property, string error)
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

        public IEnumerable GetErrors(string propertyName)
        {
            return errors;
        }

        protected void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}