# BindingSharp # 

> Note: As GtkSharp will probably not gain support for `INotifyPropertyChanged` this project will be rebased on [gir.core](https://github.com/gircore/gir.core) as it's successor as soon as there are Nuget packages available.

BindingSharp enables MVVM-Style programming with [GTKSharp][]. It is a library to bind properties of a GTK widget to a viewmodel. 

If you have a .NET Standard 2.0 or .NET CORE application you can port it's view to [GTK][] and reuse the rest of your code to be deployed on Windows, Linux and MacOs with differnt native UI toolkits.

[GtkSharp]: https://github.com/GtkSharp/GtkSharp
[Gtk]: https://gtk.org

## Features ##
 * Binds properties of a [GTk.Widget][] to a viewmodel with a one-way or two-way binding via the [INotifyPropertyChanged][] interface
 * Special binding for a [Gtk.Button][] which can be bound to an [ICommand][]
 * Supports binding of a [GTk.Widget][] to a property of the viewmodel to support validation via the [INotifyDataErrorInfo][] interface (still work in progress)

 [Gtk.Widget]: https://developer.gnome.org/gtk3/stable/GtkWidget.html
 [Gtk.Button]: https://developer.gnome.org/gtk3/stable/GtkButton.html
 [ICommand]: https://docs.microsoft.com/de-de/dotnet/api/system.windows.input.icommand?view=netstandard-2.0
 [INotifyPropertyChanged]: https://docs.microsoft.com/de-de/dotnet/api/system.componentmodel.inotifypropertychanged?view=netstandard-2.0
 [INotifyDataErrorInfo]: https://docs.microsoft.com/de-de/dotnet/api/system.componentmodel.inotifydataerrorinfo?view=netstandard-2.0

## Using ##
To use the binding the application must provide the viewmodel to the view to be able to create the binding inside the view.

For a complete sample see the [Sample App](BindingSharp.Sample).

1. Create a view class with a matching glade file which describes the user interface as XML. Inside your view reference some UI widgets in fields. For working examples see the [templates][] of GtkSharp.
2. Add the _PropertyBindingAttribute_ or _CommandBindingAttribute_ or _ValidationBindingAttribute_ to a widget of your UI
3. Call _Bind(object obj)_ in your view's constructor to setup the binding
    
        public class MyWidget : Box
        {
            ...

            [UI]
            [CommandBinding(nameof(ViewModelClass.MyCommand))]
            private Button MyButton;

            public MyWidget(object viewmodel) : this(new Builder("MyWidget.glade")) 
            { 
                this.Bind(viewmodel)
            }

            ...
        }
[templates]: https://github.com/GtkSharp/GtkSharp/tree/master/Source/Templates/GtkSharp.Template.CSharp/content

## Building from source ##

---
**Note**

Currently GtkSharp is missing the support for _INotifyPropertyChanged_ on _GLib.Object_.

There is a [pull request][1] to add this feature. Until the pull request is merged you can use a custom version from GtkSharp from [my temporary fork][2].

To compile the code checkout the GTKSharp fork and put it parallel to this repository (see references in _*.csproj_ files)

---

There are 3 projects inside the repository:
 - **BindingSharp:** Project source
 - **BindingSharp.Sample:** Example gtk application
 - **BindingSharp.Test:** Unit tests

 To build the source code run `dotnet build` in the corresponding project folder.

 To run the sample app execute `dotnet run` in the BindingSharp.Sample folder.
 
 To test the code run `dotnet test` in the BindingSharp.Test folder.

[1]: https://github.com/GtkSharp/GtkSharp/pull/103
[2]: https://github.com/badcel/GtkSharp/tree/InterfaceSupport

## License ##
BindingSharp and its related components are licensed under [LGPL v2.1 license](LICENSE).
