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

        [Callback]
        private void button_clicked(Button button)
        {
            button.Label = "Fubar";
        } 

        public void SetupBindings(object viewModel)
        {

        }
    }
}