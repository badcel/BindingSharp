using System;
using Gtk;

namespace MVVMSharp.Samples
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();
            
            var app = new Application("org.GTK.GTK", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new Window("Sample app");
            win.DeleteEvent += (o, argss) => Application.Quit();
            app.AddWindow(win);

            var viewModel = new ViewModel();
            var view = new View(viewModel);

            win.Add(view);
            win.ShowAll();
            Application.Run();
        }
    }
}
