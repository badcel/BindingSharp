namespace MVVMSharp.Gtk
{
    public interface IBindToCommand 
    {
        void Bind(object viewModel, string commandPropertyName);
    }
}