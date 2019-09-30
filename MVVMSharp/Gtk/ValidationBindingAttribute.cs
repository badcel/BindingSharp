using System;

namespace MVVMSharp.Gtk
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