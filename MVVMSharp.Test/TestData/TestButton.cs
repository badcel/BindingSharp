using System;
using System.ComponentModel;
using Gtk;

namespace Binding.Test.TestData
{
    public class TestButton : IButton
    {
        public event PropertyChangedEventHandler PropertyChanged {add {} remove {}}

        public event EventHandler Clicked
        {
            add { ClickedEventWasAdded = true; }
            remove { ClickedEventWasRemoved = true; }
        }

        public bool ClickedEventWasRemoved;
        public bool ClickedEventWasAdded;

        public bool Sensitive { get; set;}

        public IStyleContext StyleContext => null;
    }
}