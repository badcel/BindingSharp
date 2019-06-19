using System;
using System.Collections.Generic;
using System.ComponentModel;
using GLib;
using Gtk;

namespace MVVM
{
    public class ViewBinding
    {
        public object ViewField { get; }

        private Dictionary<string, string> propertyBindings;

        public ViewBinding(object viewField)
        {
            ViewField = viewField;
            propertyBindings = new Dictionary<string, string>();
        }

        public void AddBinding(string viewFieldPropertyName, string viewModelPropertyName)
        {
            propertyBindings[viewFieldPropertyName] = viewModelPropertyName;
        }

        public string GetBinding(string viewFieldPropertyName)
        {
            if(propertyBindings.ContainsKey(viewFieldPropertyName))
                return propertyBindings[viewFieldPropertyName];
            else
                return null;
        }
    }

    public class ViewPropertyBinding
    {
        public string ViewFieldPropertyName { get; }
        public string ViewModelPropertyName { get; }

        public ViewPropertyBinding(string viewFieldPropertyName, string viewModelPropertyName)
        {
            ViewFieldPropertyName = viewFieldPropertyName;
            ViewModelPropertyName = viewModelPropertyName;
        }
    }

    public class ViewModelBinding
    {
        public string ViewModelPropertyName { get; }
        public object ViewField { get; }

        private HashSet<string> viewFieldPropertyNames;
        public IEnumerable<string> ViewFieldPropertyNames => viewFieldPropertyNames;

        public ViewModelBinding(string viewModelPropertyName, object viewField, string viewFieldPropertyName)
        {
            ViewModelPropertyName = viewModelPropertyName;
            ViewField = viewField;
            viewFieldPropertyNames = new HashSet<string>(){viewFieldPropertyName};
        }

        public bool AddViewFieldPropertyName(string propertyName)
        {
            return viewFieldPropertyNames.Add(propertyName);
        }
    }

    public static class WidgetExtension
    {
        private static Dictionary<Widget, HashSet<ViewBinding>> viewBindings = new Dictionary<Widget, HashSet<ViewBinding>>();
        private static Dictionary<Widget, ViewModelBinding>  viewModelBindings = new Dictionary<Widget, ViewModelBinding>();

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
                    
                    //Notification in both directions contains risk of endless loop!
                    AddViewToViewModelNotification(viewFieldGObject, viewProperty, bindingAttr.ViewModelProperty);
                    AddViewModelToViewNotification(bindingAttr.ViewModelProperty, viewFieldGObject, viewProperty);
                }

                if(viewModel is INotifyPropertyChanged notifyPropertyChanged)
                {
                    notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
                }
                else
                {
                    Console.Error.WriteLine($"Can not forwared notify property changed events because viewModel does not implement {nameof(INotifyPropertyChanged)}");
                }
            }
        }

        private static void AddViewModelToViewNotification(string viewModelProperty, GLib.Object gObject, string viewProperty)
        {
            if(!viewModelBindings.ContainsKey(viewModelProperty))
            {
                viewModelBindings[viewModelProperty] = new Dictionary<object, HashSet<string>>();
            }

            if(!viewModelBindings[viewModelProperty].ContainsKey(gObject))
            {
                viewModelBindings[viewModelProperty][gObject] = new HashSet<string>(){viewProperty};
            }
            else
            {
                viewModelBindings[viewModelProperty][gObject].Add(viewProperty);
            }
        }

        private static void AddViewToViewModelNotification(Widget widget, GLib.Object viewFieldGObject, string viewProperty, string viewModelProperty)
        {
            if(!viewBindings.ContainsKey(widget))
            {
                viewBindings[widget] = new HashSet<ViewBinding>();   
            }

            if(viewBindings[widget].)
            
            var viewBinding = new ViewBinding(viewFieldGObject);
                viewBinding.AddBinding(viewProperty, viewModelProperty);
                viewFieldGObject.AddNotification(NotifyViewModel);

        }

        private static void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModelPropertyName = e.PropertyName;
            if(!viewModelBindings.ContainsKey(viewModelPropertyName))
                return;

            var value = sender.GetType().GetProperty(viewModelPropertyName).GetValue(sender);

            foreach(var keyValue in viewModelBindings[viewModelPropertyName])
            {
                if(keyValue.Key is GLib.Object gObject)
                {
                    foreach(var viewPropertyName in keyValue.Value)
                    {
                        var glibValue = new GLib.Value(value);
                        gObject.SetProperty(viewPropertyName, glibValue);
                    }
                } 
            }
        }

        private static void NotifyViewModel(object source, NotifyArgs args)
        {
            var sourceProp = args.Property;

            if(!viewBindings.ContainsKey(source))
                return;

            if(!viewBindings[source].ContainsKey(sourceProp))
                return;

            if (source is GLib.Object gobject)
            {
                var value = gobject.GetProperty(sourceProp);
                if (value is GLib.Value gvalue)
                {
                    var viewModelPropertyName = viewBindings[source][sourceProp];
                    var viewModelProp = viewModel.GetType().GetProperty(viewModelPropertyName);

                    if(viewModelProp != null)
                    {
                        viewModelProp.SetValue(viewModel, gvalue.Val);
                    }
                    else
                    {
                        Console.Error.WriteLine($"Could not find property {viewModelPropertyName} on viewmodel type {viewModel.GetType().FullName}");
                    }
                }
            }
        }
    }
}