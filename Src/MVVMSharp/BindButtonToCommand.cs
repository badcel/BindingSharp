using System;
using System.Windows.Input;
using Gtk;

namespace MVVMSharp.Gtk
{
    public class BindButtonToCommand : IBinder, IDisposable
    {
        private readonly IButton button;
        private ICommand command;

        public BindButtonToCommand(IButton button)
        {
            this.button = button ?? throw new ArgumentNullException(nameof(button));
        }

        public void Bind(object viewModel, string commandPropertyName)
        {
            if(viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            var property = viewModel.GetType().GetProperty(commandPropertyName);

            if(property == null)
                throw new BindingException(button, viewModel, $"Property {commandPropertyName} is not a property of viewmodel.");

            if(property.PropertyType != typeof(ICommand))
                throw new BindingException(button, viewModel, $"Property {commandPropertyName} is not an ICommand.");

            command = (ICommand) property.GetValue(viewModel);
            command.CanExecuteChanged += OnCommandCanExectueChanged;

            button.Clicked += OnButtonBlicked;
            button.Sensitive = command.CanExecute(null);
        }

        private void OnButtonBlicked(object sender, EventArgs args)
        {
            if(command != null)
            {
                command.Execute(null);
            }
        }

        private void OnCommandCanExectueChanged(object sender, EventArgs args)
        {
            button.Sensitive = command.CanExecute(null);
        }

        #region IDisposable Support
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
        #endregion
    }
}