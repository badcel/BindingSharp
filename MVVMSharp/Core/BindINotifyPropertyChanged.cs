using System;
using System.ComponentModel;
using System.Reflection;

namespace MVVMSharp.Core
{
    public class BindINotifyPropertyChanged : IBinder, IBinder<INotifyPropertyChanged>, IDisposable
    {
        private object target;
        private PropertyInfo targetPropertyInfo;

        private INotifyPropertyChanged source;
        private PropertyInfo sourcePropertyInfo;


        public BindINotifyPropertyChanged(INotifyPropertyChanged source, string property)
        {
            this.source = source ?? throw new System.ArgumentNullException(nameof(source));
            this.sourcePropertyInfo = source.GetType().GetProperty(property);

            if(sourcePropertyInfo == null)
                throw new BindingException(source, $"Property {property} is not a property of {nameof(source)}.");
        }

        public void Bind(INotifyPropertyChanged target, string property)
        {
            Bind((object)target, property);
        }

        public void Bind(object target, string property)
        {

            if(target == null)
                throw new ArgumentNullException(nameof(target));

            targetPropertyInfo = target.GetType().GetProperty(property);

            if(targetPropertyInfo == null)
                throw new BindingException(target, $"Property {property} is not a property of {nameof(target)}.");

            this.target = target;
            if(target is INotifyPropertyChanged targetNotify)
                targetNotify.PropertyChanged += OnTargetPropertyChanged;

            source.PropertyChanged += OnSourcePropertyChanged;
        }

        protected void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == sourcePropertyInfo.Name)
            {
                var value = sourcePropertyInfo.GetValue(sender);
                targetPropertyInfo.SetValue(target, value);
            }
        }

        protected void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == targetPropertyInfo.Name)
            {
                var value = targetPropertyInfo.GetValue(sender);
                sourcePropertyInfo.SetValue(source, value);
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
                    if(source != null) source.PropertyChanged -= OnSourcePropertyChanged;
                    if(target != null && target is INotifyPropertyChanged vmNotify) vmNotify.PropertyChanged -= OnTargetPropertyChanged;
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