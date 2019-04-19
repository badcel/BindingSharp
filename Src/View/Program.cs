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
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
