using System;
using System.IO;
using System.Reflection;
using GLib;
using GtkLib = Gtk;
using MVVMSharp.Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace MVVMSharp.Samples
{
    public class View : GtkLib.Box
    {
        [ValidationBinding(nameof(ViewModel.Label))]
        [PropertyBinding(nameof(GtkLib.Label.LabelProp), nameof(ViewModel.Label))]
        [UI]
        public GtkLib.Label Label;

        [CommandBinding(nameof(ViewModel.MyCommand))]
        [UI]
        public GtkLib.Button Button;

        [ValidationBinding(nameof(ViewModel.MyCommand2))]
        [CommandBinding(nameof(ViewModel.MyCommand2))]
        [UI]
        public GtkLib.Button Button2;

        public View(object viewModel) : this(new GtkLib.Builder("View.glade"))
        {
            this.BindViewModel(viewModel);
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