using Gtk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Binding.Core;
using Binding.Gtk;
using Binding.Test.TestData;

namespace Binding.Test.Gtk
{
    [TestClass]
    public class WidgetExtensionTest
    {
        [TestMethod]
        public void BindViewModelIgnoresPropertiesWithoutCommandBindingAttribute()
        {
            var viewModel = new Mock<ViewModel.WithCommandProperty>();
            var view = new View.WithoutCommandBinding();

            view.Bind(viewModel.Object);
            
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
            Binding.Gtk.WidgetExtension.CommandBindingProvider = (IButton b) => 
            { 
                if(b == button.Object)
                    buttonPassed = true;

                return bindToCommand.Object;
            };
            
            view.Bind(viewModel.Object);

            Assert.IsTrue(buttonPassed);
            bindToCommand.Verify(x => x.Bind(viewModel.Object, nameof(viewModel.Object.CommandProperty)));
        }
    }
}
