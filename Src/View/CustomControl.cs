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
            Button.Clicked += (o, args) => Button.Label = "FUBAR";
        }
        public void SetupBindings(object viewModel)
        {

        }
    }
}