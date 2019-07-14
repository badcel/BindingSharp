using System.Windows.Input;
using Gtk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MVVMSharp.Gtk;

namespace MVVMSharp.Test.Gtk.View
{
    [TestClass]
    public class WidgetExtension
    {
        [TestMethod]
        public void BindViewModelIgnoresPropertiesWithoutCommandBindingAttribute()
        {
            var viewModel = new Mock<TestData.ViewModel>();
            var view = new TestData.View.WithoutCommandBinding();

            view.BindViewModel(viewModel.Object);
            
            viewModel.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void BindViewModelThrowsBindingExceptionIfCommandBindingAttributeIsNotSetOnIButtonField()
        {
            var viewModel = new Mock<TestData.ViewModel>();
            var view = new TestData.View.WithCommandBindingWithoutIButton();

            Assert.ThrowsException<BindingException>(() => view.BindViewModel(viewModel.Object));
        }

        [TestMethod]
        public void BindViewModelBindsButtonToICommandOfViewModel(bool canExecute)
        {
            var command = new Mock<ICommand>();
            command.Setup(x => x.CanExecute(It.IsAny<object>())).Returns(canExecute);
            var viewModel = new Mock<TestData.ViewModel>();
            viewModel.Setup(x => x.CommandProperty).Returns(command.Object);
            var button = new Mock<IButton>();

            var view = new TestData.View.WithCommandBinding();
            view.Button = button.Object;

            WidgetExtension.GetCommandBinding = null;
            
            view.BindViewModel(viewModel.Object);

            //TODO: Verifies only if sensitivity is sets and assumes that binding is working
            button.VerifySet(x => x.Sensitive = canExecute);
        }
    }
}
