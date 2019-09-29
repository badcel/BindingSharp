using System.ComponentModel;
using System.Windows.Input;

namespace MVVMSharp.Test.TestData
{
    internal class ViewModel
    {
        internal interface WithCommandProperty 
        {
            object ObjectProperty { get; }
            ICommand CommandProperty { get; }
        }

        internal interface WithoutINotifyPropertyChangedImplementation
        {
            object ObjectProperty { get; }
        }

        internal interface WithINotifyPropertyChangedImplementation : INotifyPropertyChanged
        {
            object ObjectProperty { get; set; }
        }
    }
}