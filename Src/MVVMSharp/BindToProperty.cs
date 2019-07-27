namespace MVVMSharp.Gtk
{
    public class BindToProperty
    {
        private object viewModel;
        private string property;

        public BindToProperty(object viewModel, string property)
        {
            this.viewModel = viewModel ?? throw new System.ArgumentNullException(nameof(viewModel));
            this.property = property ?? throw new System.ArgumentNullException(nameof(property));
        }
    }
}