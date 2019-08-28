namespace MVVMSharp.Gtk
{
    public interface IBinder 
    {
        void Bind(object viewModel, string commandPropertyName);
    }

    public interface IBinder<T>
    {
        void Bind(T viewModel, string property);
    }
}