using System;

namespace MVVMSharp.Gtk
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyBindingAttribute : Attribute
    {
        public string Property { get; }

        public PropertyBindingAttribute(string property)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }
    }
}