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
        [Child]
        public Button Button;

        public CustomControl()
        {
            //Button.Clicked += (o, args) => Label.LabelProp = "FUBAR";
            /*var b = new Button();
            b.Label = "TEST";
            this.Add(b);*/
        }
        public void SetupBindings(object viewModel)
        {

        }
    }
}