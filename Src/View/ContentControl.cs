using System;
using Gtk;

namespace MVVM
{
    public class ContentControl : Bin
    {
        public void SetContent(IViewModel viewModel)
        {
            var view = Activator.CreateInstance(viewModel.View, viewModel);

            if(view is Widget widget)
            {
                this.Add(widget);
                widget.ShowAll();
            }
        }
    }
}