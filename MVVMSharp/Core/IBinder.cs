namespace MVVMSharp.Core
{
    public interface IBinder 
    {
        void Bind(object target, string property);
    }

    public interface IBinder<T>
    {
        void Bind(T target, string property);
    }
}