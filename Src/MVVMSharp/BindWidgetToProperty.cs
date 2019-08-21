using System;
using System.ComponentModel;
using System.Reflection;
using Gtk;

namespace MVVMSharp.Gtk
{
    public class BindWidgetToProperty : IBinder, IDisposable
    {

        private INotifyPropertyChanged viewModel;
        private PropertyInfo viewModelPropertyInfo;

        private IWidget widget;
        private PropertyInfo widgetPropertyInfo;


        public BindWidgetToProperty(IWidget widget, string property)
        {
            this.widget = widget ?? throw new System.ArgumentNullException(nameof(widget));
            this.widgetPropertyInfo = widget.GetType().GetProperty(property);

            if(widgetPropertyInfo == null)
                throw new BindingException(widget, $"Property {property} is not a property of widget.");
        }

        public void Bind(object viewModel, string commandPropertyName)
        {

            if(viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            viewModelPropertyInfo = viewModel.GetType().GetProperty(commandPropertyName);

            if(viewModelPropertyInfo == null)
                throw new BindingException(viewModel, $"Property {commandPropertyName} is not a property of viewmodel.");

            if(!(viewModel is INotifyPropertyChanged vmNotify))
                throw new BindingException(viewModel, $"ViewModel does not implement {nameof(INotifyPropertyChanged)}");

            widget.PropertyChanged += OnWidgetPropertyChanged;
            this.viewModel = vmNotify;
            this.viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        protected void OnWidgetPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == widgetPropertyInfo.Name)
            {
                var value = widgetPropertyInfo.GetValue(sender);
                viewModelPropertyInfo.SetValue(viewModel, value);
            }
        }

        protected void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == viewModelPropertyInfo.Name)
            {
                var value = viewModelPropertyInfo.GetValue(sender);
                widgetPropertyInfo.SetValue(widget, value);
            }
        }

       #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(widget != null) widget.PropertyChanged -= OnWidgetPropertyChanged;
                    if(viewModel != null) viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}