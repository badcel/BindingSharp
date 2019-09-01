using System;
using System.ComponentModel;
using System.Reflection;
using Gtk;

namespace MVVMSharp.Gtk
{
    public class BindWidgetToProperty : IBinder, IBinder<INotifyPropertyChanged>, IDisposable
    {
        private object viewModel;
        private PropertyInfo viewModelPropertyInfo;

        private IWidget widget;
        private PropertyInfo widgetPropertyInfo;


        public BindWidgetToProperty(IWidget widget, string property)
        {
            this.widget = widget ?? throw new System.ArgumentNullException(nameof(widget));
            this.widgetPropertyInfo = widget.GetType().GetProperty(property);

            if(widgetPropertyInfo == null)
                throw new BindingException(null, widget, $"Property {property} is not a property of widget.");
        }

        public void Bind(INotifyPropertyChanged viewModel, string commandPropertyName)
        {
            Bind((object)viewModel, commandPropertyName);
        }

        public void Bind(object viewModel, string commandPropertyName)
        {

            if(viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            viewModelPropertyInfo = viewModel.GetType().GetProperty(commandPropertyName);

            if(viewModelPropertyInfo == null)
                throw new BindingException(widget, viewModel, $"Property {commandPropertyName} is not a property of viewmodel.");

            this.viewModel = viewModel;
            if(viewModel is INotifyPropertyChanged vmNotify)
                vmNotify.PropertyChanged += OnViewModelPropertyChanged;

            widget.PropertyChanged += OnWidgetPropertyChanged;
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
                    if(viewModel != null && viewModel is INotifyPropertyChanged vmNotify) vmNotify.PropertyChanged -= OnViewModelPropertyChanged;
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