using System;
using GLib;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace MVVM
{
    [Template("CustomControl.glade")]
    public class CustomControl : Widget, IView
    {
        private readonly BindingBuilder builder;

        [Child]
        private Button Button;

        [Child]
        private Label Label;

        public CustomControl()
        {
            //Button.Clicked += (o, args) => Label.LabelProp = "FUBAR";
        }
        public void SetupBindings(object viewModel)
        {
            
        }
    }
}