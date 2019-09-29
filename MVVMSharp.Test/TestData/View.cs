using System.ComponentModel;
using Gtk;
using MVVMSharp.Gtk;

namespace MVVMSharp.Test.TestData
{
    internal class View
    {
        internal class WithoutCommandBinding : IWidget
        {
            public event PropertyChangedEventHandler PropertyChanged { add {} remove{} }

            public bool Sensitive { get; set; }

            public IButton Button = null;
        }

        internal class WithCommandBindingWithoutIButton : IWidget
        {
            public event PropertyChangedEventHandler PropertyChanged {add {} remove {}}

            public bool Sensitive { get; set; }

            [CommandBinding(nameof(TestData.ViewModel.WithCommandProperty.CommandProperty))]
            public object Button = null;
        }

        internal class WithCommandBinding : IWidget
        {
            public event PropertyChangedEventHandler PropertyChanged {add {} remove{}}
            
            public bool Sensitive { get; set; }

            [CommandBinding(nameof(TestData.ViewModel.WithCommandProperty.CommandProperty))]
            public IButton Button;
        }

        internal interface WithObjectProperty
        {
            object ObjectProperty { get; }
        }

        internal interface WidgetWithObjectPropery : IWidget
        {
            object ObjectProperty { get; set;}
        }


        internal interface WithoutINotifyPropertyChanged
        {
            object ObjectProperty { get; }
        }
    }
}