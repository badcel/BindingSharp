using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gtk;
using Moq;
using MVVMSharp.Test.TestData;
using System.Windows.Input;

namespace MVVMSharp.Test.View
{
    [TestClass]
    public class BindToCommand
    {
        [TestMethod]
        public void CreateWithoutGtkButtonThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(()=> new MVVMSharp.BindToCommand(null));
        }

        [TestMethod]
        public void BindThrowsArgumentNullExceptionIfViewModelIsNull()
        {
            var obj = new MVVMSharp.BindToCommand(Mock.Of<IButton>());

            Assert.ThrowsException<ArgumentNullException>(() => obj.Bind(null, ""));
        }

        [TestMethod]
        public void BindThrowsBindingExceptionIfAttributeIsNotFoundInViewModel()
        {
            var obj = new MVVMSharp.BindToCommand(Mock.Of<IButton>());

            Assert.ThrowsException<BindingException>(() => obj.Bind(new object(), "Invalid"));
        }

        [TestMethod]
        public void BindThrowsBindingExceptionIfAttributeDoesNotReferenceICommandInViewModel()
        {
            var viewModel = new Mock<ViewModel>();
            viewModel.Setup(x => x.ObjectProperty).Returns(new object());

            var obj = new MVVMSharp.BindToCommand(Mock.Of<IButton>());

            Assert.ThrowsException<BindingException>(() => obj.Bind(viewModel.Object, nameof(ViewModel.ObjectProperty)));
        }

        [TestMethod]
        public void ForwardsICommandCanExecuteChangedEventToReferencedGtkButtonFieldIsSensitiveProperty()
        {
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void BindSetsGtkButtonSensitiveToICommandCanExecuteMethod(bool canExecute)
        {
            var button = new Mock<IButton>();
            var command = new Mock<ICommand>();
            command.Setup(x => x.CanExecute(It.IsAny<object>())).Returns(canExecute);
            var viewModel = new Mock<ViewModel>();
            viewModel.Setup(x => x.CommandProperty).Returns(command.Object);

            var obj = new MVVMSharp.BindToCommand(button.Object);
            obj.Bind(viewModel.Object, nameof(ViewModel.CommandProperty));

            button.VerifySet(b => b.Sensitive = canExecute);
        }

        [TestMethod]
        public void ForwardGtkButtonFieldClickedEventToReferencedICommandExecuteMethod()
        {
        }
    }
}
