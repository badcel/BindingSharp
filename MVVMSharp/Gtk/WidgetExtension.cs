using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Gtk;
using MVVMSharp.Core;

namespace MVVMSharp.Gtk
{
    public static class WidgetExtension
    {
        private static Dictionary<IWidget, HashSet<IBinder>>  bindings = new Dictionary<IWidget, HashSet<IBinder>>();

        private static Func<IButton, IBinder> commandBindingProvider;
        public static Func<IButton, IBinder> CommandBindingProvider
        {
            [ExcludeFromCodeCoverage]
            get
            {
                if(commandBindingProvider == null)
                    commandBindingProvider =  GtkCommandBindingProvider;

                return commandBindingProvider;
            }
            internal set { commandBindingProvider = value; }
        }

        private static Func<IWidget, string, IBinder> widgetBindingProvider;
        public static Func<IWidget, string, IBinder> WidgetBindingProvider
        {
            [ExcludeFromCodeCoverage]
            get
            {
                if(widgetBindingProvider == null)
                    widgetBindingProvider =  GtkWidgetBindingProvider;

                return widgetBindingProvider;
            }
            internal set { widgetBindingProvider = value; }
        }

        [ExcludeFromCodeCoverage]
        private static IBinder GtkCommandBindingProvider(IButton b)
        {
            return new BindButtonToCommand(b);
        }

        [ExcludeFromCodeCoverage]
        private static IBinder GtkWidgetBindingProvider(IWidget widget, string propertyName)
        {
            return new BindINotifyPropertyChanged(widget, propertyName);
        }

        public static void BindViewModel(this IWidget view, object viewModel)
        {
            var flags = System.Reflection.BindingFlags.Public;
            flags |= System.Reflection.BindingFlags.NonPublic;
            flags |= System.Reflection.BindingFlags.DeclaredOnly;
            flags |= System.Reflection.BindingFlags.Instance;

            var viewFields = view.GetType().GetFields(flags);

            foreach (var viewField in viewFields)
            {
                if(Attribute.IsDefined(viewField, typeof(CommandBindingAttribute)))
                    BindCommand(view, viewModel, viewField);

                if(Attribute.IsDefined(viewField, typeof(PropertyBindingAttribute)))
                    BindProperty(view, viewModel, viewField);
            }
        }

        private static T GetViewPropertyAs<T>(IWidget view, FieldInfo viewField)
        {
            if(!typeof(T).IsAssignableFrom(viewField.FieldType))
                throw new Exception("??");

            return (T) viewField.GetValue(view);
        }

        private static T GetViewPropertyAttribute<T>(FieldInfo viewField)
        {
            var viewFieldBindingAttrs = viewField.GetCustomAttributes(typeof(T), false);

            if (viewFieldBindingAttrs.Length == 0)
                return default(T);
            else
                return (T)viewFieldBindingAttrs[0];
        }

        private static void BindProperty(IWidget view, object viewModel, FieldInfo viewField)
        {
            var attribute = GetViewPropertyAttribute<PropertyBindingAttribute>(viewField);
            if(attribute != null)
            {
                var widget = GetViewPropertyAs<IWidget>(view, viewField);
                var binder = WidgetBindingProvider(widget, attribute.WidgetProperty);
                Bind(binder, view, viewModel, attribute.ViewModelProperty);
            }
        }

        private static void BindCommand(IWidget view, object viewModel, FieldInfo viewField)
        {
            var attribute = GetViewPropertyAttribute<CommandBindingAttribute>(viewField);
            if(attribute != null)
            {
                var button = GetViewPropertyAs<IButton>(view, viewField);
                var binder = CommandBindingProvider(button);
                Bind(binder, view, viewModel, attribute.CommandProperty);
            }
        }

        private static void Bind(IBinder binder, IWidget view, object viewModel, string propertyName)
        {
            binder.Bind(viewModel, propertyName);
            CacheBinder(view, binder);
        }

        private static void CacheBinder(IWidget view, IBinder binder)
        {
            if(!bindings.ContainsKey(view))
                bindings[view] = new HashSet<IBinder>();

            bindings[view].Add(binder);
        }

        ///<summary>
        /// Call this before disposing the widget
        //</summary>
        public static void DisposeBindings(this IWidget view)
        {
            if(bindings.ContainsKey(view))
            {
                foreach(var binder in bindings[view])
                {
                    if(binder is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                bindings.Remove(view);
            }
        }
    }
}