using System;
using System.ComponentModel;
using Gtk;

namespace MVVMSharp.Test.TestData
{
    internal class TestButton : IButton
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
    }
}