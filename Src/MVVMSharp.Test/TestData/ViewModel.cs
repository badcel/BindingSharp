using System.Windows.Input;

namespace MVVMSharp.Test.TestData
{
    public interface ViewModel 
    {
        object ObjectProperty { get; }
        ICommand CommandProperty { get; }
    }
}