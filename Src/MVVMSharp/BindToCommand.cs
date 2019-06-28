using System;
using System.Windows.Input;
using Gtk;

namespace MVVMSharp
{
    public class BindToCommand
    {
        private readonly IButton button;

        public BindToCommand(IButton button)
        {
            this.button = button ?? throw new ArgumentNullException(nameof(button));
        }

        public void Bind(object viewModel, string commandPropertyName)
        {
            if(viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            var property = viewModel.GetType().GetProperty(commandPropertyName);

            if(property == null)
                throw new BindingException(viewModel, $"Property {commandPropertyName} is not a property of viewmodel.");

            if(property.PropertyType != typeof(ICommand))
                throw new BindingException(viewModel, $"Property {commandPropertyName} is not an ICommand.");

            var command = (ICommand) property.GetValue(viewModel);
            button.Sensitive = command.CanExecute(null);
        }
    }
}