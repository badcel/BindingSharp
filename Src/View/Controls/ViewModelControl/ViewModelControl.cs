using System;
using GLib;
using Gtk;

namespace MVVM
{
    [TypeName(nameof(ViewModelControl))]
    [Template("ViewModelControl.glade")]
    public class ViewModelControl : ViewBox
    {
        [Binding(nameof(Gtk.Label.LabelProp), nameof(ViewModelControlViewModel.Label))]
        public Label Label;
        

        public ViewModelControl(object viewModel) : base(viewModel)
        {
        }

        private void button_clicked(object obj, EventArgs args)
        {
            Label.LabelProp = "aha";
        } 
    }
}