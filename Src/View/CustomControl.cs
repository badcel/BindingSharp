using System;
using GLib;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace MVVM
{
    [TypeName("CustomControl")]
    [Template("CustomControl.glade")]
    public class CustomControl : Bin, IView
    {
        [Child(isInternal: false)]
        public Button Button;

        public CustomControl()
        {
            //Button.Clicked += (o, args) => Label.LabelProp = "FUBAR";
        }
        public void SetupBindings(object viewModel)
        {
            
        }
    }
}