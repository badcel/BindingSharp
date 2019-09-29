# VVM-Sharp # 

VVM-Sharp enables MVVM-Style programming with [GTKSharp][]. It is a library to bind properties of a GTK widget to a viewmodel. 

If you have a .NET Standard 2.0 or .NET CORE application you can port it's view to [GTK][] and reuse the rest of your code to be deployed on Windows, Linux and MacOs with differnt native UI toolkits.

[GtkSharp]: https://github.com/GtkSharp/GtkSharp
[Gtk]: https://gtk.org

## Features ##
 * Binds properties of a [GTk.Widget][Gtk.Widget] to a viewmodel with a one-way or two-way binding via the [INotifyPropertyChanged][] interface
 * Special binding for a [Gtk.Button][] which can be bound to an [ICommand][]
 * Planned: Support validation of widgets via the [INotifyDataErrorInfo][] interface

 [Gtk.Widget]: https://google.de
 [Gtk.Button]: https://google.de
 [ICommand]: https://google.de
 [INotifyPropertyChanged]: https://google.de
 [INotifyDataErrorInfo]: https://google.de

## Installing ##
Add the [nuget package][1] as a reference to your project with the dotnet command

    # dotnet add packages ....

[1]: https://google.de

## Using ##
To use the binding the application must provide the viewmodel to the view to be able to create the binding inside the view.

### Preparation ###
use helper user controls


### Create the View ###
1. Create a view class with a matching glade file which describes the user interface as XML. Inside your view reference some UI widgets in fields. For working examples see the [samples][] of GtkSharp.
2. Add the _PropertyBindingAttribute_ or _CommandBindingAttribute_ to a widget of your UI
2. Add a dependency in your view's constructor to expect a viewmodel
3. Call _BindViewModel(object viewmodel)_ in your view's constructor to setup the binding
    
        public class MyWidget : Box
        {
            ...

            [UI]
            [CommandBinding(nameof(ViewModelClass.MyCommand))]
            private Button MyButton;

            [UI]
            [PropertyBinding(nameof(Label.LabelProp), nameof(ViewModelClass.MyLabelText))]
            private Label MyLabel;

            public MyWidget(object viewmodel) : this(new Builder("MyWidget.glade")) 
            { 
                this.BindViewModel(viewmodel)
            }

            ...
        }
[samples]: https://google.de

## License ##
LGPL