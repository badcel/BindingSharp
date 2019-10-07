using System.ComponentModel;
using Gtk;

namespace Binding.Test.TestData
{
    public class TestWidget : IWidget
    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { PropertyChangedEventAdded = true; }
            remove { PropertyChangedEventRemoved = true; }
        }

        public bool PropertyChangedEventAdded;
        public bool PropertyChangedEventRemoved;

        public bool TestBool { get; set; }
        public bool Sensitive { get; set; }

        public IStyleContext StyleContext => null;
    }
}