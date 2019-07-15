using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Gtk;

namespace MVVMSharp.Gtk
{
    public static class WidgetExtension
    {
        private static Dictionary<IWidget, HashSet<IBindToCommand>>  commandBindings = new Dictionary<IWidget, HashSet<IBindToCommand>>();

        private static Func<IButton, IBindToCommand> commandBindingProvider;
        public static Func<IButton, IBindToCommand> CommandBindingProvider
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

        [ExcludeFromCodeCoverage]
        private static IBindToCommand GtkCommandBindingProvider(IButton b)
        {
            return new BindToCommand(b);
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
                commandBindings[view] = new HashSet<IBindToCommand>();

            commandBindings[view].Add(bindToCommand);
        }
    }
}