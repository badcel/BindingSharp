using System.ComponentModel;

namespace MVVMSharp.Test
{
    public class TestView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { PropertyChangedEventAdded = true; }
            remove { PropertyChangedEventRemoved = true; }
        }

        public bool PropertyChangedEventAdded;
        public bool PropertyChangedEventRemoved;

        public bool TestBool { get; set; }
    }
}