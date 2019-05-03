using System;
using GLib;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace MVVM
{
    [Template("CustomControl.glade")]
    public class CustomControl : Bin, IView
    {
        private readonly BindingBuilder builder;

        [Child(isInternal: false)]
        private Button Button;

        public CustomControl()
        {
            //Button.Clicked += (o, args) => Label.LabelProp = "FUBAR";
        }
        public void SetupBindings(object viewModel)
        {
            
        }
    }
}