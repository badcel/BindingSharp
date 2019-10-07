using System;

namespace Binding.Gtk
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ValidationBindingAttribute : Attribute
    {
        public string Property { get; }

        public ValidationBindingAttribute(string property)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }
    }
}