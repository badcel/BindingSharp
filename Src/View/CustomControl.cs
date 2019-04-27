using System;
using GLib;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace MVVM
{
    public class CustomControl : Box, IView
    {
        private readonly BindingBuilder builder;

        [UI]
        private Button Button;

        [Binding(nameof(Gtk.Label.LabelProp), "Label")]
        private Label Label;

        public CustomControl() : this(new BindingBuilder("CustomControl.glade")) { }

        public CustomControl(BindingBuilder builder) : base(builder.GetObject("Box").Handle)
        {
            this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public void SetupBindings(object viewModel)
        {
            builder.Autoconnect(this, viewModel);

            Button.Clicked += (o, args) => Label.LabelProp = "FUBAR";
        }
    }
}