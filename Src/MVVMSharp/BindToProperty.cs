using System;
using System.ComponentModel;
using System.Reflection;

namespace MVVMSharp.Gtk
{
    public class BindToProperty : IBinder, IDisposable
    {

        private INotifyPropertyChanged viewModel;
        private PropertyInfo viewModelPropertyInfo;

        private INotifyPropertyChanged view;
        private PropertyInfo viewPropertyInfo;


        public BindToProperty(INotifyPropertyChanged view, string property)
        {
            this.view = view ?? throw new System.ArgumentNullException(nameof(view));
            this.viewPropertyInfo = view.GetType().GetProperty(property);

            if(viewPropertyInfo == null)
                throw new BindingException(view, $"Property {property} is not a property of view.");
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

            view.PropertyChanged += OnViewPropertyChanged;
            this.viewModel = vmNotify;
            this.viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        protected void OnViewPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var value = sender.GetType().GetProperty(args.PropertyName).GetValue(sender);
            viewModelPropertyInfo.SetValue(viewModel, value);
        }

        protected void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var value = sender.GetType().GetProperty(args.PropertyName).GetValue(sender);
            viewPropertyInfo.SetValue(view, value);
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