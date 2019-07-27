using System;
using Gtk;

namespace MVVMSharp.Test
{
    internal class TestButton : IButton
    {
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