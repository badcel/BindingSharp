using System;
using GLib;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace MVVM
{
    [Template("CustomControl.glade")]
    public class CustomControl : Box, IView
    {
        private readonly BindingBuilder builder;

        [Child]
        private Button Button;

        [Child]
        private Label Label;

        public CustomControl()
        {

        }

        public void SetupBindings(object viewModel)
        {
            //builder.Autoconnect(this, viewModel);

            Button.Clicked += (o, args) => Label.LabelProp = "FUBAR";
        }
    }
}