namespace MVVMSharp.Gtk
{
    public interface IBinder 
    {
        void Bind(object viewModel, string commandPropertyName);
    }
}