using System;

namespace MVVMSharp
{
    public class BindingException : Exception
    {
        public object Target { get; }
        public BindingException(object target, string message) : base(message)
        {
            Target = target;
        }
    }
}