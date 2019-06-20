using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GLib;
using Gtk;

namespace MVVM
{
    public class Binding
    {
        public object View { get; }
        public string ViewProp { get; }
        public object ViewModel { get; }
        public string ViewModelProp { get; }

        public Binding(object view, string viewProp, object viewModel, string viewModelProp)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
            ViewProp = viewProp ?? throw new ArgumentNullException(nameof(viewProp));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            ViewModelProp = viewModelProp ?? throw new ArgumentNullException(nameof(viewModelProp));
        }
    }

    public static class WidgetExtension
    {
        private static Dictionary<Widget, List<Binding>> viewBindings = new Dictionary<Widget, List<Binding>>();
        private static Dictionary<object, List<Binding>>  viewModelBindings = new Dictionary<object, List<Binding>>();

        public static void BindViewModel(this Gtk.Widget view, object viewModel)
        {
            var flags = System.Reflection.BindingFlags.Public;
            flags |= System.Reflection.BindingFlags.NonPublic;
            flags |= System.Reflection.BindingFlags.DeclaredOnly;
            flags |= System.Reflection.BindingFlags.Instance;

            var viewFields = view.GetType().GetFields(flags);

            foreach (var viewField in viewFields)
            {
                var viewFieldBindingAttrs = viewField.GetCustomAttributes(typeof(BindingAttribute), false);

                if (viewFieldBindingAttrs == null || viewFieldBindingAttrs.Length == 0)
                    continue;

                var obj = viewField.GetValue(view);
                if (obj is GLib.Object viewFieldGObject)
                {
                    var bindingAttr = (BindingAttribute)viewFieldBindingAttrs[0];

                    var viewModelProp = viewModel.GetType().GetProperty(bindingAttr.ViewModelProperty);
                    if(viewModelProp == null)
                    {
                        Console.Error.WriteLine($"Could not bind '{viewField.Name}' to viewmodel property '{bindingAttr.ViewModelProperty}'. '{bindingAttr.ViewModelProperty}' is no property of the viewmodel.");
                        continue;
                    }

                    var viewModelPropValue = viewModelProp.GetValue(viewModel);

                    var bindingViewProp = viewFieldGObject.GetType().GetProperty(bindingAttr.ViewProperty);
                    if(bindingViewProp == null)
                    {
                        Console.Error.WriteLine($"Could not bind '{viewField.Name}' to viewmodel property '{bindingAttr.ViewModelProperty}'. '{bindingAttr.ViewProperty}' is no property of the view.");
                        continue;
                    }

                    var bindingViewPropAttr = bindingViewProp.GetCustomAttributes(typeof(PropertyAttribute), false);
                    if (bindingViewPropAttr == null || bindingViewPropAttr.Length == 0)
                    {
                        Console.Error.WriteLine($"Could not bind field '{viewField.Name}' to viewmodel property '{bindingAttr.ViewModelProperty}'. '{bindingAttr.ViewProperty}' is no gobject property of field '{viewField.Name}'.");
                        continue;
                    }
                    
                    var viewProperty = ((PropertyAttribute)bindingViewPropAttr[0]).Name;
                    viewFieldGObject.SetProperty(viewProperty, new GLib.Value(viewModelPropValue));
                    
                    AddBinding(view, new Binding(viewFieldGObject, viewProperty, viewModel, bindingAttr.ViewModelProperty));
                }
            }
        }

        public static void Unbind(this Gtk.Widget view)
        {
            foreach(var binding in viewBindings[view])
            {
                if(binding.View is GLib.Object gObject)
                {
                    gObject.RemoveNotification(binding.ViewProp, NotifyViewModel);
                }

                var curViewModelBindings = viewModelBindings[binding.ViewModel];
                curViewModelBindings.Remove(binding);

                if(!curViewModelBindings.Any() && binding.ViewModel is INotifyPropertyChanged notifyPropertyChanged)
                {
                    notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
                }
            }

            viewBindings.Remove(view);
        }

        private static void AddBinding(Widget widget, Binding binding)
        {
            if(!viewBindings.ContainsKey(widget))
            {
                viewBindings[widget] = new List<Binding>();   
            }            
            viewBindings[widget].Add(binding);

            if(!viewModelBindings.ContainsKey(binding.ViewModel))
            {
                viewModelBindings[binding.ViewModel] = new List<Binding>();

                if(binding.ViewModel is INotifyPropertyChanged notifyPropertyChanged)
                {
                    notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
                }
                else
                {
                    Console.Error.WriteLine($"Can not forward notify property changed events because viewModel does not implement {nameof(INotifyPropertyChanged)}");
                }
            }
            viewModelBindings[binding.ViewModel].Add(binding);

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

            var bindings = viewBindings.SelectMany(x => x.Value).Where(x => x.View == source && x.ViewProp == sourceProp);

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

            var bindings = viewBindings.SelectMany(x => x.Value).Where(x => x.ViewModel == sender && x.ViewModelProp == viewModelPropertyName);

            foreach(var binding in bindings)
            {
                if(binding.View is GLib.Object gObject)
                {
                    var glibValue = new GLib.Value(value);
                    gObject.SetProperty(binding.ViewProp, glibValue);
                } 
            }
        }
    }
}