using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using GLib;
using Gtk;

namespace MVVM
{
    /*public static class WidgetExtension
    {
        private static Dictionary<Widget, List<PropertyBinding>> propertyBindings = new Dictionary<Widget, List<PropertyBinding>>();

        private static Dictionary<Widget, List<CommandBinding>>  commandBindings = new Dictionary<Widget, List<CommandBinding>>();

        public static void BindViewModel(this Gtk.Widget view, object viewModel)
        {
            var flags = System.Reflection.BindingFlags.Public;
            flags |= System.Reflection.BindingFlags.NonPublic;
            flags |= System.Reflection.BindingFlags.DeclaredOnly;
            flags |= System.Reflection.BindingFlags.Instance;

            var viewFields = view.GetType().GetFields(flags);

            foreach (var viewField in viewFields)
            {
                BindProperty(view, viewModel, viewField);
                BindCommand(view, viewModel, viewField);
            }
        }

        #region BindCommand

        private static void BindCommand(Gtk.Widget view, object viewModel, FieldInfo viewField)
        {
            var viewFieldBindingAttrs = viewField.GetCustomAttributes(typeof(CommandBindingAttribute), false);

            if (viewFieldBindingAttrs == null || viewFieldBindingAttrs.Length == 0)
                return;

            var bindingAttr = (CommandBindingAttribute) viewFieldBindingAttrs[0];
            var viewModelCommandProperty = viewModel.GetType().GetProperty(bindingAttr.CommandProperty);

            if(viewModelCommandProperty == null)
            {
                Console.Error.WriteLine($"Could not find property {bindingAttr.CommandProperty} on viewmodel.");
                return;
            }
            var viewModelCommand = viewModelCommandProperty.GetValue(viewModel);
            if(!(viewModelCommand is ICommand))
            {
                Console.Error.WriteLine($"Viewmodel property {bindingAttr.CommandProperty} is not an ICommand");
                return;
            }

            var obj = viewField.GetValue(view);         
            if(obj is Gtk.Button button)
            {
                var command = (ICommand) viewModelCommand;
                command.CanExecuteChanged += OnCanExecuteChanged;

                button.Sensitive = command.CanExecute(null);
                button.Clicked += OnButtonClicked;

                if(!commandBindings.ContainsKey(view))
                {
                    commandBindings[view] = new List<CommandBinding>();
                }
                commandBindings[view].Add(new CommandBinding(button, command));
            }
            else
            {
                Console.WriteLine($"View field {viewField.Name} is not a gtk button");
            }
        }

        private static void OnButtonClicked(object sender, EventArgs e)
        {
            var executeBindings = commandBindings.SelectMany(x => x.Value).Where(x => x.Button == sender);

            foreach(var executeBinding in executeBindings)
            {
                executeBinding.Command.Execute(null);
            }
        }

        private static void OnCanExecuteChanged(object obj, EventArgs args)
        {
            var executeBindings = commandBindings.SelectMany(x => x.Value).Where(x => x.Command == obj);

            foreach(var executeBinding in executeBindings)
            {
                var canExecute = executeBinding.Command.CanExecute(null);
                executeBinding.Button.Sensitive = canExecute;
            }
        }
        #endregion BindCommand

        #region BindProperty

        private static void BindProperty(Gtk.Widget view, object viewModel, FieldInfo viewField)
        {
            var viewFieldBindingAttrs = viewField.GetCustomAttributes(typeof(PropertyBindingAttribute), false);

            if (viewFieldBindingAttrs == null || viewFieldBindingAttrs.Length == 0)
                return;

            var obj = viewField.GetValue(view);
            if (obj is GLib.Object viewFieldGObject)
            {
                var bindingAttr = (PropertyBindingAttribute)viewFieldBindingAttrs[0];

                var viewModelProp = viewModel.GetType().GetProperty(bindingAttr.ViewModelProperty);
                if(viewModelProp == null)
                {
                    Console.Error.WriteLine($"Could not bind '{viewField.Name}' to viewmodel property '{bindingAttr.ViewModelProperty}'. '{bindingAttr.ViewModelProperty}' is no property of the viewmodel.");
                    return;
                }

                var viewModelPropValue = viewModelProp.GetValue(viewModel);

                var bindingViewProp = viewFieldGObject.GetType().GetProperty(bindingAttr.ViewProperty);
                if(bindingViewProp == null)
                {
                    Console.Error.WriteLine($"Could not bind '{viewField.Name}' to viewmodel property '{bindingAttr.ViewModelProperty}'. '{bindingAttr.ViewProperty}' is no property of the view.");
                    return;
                }

                var bindingViewPropAttr = bindingViewProp.GetCustomAttributes(typeof(PropertyAttribute), false);
                if (bindingViewPropAttr == null || bindingViewPropAttr.Length == 0)
                {
                    Console.Error.WriteLine($"Could not bind field '{viewField.Name}' to viewmodel property '{bindingAttr.ViewModelProperty}'. '{bindingAttr.ViewProperty}' is no gobject property of field '{viewField.Name}'.");
                    return;
                }
                
                var viewProperty = ((PropertyAttribute)bindingViewPropAttr[0]).Name;
                viewFieldGObject.SetProperty(viewProperty, new GLib.Value(viewModelPropValue));
                
                AddBinding(view, new PropertyBinding(viewFieldGObject, viewProperty, viewModel, bindingAttr.ViewModelProperty));
            }
        }

        private static void AddBinding(Widget widget, PropertyBinding binding)
        {
            if(!propertyBindings.ContainsKey(widget))
            {
                propertyBindings[widget] = new List<PropertyBinding>();   
            }            
            propertyBindings[widget].Add(binding);

            if(binding.ViewModel is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
            }
            else
            {
                Console.Error.WriteLine($"Can not forward notify property changed events because viewModel does not implement {nameof(INotifyPropertyChanged)}");
            }

            if(binding.View is GLib.Object gObject)
            {
                gObject.AddNotification(binding.ViewProp, NotifyViewModel);
            }
            else
            {
                Console.Error.WriteLine("Could not register for changes on the view");
            }
        }

        private static void NotifyViewModel(object source, NotifyArgs args)
        {
            var sourceProp = args.Property;

            var bindings = propertyBindings.SelectMany(x => x.Value).Where(x => x.View == source && x.ViewProp == sourceProp);

            foreach(var binding in bindings)
            {
                if (binding.View is GLib.Object sourceGObject)
                {
                    var value = sourceGObject.GetProperty(sourceProp);
                    if (value is GLib.Value gvalue)
                    {
                        var viewModelProp = binding.ViewModel.GetType().GetProperty(binding.ViewModelProp);
                        if(viewModelProp != null)
                        {
                            viewModelProp.SetValue(binding.ViewModel, gvalue.Val);
                        }
                        else
                        {
                            Console.Error.WriteLine($"Could not find property {binding.ViewModelProp} on viewmodel type {binding.ViewModel.GetType().FullName}");
                        }
                    }
                }
            }
        }

        private static void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModelPropertyName = e.PropertyName;
            
            var value = sender.GetType().GetProperty(viewModelPropertyName).GetValue(sender);

            var bindings = propertyBindings.SelectMany(x => x.Value).Where(x => x.ViewModel == sender && x.ViewModelProp == viewModelPropertyName);

            foreach(var binding in bindings)
            {
                if(binding.View is GLib.Object gObject)
                {
                    var glibValue = new GLib.Value(value);
                    gObject.SetProperty(binding.ViewProp, glibValue);
                } 
            }
        }

        #endregion BindProperty

        public static void Unbind(this Gtk.Widget view)
        {
            UnbindProperties(view);
            UnbindProperties(view);
        }

        private static void UnbindCommands(Gtk.Widget view)
        {
            foreach(var executeBinding in commandBindings[view])
            {
                executeBinding.Button.Clicked -= OnButtonClicked;
                executeBinding.Command.CanExecuteChanged -= OnCanExecuteChanged;
            }

            commandBindings.Remove(view);
        }

        private static void UnbindProperties(Gtk.Widget view)
        {
            foreach(var binding in propertyBindings[view])
            {
                if(binding.View is GLib.Object gObject)
                {
                    gObject.RemoveNotification(binding.ViewProp, NotifyViewModel);
                }

                if(binding.ViewModel is INotifyPropertyChanged notifyPropertyChanged)
                {
                    notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
                }
            }

            propertyBindings.Remove(view);
        }
        
    }*/
}