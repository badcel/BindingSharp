using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Gtk;

namespace MVVMSharp.Gtk
{
    public static class WidgetExtension
    {
        private static Dictionary<IWidget, HashSet<IBinder>>  commandBindings = new Dictionary<IWidget, HashSet<IBinder>>();

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

        private static Func<INotifyPropertyChanged, string, IBinder> propertyBindingProvider;
        public static Func<INotifyPropertyChanged, string, IBinder> PropertyBindingProvider
        {
            [ExcludeFromCodeCoverage]
            get
            {
                if(propertyBindingProvider == null)
                    propertyBindingProvider =  NotifyPropertyChangedBindingProvider;

                return propertyBindingProvider;
            }
            internal set { propertyBindingProvider = value; }
        }

        [ExcludeFromCodeCoverage]
        private static IBinder GtkCommandBindingProvider(IButton b)
        {
            return new BindToCommand(b);
        }

        [ExcludeFromCodeCoverage]
        private static IBinder NotifyPropertyChangedBindingProvider(INotifyPropertyChanged n, string propertyName)
        {
            return new BindToProperty(n, propertyName);
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
                BindCommand(view, viewModel, viewField);
            }
        }

        private static void BindProperty(IWidget view, object viewModel, FieldInfo viewField)
        {
            var viewFieldBindingAttrs = viewField.GetCustomAttributes(typeof(PropertyBindingAttribute), false);

            if (viewFieldBindingAttrs.Length == 0)
                return;
            if(!(view is INotifyPropertyChanged np))
                throw new Exception();

            //TODO: PropertyBinding needs Properties, but commands are fields
            var bindToProperty = PropertyBindingProvider(np, viewField.Name);
            bindToProperty.Bind(viewModel, )
        }

        private static void BindCommand(IWidget view, object viewModel, FieldInfo viewField)
        {
            var viewFieldBindingAttrs = viewField.GetCustomAttributes(typeof(CommandBindingAttribute), false);

            if (viewFieldBindingAttrs.Length == 0)
                return;

            if(!typeof(IButton).IsAssignableFrom(viewField.FieldType))
                throw new BindingException(viewModel, $"View Field type of field {viewField.Name} must be assignable to {nameof(IButton)}!");

            var bindToCommand = CommandBindingProvider((IButton) viewField.GetValue(view));
            bindToCommand.Bind(viewModel, ((CommandBindingAttribute) viewFieldBindingAttrs[0]).CommandProperty);

            if(!commandBindings.ContainsKey(view))
                commandBindings[view] = new HashSet<IBinder>();

            commandBindings[view].Add(bindToCommand);
        }
    }
}