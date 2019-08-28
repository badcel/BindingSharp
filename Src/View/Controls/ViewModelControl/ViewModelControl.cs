using System;
using GLib;
using Gtk;
using MVVMSharp.Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace MVVM
{
    public class ViewModelControl : Box
    {
        [PropertyBinding(nameof(Gtk.Label.LabelProp), nameof(ViewModelControlViewModel.Label))]
        [UI]
        public Label Label;

        [CommandBinding(nameof(ViewModelControlViewModel.MyCommand))]
        [UI]
        public Button Button;

        public ViewModelControl(object viewModel) : this(new Builder("ViewModelControl.glade"))
        {
            this.BindViewModel(viewModel);
        }

        private ViewModelControl(Builder builder) : base(builder.GetObject("ViewModelControl").Handle)
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