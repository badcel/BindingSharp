using GLib;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace MVVM
{
    public class CustomControl : Box, IView
    {
        [UI]
        [Binding(nameof(Gtk.Button.Label), "Button")]
        private Button Button;

        [UI]
        [Binding(nameof(Gtk.Label.Text), "Label")]
        private Label Label;

        public CustomControl() : this(new Builder("CustomControl.glade")) { }

        public CustomControl(Builder builder): base(builder.GetObject("Box").Handle)
        {
            builder.Autoconnect(this);
        }

        public void SetupBindings(object viewModel)
        {
            var flags = System.Reflection.BindingFlags.Public;
                flags |= System.Reflection.BindingFlags.NonPublic;
                flags |= System.Reflection.BindingFlags.DeclaredOnly;
				flags |= System.Reflection.BindingFlags.Instance;

            var fields = GetType().GetFields(flags);

            foreach(var field in fields)
            {
                var attrs = field.GetCustomAttributes(typeof(BindingAttribute), false);
    
                if (attrs == null || attrs.Length == 0)
                    continue;

                var gtkObject = field.GetValue(this);
                if(gtkObject is GLib.Object gobject)
                {
                    var attr = (BindingAttribute) attrs[0];
                    gobject.AddNotification(attr.Source, (o, args) => NotifyViewModel(o, args.Property, viewModel, attr.Target));
                }
            }
        }

        private void NotifyViewModel(object source, string sourceProp, object target, string targetProp)
        {
            //TODO
        }
    }
}