using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Gtk;
using Binding.Core;

namespace Binding.Gtk
{
    public static class WidgetExtension
    {
        private static Dictionary<IWidget, HashSet<IBinder>>  bindings = new Dictionary<IWidget, HashSet<IBinder>>();

        #region Providers

        private static IStyleProvider styleProvider;
        public static IStyleProvider StyleProvider
        {
            [ExcludeFromCodeCoverage]
            get
            {
                if(styleProvider == null)
                    styleProvider = GetCssProvider();

                return styleProvider;
            }
            internal set { styleProvider = value; }            
        }

        private static Func<IStyleContext, string, IBinder> styleContextBindingProvider;
        public static Func<IStyleContext, string, IBinder> StyleContextBindingProvider
        {
            [ExcludeFromCodeCoverage]
            get
            {
                if(styleContextBindingProvider == null)
                    styleContextBindingProvider =  GtkStyleContextBindingProvider;

                return styleContextBindingProvider;
            }
            internal set { styleContextBindingProvider = value; }
        }

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
        private static CssProvider GetCssProvider()
        {
            var provider = new CssProvider();

            using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("invalid.css"))
            using(var reader = new StreamReader(stream))
            {
                provider.LoadFromData(reader.ReadToEnd());   
            }

            return provider;
        }

        [ExcludeFromCodeCoverage]
        private static IBinder GtkStyleContextBindingProvider(IStyleContext styleContext, string cssClasName)
        {
            return new BindStyleContextToNotifyDataErrorInfo(styleContext, cssClasName);
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
        #endregion Providers

        public static void Bind(this IWidget view, object viewModel)
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

                if(Attribute.IsDefined(viewField, typeof(ValidationBindingAttribute)))
                    BindValidation(view, viewModel, viewField);
            }
        }

        private static T GetViewFieldAs<T>(IWidget view, FieldInfo viewField)
        {
            if(!typeof(T).IsAssignableFrom(viewField.FieldType))
                throw new Exception("??");

            return (T) viewField.GetValue(view);
        }

        private static T GetViewFieldAttribute<T>(FieldInfo viewField)
        {
            var viewFieldBindingAttrs = viewField.GetCustomAttributes(typeof(T), false);

            if (viewFieldBindingAttrs.Length == 0)
                return default(T);
            else
                return (T)viewFieldBindingAttrs[0];
        }

        private static void BindValidation(IWidget view, object viewModel, FieldInfo viewField)
        {
            var attribute = GetViewFieldAttribute<ValidationBindingAttribute>(viewField);
            if(attribute != null)
            {
                var widget = GetViewFieldAs<IWidget>(view, viewField);
                var styleContext = widget.StyleContext;
                styleContext.AddProvider(StyleProvider, uint.MaxValue);

                var binder = StyleContextBindingProvider(styleContext, "invalid");
                Bind(binder, view, viewModel, attribute.Property);
            }
        }

        private static void BindProperty(IWidget view, object viewModel, FieldInfo viewField)
        {
            var attribute = GetViewFieldAttribute<PropertyBindingAttribute>(viewField);
            if(attribute != null)
            {
                var widget = GetViewFieldAs<IWidget>(view, viewField);
                var binder = WidgetBindingProvider(widget, attribute.WidgetProperty);
                Bind(binder, view, viewModel, attribute.ViewModelProperty);
            }
        }

        private static void BindCommand(IWidget view, object viewModel, FieldInfo viewField)
        {
            var attribute = GetViewFieldAttribute<CommandBindingAttribute>(viewField);
            if(attribute != null)
            {
                var button = GetViewFieldAs<IButton>(view, viewField);
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
        /// Call this if disposing the widget
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