using System.Reflection;
using GLib;
using Gtk;

namespace MVVM
{
    public class BindingBuilder : Builder
    {
        //Use CompositeWidgets: https://blogs.gnome.org/tvb/2013/04/09/announcing-composite-widget-templates/
        //https://blogs.gnome.org/tvb/2013/05/29/composite-templates-lands-in-vala/
        public BindingBuilder(string resource_name) : base(Assembly.GetCallingAssembly().GetManifestResourceStream(resource_name))
        {
        }

        public void Autoconnect(object view, object viewModel)
        {
            this.Autoconnect(view);
            Bind(view, viewModel);
        }

        private void Bind(object view, object viewModel)
        {
            var flags = System.Reflection.BindingFlags.Public;
            flags |= System.Reflection.BindingFlags.NonPublic;
            flags |= System.Reflection.BindingFlags.DeclaredOnly;
            flags |= System.Reflection.BindingFlags.Instance;

            var fields = view.GetType().GetFields(flags);

            foreach (var field in fields)
            {
                var bindingAttrs = field.GetCustomAttributes(typeof(BindingAttribute), false);

                if (bindingAttrs == null || bindingAttrs.Length == 0)
                    continue;

                var obj = field.GetValue(view);
                if (obj is GLib.Object gobject)
                {
                    var bindingAttr = (BindingAttribute)bindingAttrs[0];

                    var bindingSrcProp = gobject.GetType().GetProperty(bindingAttr.Source);
                    var bindingSrcPropPropAttr = bindingSrcProp.GetCustomAttributes(typeof(PropertyAttribute), false);

                    if (bindingSrcPropPropAttr == null || bindingSrcPropPropAttr.Length == 0)
                        continue;

                    var srcPropName = ((PropertyAttribute)bindingSrcPropPropAttr[0]).Name;

                    gobject.AddNotification(srcPropName, (o, args) => NotifyViewModel(o, args.Property, viewModel, bindingAttr.Target));
                }
            }
        }

        private void NotifyViewModel(object source, string sourceProp, object target, string targetProp)
        {
            if (source is GLib.Object gobject)
            {
                var value = gobject.GetProperty(sourceProp);
                if (value is GLib.Value gvalue)
                {
                    var prop = target.GetType().GetProperty(targetProp);
                    prop.SetValue(target, gvalue.Val);
                }
            }
        }
    }
}