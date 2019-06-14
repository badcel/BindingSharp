using System;

namespace MVVM
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindingAttribute : Gtk.ChildAttribute
    {
        public string ViewProperty { get; }
        public string ViewModelProperty { get; }

        public BindingAttribute(string viewProperty, string viewModelProperty)
        {
            ViewProperty = viewProperty ?? throw new ArgumentNullException(nameof(viewProperty));
            ViewModelProperty = viewModelProperty ?? throw new ArgumentNullException(nameof(viewModelProperty));
        }
    }
}