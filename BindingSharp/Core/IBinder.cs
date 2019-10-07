namespace Binding.Core
{
    internal interface IBinder 
    {
        void Bind(object target, string property);
    }

    internal interface IBinder<T>
    {
        void Bind(T target, string property);
    }
}