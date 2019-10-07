using System;
using System.ComponentModel;
using System.Reflection;

namespace Binding.Core
{
    internal partial class BindINotifyPropertyChanged : IDisposable
    {
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
    }
}