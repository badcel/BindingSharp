using System;
using System.Collections.Generic;
using System.Reflection;
using Gtk;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("MVVMSharp.Test")]

namespace MVVMSharp.Gtk
{
    public static class WidgetExtension
    {
        private static Dictionary<IWidget, List<IBindToCommand>>  commandBindings = new Dictionary<IWidget, List<IBindToCommand>>();

        private static Func<IButton, IBindToCommand> getCommandBinding;
        public static Func<IButton, IBindToCommand> GetCommandBinding
        {
            get
            {
                if(getCommandBinding == null)
                    getCommandBinding = CreateGtkCommandBinding;

                return getCommandBinding;
            }
            set { getCommandBinding = value; }
        }

        private static IBindToCommand CreateGtkCommandBinding(IButton button)
        {
            return new BindToCommand(button);
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

            if (viewFieldBindingAttrs == null || viewFieldBindingAttrs.Length == 0)
                return;

            if(!typeof(IButton).IsAssignableFrom(viewField.FieldType))
                throw new BindingException(viewModel, $"View Field type of field {viewField.Name} must be assignable to {nameof(IButton)}!");

            var bla = GetCommandBinding((IButton) viewField.GetValue(view));
            bla.Bind(viewModel, ((CommandBindingAttribute) viewFieldBindingAttrs[0]).CommandProperty);

            if(!commandBindings.ContainsKey(view))
                commandBindings[view] = new List<IBindToCommand>();

            commandBindings[view].Add(bla);
        }
    }
}