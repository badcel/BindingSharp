using System.ComponentModel;
using Gtk;
using Binding.Gtk;

namespace Binding.Test.TestData
{
    public class View
    {
        public class WithoutCommandBinding : IWidget
        {
            public event PropertyChangedEventHandler PropertyChanged { add {} remove{} }

            public bool Sensitive { get; set; }

            public IStyleContext StyleContext => null;

            public IButton Button = null;
        }

        public class WithCommandBindingWithoutIButton : IWidget
        {
            public event PropertyChangedEventHandler PropertyChanged {add {} remove {}}

            public bool Sensitive { get; set; }

            public IStyleContext StyleContext => null;

            [CommandBinding(nameof(TestData.ViewModel.WithCommandProperty.CommandProperty))]
            public object Button = null;
        }

        public class WithCommandBinding : IWidget
        {
            public event PropertyChangedEventHandler PropertyChanged {add {} remove{}}
            
            public bool Sensitive { get; set; }

            public IStyleContext StyleContext => null;

            [CommandBinding(nameof(TestData.ViewModel.WithCommandProperty.CommandProperty))]
            public IButton Button;
        }

        public interface WithObjectProperty
        {
            object ObjectProperty { get; }
        }

        public interface WidgetWithObjectPropery : IWidget
        {
            object ObjectProperty { get; set;}
        }


        public interface WithoutINotifyPropertyChanged
        {
            object ObjectProperty { get; }
        }
    }
}