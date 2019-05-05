using System;
using Gtk;

namespace MVVM
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();
            
            var app = new Application("org.GTK.GTK", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new Window("MEIN FENSTER");
            win.DeleteEvent += (o, argss) => Application.Quit();
            app.AddWindow(win);

            /*var contentControl = new ContentControl();
            contentControl.SetContent(new CustomControlViewModel());
            win.Add(contentControl);
            contentControl.Show();*/

            win.Add(new CustomControl());

            win.ShowAll();
            Application.Run();
        }
    }
}
