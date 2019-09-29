using System.ComponentModel;
using Gtk;

namespace MVVMSharp.Test.TestData
{
    internal class TestWidget : IWidget
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
    }
}