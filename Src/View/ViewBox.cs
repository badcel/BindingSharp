using System;
using System.Collections.Generic;
using System.ComponentModel;
using GLib;
using Gtk;

namespace MVVM
{
    [TypeName(nameof(ViewBox))]
    public class ViewBox : Box
    {
        protected readonly object viewModel;

        private Dictionary<object, Dictionary<string, string>> viewBindings; //{Gobject, {GObject Property, ViewModelProperty}}
        private Dictionary<string, Dictionary<object, HashSet<string>>> viewModelBindings; //{ViewModelProperty, {GObject, List<GObject Property>}}

        public ViewBox(object viewModel)
        {
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            viewBindings = new Dictionary<object, Dictionary<string, string>> ();
            viewModelBindings = new Dictionary<string, Dictionary<object, HashSet<string>>>();

            Bind(this, viewModel);
        }

        private void Bind(object view, object viewModel)
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
            }
        }

        private void AddViewModelToViewNotification(string viewModelProperty, GLib.Object gObject, string viewProperty)
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

        private void AddViewToViewModelNotification(GLib.Object viewFieldGObject, string viewProperty, string viewModelProperty)
        {
            if(!viewBindings.ContainsKey(viewFieldGObject))
            {
                viewBindings[viewFieldGObject] = new Dictionary<string ,string>(){{viewProperty, viewModelProperty}};
                viewFieldGObject.AddNotification(NotifyViewModel);
            }
            else
            {
                viewBindings[viewFieldGObject][viewProperty] = viewModelProperty;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
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

        private void NotifyViewModel(object source, NotifyArgs args)
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(disposing)
            {
                foreach(var obj in viewBindings.Keys)
                {
                    if(obj is GLib.Object gobject)
                    {
                        gobject.RemoveNotification(NotifyViewModel);
                    }
                }

                if(viewModel is INotifyPropertyChanged notifyPropertyChanged)
                {
                    notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
                }
            }
        }
    }
}