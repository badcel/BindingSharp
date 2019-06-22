using System;

namespace MVVM
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandBindingAttribute : Gtk.ChildAttribute
    {
        public string CommandProperty { get; }

        public CommandBindingAttribute(string commandProperty)
        {
            CommandProperty = commandProperty ?? throw new ArgumentNullException(nameof(commandProperty));
        }
    }
}