using System;
using GLib;
using Gtk;

namespace MVVM
{
    [TypeName(nameof(ViewModelControl))]
    [Template("ViewModelControl.glade")]
    public class ViewModelControl : Box
    {
        [Binding(nameof(Gtk.Label.LabelProp), nameof(ViewModelControlViewModel.Label))]
        public Label Label;
        

        public ViewModelControl(object viewModel)
        {
            this.BindViewModel(viewModel);
        }

        private void button_clicked(object obj, EventArgs args)
        {
            Label.LabelProp = "aha";
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(disposing)
            {
                this.Unbind();
            }
        }
    }
}