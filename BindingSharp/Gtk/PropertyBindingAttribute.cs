using System;

namespace Binding.Gtk
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PropertyBindingAttribute : Attribute
    {
        public string WidgetProperty { get; }
        public string ViewModelProperty { get; }

        public PropertyBindingAttribute(string widgetProperty, string viewModelProperty)
        {
            this.WidgetProperty = widgetProperty ?? throw new ArgumentNullException(nameof(widgetProperty));
            this.ViewModelProperty = viewModelProperty ?? throw new ArgumentNullException(nameof(viewModelProperty));
        }
    }
}