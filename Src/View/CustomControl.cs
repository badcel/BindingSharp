using System;
using GLib;
using Gtk;

namespace MVVM
{
    [TypeName(nameof(CustomControl))]
    [Template("CustomControl.glade")]
    public class CustomControl : Bin, IView
    {
        [Child]
        public Button Button;

        public CustomControl()
        {
        }

        private void button_clicked(object obj, EventArgs args)
        {
            Button.Label = "Fubar";
        } 

        public void SetupBindings(object viewModel)
        {

        }
    }
}