using System;
using System.Windows.Input;
using Gtk;
using Binding.Core;

namespace Binding.Gtk
{
    internal partial class BindButtonToCommand : IDisposable
    {
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(button != null) button.Clicked -= OnButtonBlicked;
                    if(command != null) command.CanExecuteChanged -= OnCommandCanExectueChanged;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}