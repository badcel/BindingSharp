using System;
using System.Diagnostics.CodeAnalysis;

namespace MVVMSharp
{
    public class BindingException : Exception
    {
        public object ViewModel { [ExcludeFromCodeCoverage] get; }

        public object View { [ExcludeFromCodeCoverage] get; }

        public BindingException(object view, object viewmodel, string message) : base(message)
        {
            ViewModel = viewmodel;
            View = view;
        }
    }
}