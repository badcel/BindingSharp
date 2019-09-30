using System;
using System.IO;
using System.Reflection;
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

        [CommandBinding(nameof(ViewModelControlViewModel.MyCommand2))]
        [UI]
        public Button Button2;

        public ViewModelControl(object viewModel) : this(new Builder("ViewModelControl.glade"))
        {
            this.BindViewModel(viewModel);

            Button2.Clicked += Restyle;
        }

        private void Restyle(object sender, EventArgs args)
        {
            var p = new CssProvider();

            using(var stream = Assembly.GetEntryAssembly().GetManifestResourceStream("invalid.css"))
            using(var reader = new StreamReader(stream))
            {
                p.LoadFromData(reader.ReadToEnd());   
            }

            IStyleContext sc = Button2.StyleContext;
            sc.AddProvider(p, uint.MaxValue);
            sc.AddClass("invalid");
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