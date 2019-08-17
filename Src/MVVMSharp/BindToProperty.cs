using System;
using System.ComponentModel;
using System.Reflection;

namespace MVVMSharp.Gtk
{
    public class BindToProperty : IBinder, IDisposable
    {

        private object viewModel;
        private PropertyInfo viewModelPropertyInfo;

        private INotifyPropertyChanged view;
        private string viewProperty;


        public BindToProperty(INotifyPropertyChanged view, string property)
        {
            this.view = view ?? throw new System.ArgumentNullException(nameof(view));
            this.viewProperty = property ?? throw new System.ArgumentNullException(nameof(property));
        }

        public void Bind(object viewModel, string commandPropertyName)
        {
            this.viewModel = viewModel;

            if(viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            viewModelPropertyInfo = viewModel.GetType().GetProperty(commandPropertyName);

            if(viewModelPropertyInfo == null)
                throw new BindingException(viewModel, $"Property {commandPropertyName} is not a property of viewmodel.");

            if(!(viewModel is INotifyPropertyChanged))
                throw new BindingException(viewModel, $"ViewModel does not implement {nameof(INotifyPropertyChanged)}");

            view.PropertyChanged += OnViewPropertyChanged;
        }

        protected void OnViewPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var value = sender.GetType().GetProperty(args.PropertyName).GetValue(sender);
            viewModelPropertyInfo.SetValue(viewModel, value);
        }

       #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(view != null) view.PropertyChanged -= OnViewPropertyChanged;
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