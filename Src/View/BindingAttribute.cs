using System;

namespace MVVM
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindingAttribute : Gtk.Builder.ObjectAttribute
    {
        public string Source { get; }
        public string Target { get; }

        public BindingAttribute(string source, string target)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }
    }
}