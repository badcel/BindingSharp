using System;
using System.Diagnostics.CodeAnalysis;

namespace MVVMSharp.Core
{
    public class BindingException : Exception
    {
        public object Object { [ExcludeFromCodeCoverage] get; }

        public BindingException(object obj, string message) : base(message)
        {
            Object = obj;
        }
    }
}