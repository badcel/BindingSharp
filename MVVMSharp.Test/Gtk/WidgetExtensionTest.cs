using Gtk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MVVMSharp.Core;
using MVVMSharp.Gtk;
using MVVMSharp.Test.TestData;

namespace MVVMSharp.Test.Gtk
{
    [TestClass]
    public class WidgetExtensionTest
    {
        [TestMethod]
        public void BindViewModelIgnoresPropertiesWithoutCommandBindingAttribute()
        {
            var viewModel = new Mock<ViewModel.WithCommandProperty>();
            var view = new View.WithoutCommandBinding();

            view.BindViewModel(viewModel.Object);
            
            viewModel.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void BindViewModelBindsButtonToICommandOfViewModel()
        {
            var viewModel = new Mock<ViewModel.WithCommandProperty>();
            var button = new Mock<IButton>();
            var bindToCommand = new Mock<IBinder>();

            var view = new View.WithCommandBinding();
            view.Button = button.Object;

            var buttonPassed = false;
            MVVMSharp.Gtk.WidgetExtension.CommandBindingProvider = (IButton b) => 
            { 
                if(b == button.Object)
                    buttonPassed = true;

                return bindToCommand.Object;
            };
            
            view.BindViewModel(viewModel.Object);

            Assert.IsTrue(buttonPassed);
            bindToCommand.Verify(x => x.Bind(viewModel.Object, nameof(viewModel.Object.CommandProperty)));
        }
    }
}
