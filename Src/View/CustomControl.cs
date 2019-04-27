using System;
using GLib;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace MVVM
{
    public class CustomControl : Box, IView
    {
        [UI]
        private Button Button;

        [UI]
        [Binding(nameof(Gtk.Label.LabelProp), "Label")]
        private Label Label;

        public CustomControl() : this(new Builder("CustomControl.glade")) { }

        public CustomControl(Builder builder): base(builder.GetObject("Box").Handle)
        {
            builder.Autoconnect(this);

            Button.Clicked += (o, args) => Label.LabelProp = "FUBAR";
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

                    var t = gobject.GetType();
                    var p = t.GetProperty(attr.Source);
                    var pattr = p.GetCustomAttributes(typeof(PropertyAttribute), false);

                    if(pattr == null || pattr.Length == 0)
                        continue;

                    var src = ((PropertyAttribute) pattr[0]).Name;

                    gobject.AddNotification(src, (o, args) => NotifyViewModel(o, args.Property, viewModel, attr.Target));
                }
            }
        }

        private void NotifyViewModel(object source, string sourceProp, object target, string targetProp)
        {
            
            if(source is GLib.Object gobject)
            {
                var value = gobject.GetProperty(sourceProp);
                if(value is GLib.Value gvalue)
                Console.WriteLine(gvalue.Val.ToString());
                //TODO
            }
        }
    }
}