using System;
using System.IO;
using System.Reflection;
using GLib;
using GtkLib = Gtk;
using Binding.Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Binding.Samples
{
    public class View : GtkLib.Box
    {
        [UI]
        [PropertyBinding(nameof(GtkLib.Label.LabelProp), nameof(ViewModel.Label))]
        public GtkLib.Label Label;

        [UI]
        [CommandBinding(nameof(ViewModel.ChangeLabelCommand))]
        public GtkLib.Button ChangeLabelButton;

        [UI]
        [ValidationBinding(nameof(ViewModel.ToggleErrorCommand))]
        [CommandBinding(nameof(ViewModel.ToggleErrorCommand))]
        public GtkLib.Button ToggleErrorButton;

        public View(object viewModel) : this(new GtkLib.Builder("View.glade"))
        {
            this.Bind(viewModel);
        }

        private View(GtkLib.Builder builder) : base(builder.GetObject("View").Handle)
        {
            builder.Autoconnect(this);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                this.DisposeBindings();
            }
            base.Dispose(disposing);
        }
    }
}