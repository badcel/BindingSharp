using System;
using Gtk;

namespace MVVM
{
    public class ContentControl : Bin
    {
        public void SetContent(IViewModel viewModel)
        {
            var instance = Activator.CreateInstance(viewModel.View);

            if(instance is IView view)
                view.SetupBindings(viewModel);

            if(instance is Widget widget)
            {
                this.Add(widget);
                widget.Show();
            }
        }
    }
}