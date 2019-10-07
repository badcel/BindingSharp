using System;

namespace Binding.Gtk
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandBindingAttribute : Attribute
    {
        public string CommandProperty { get; }

        public CommandBindingAttribute(string commandProperty)
        {
            CommandProperty = commandProperty ?? throw new ArgumentNullException(nameof(commandProperty));
        }
    }
}