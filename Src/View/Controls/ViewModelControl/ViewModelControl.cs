using System;
using GLib;
using Gtk;

namespace MVVM
{
    [TypeName(nameof(ViewModelControl))]
    [Template("ViewModelControl.glade")]
    public class ViewModelControl : Box
    {
        [PropertyBinding(nameof(Gtk.Label.LabelProp), nameof(ViewModelControlViewModel.Label))]
        public Label Label;

        [CommandBinding(nameof(ViewModelControlViewModel.MyCommand))]
        public Button Button;        

        public ViewModelControl(object viewModel)
        {
            this.BindViewModel(viewModel);
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