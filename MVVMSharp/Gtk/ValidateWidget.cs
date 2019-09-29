using System;
using System.ComponentModel;
using Gtk;
using MVVMSharp.Core;

namespace MVVMSharp.Gtk
{
    public class ValidateWidget : IBinder, IBinder<INotifyDataErrorInfo>
    {
        private readonly INotifyDataErrorInfo notifyDataErrorInfo;
        private readonly IWidget widget;
        private string propertyName;

        public ValidateWidget(IWidget widget)
        {
            this.widget = widget ?? throw new System.ArgumentNullException(nameof(widget));

        }

        public void Bind(INotifyDataErrorInfo target, string property)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            if(property == null)
                throw new ArgumentNullException(nameof(property));

            propertyName = property;

            target.ErrorsChanged += OnErrorsChanged;
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            //if(e.PropertyName == propertyName)
                ;//TODO
        }

        public void Bind(object target, string property)
        {
            

            var targetProperty = target.GetType().GetProperty(property);

            if(targetProperty == null)
                throw new BindingException(target, $"Property {property} is not a property of {nameof(target)}.");

            //if(target is )
            
        }
    }
}