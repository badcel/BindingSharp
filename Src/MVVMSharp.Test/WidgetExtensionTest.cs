using Gtk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MVVMSharp.Gtk;

namespace MVVMSharp.Test.Gtk.View
{
    [TestClass]
    public class WidgetExtensionTest
    {
        [TestMethod]
        public void BindViewModelIgnoresPropertiesWithoutCommandBindingAttribute()
        {
            var viewModel = new Mock<TestData.ViewModel.WithCommandProperty>();
            var view = new TestData.View.WithoutCommandBinding();

            view.BindViewModel(viewModel.Object);
            
            viewModel.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void BindViewModelThrowsBindingExceptionIfCommandBindingAttributeIsNotSetOnIButtonField()
        {
            var viewModel = new Mock<TestData.ViewModel.WithCommandProperty>();
            var view = new TestData.View.WithCommandBindingWithoutIButton();

            Assert.ThrowsException<BindingException>(() => view.BindViewModel(viewModel.Object));
        }

        [TestMethod]
        public void BindViewModelBindsButtonToICommandOfViewModel()
        {
            var viewModel = new Mock<TestData.ViewModel.WithCommandProperty>();
            var button = new Mock<IButton>();
            var bindToCommand = new Mock<IBinder>();

            var view = new TestData.View.WithCommandBinding();
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
            bindToCommand.Verify(x => x.Bind(viewModel.Object, nameof(TestData.ViewModel.WithCommandProperty.CommandProperty)));
        }
    }
}
