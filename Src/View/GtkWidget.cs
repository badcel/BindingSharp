using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace MVVM
{
    public class GtkWidget : Box
    {
        [UI]
        private Button Button;

        [UI]
        private Label Label;

        public GtkWidget() : this(new Builder("CustomControlObject.glade")) { }

        private GtkWidget(Builder builder) : base(builder.GetObject("Box").Handle)
        {
            builder.Autoconnect(this);
        }
    }
}