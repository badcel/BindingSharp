using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gtk;
using Moq;
using MVVMSharp.Test.TestData;
using System.Windows.Input;

namespace MVVMSharp.Test.Gtk.View
{
    [TestClass]
    public class BindToCommand
    {
        [TestMethod]
        public void CreateWithoutGtkButtonThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(()=> new MVVMSharp.Gtk.BindToCommand(null));
        }

        [TestMethod]
        public void BindThrowsArgumentNullExceptionIfViewModelIsNull()
        {
            var obj = new MVVMSharp.Gtk.BindToCommand(Mock.Of<IButton>());

            Assert.ThrowsException<ArgumentNullException>(() => obj.Bind(null, ""));
        }

        [TestMethod]
        public void BindThrowsBindingExceptionIfAttributeIsNotFoundInViewModel()
        {
            var obj = new MVVMSharp.Gtk.BindToCommand(Mock.Of<IButton>());

            Assert.ThrowsException<BindingException>(() => obj.Bind(new object(), "Invalid"));
        }

        [TestMethod]
        public void BindThrowsBindingExceptionIfAttributeDoesNotReferenceICommandInViewModel()
        {
            var viewModel = new Mock<ViewModel>();
            viewModel.Setup(x => x.ObjectProperty).Returns(new object());

            var obj = new MVVMSharp.Gtk.BindToCommand(Mock.Of<IButton>());

            Assert.ThrowsException<BindingException>(() => obj.Bind(viewModel.Object, nameof(ViewModel.ObjectProperty)));
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ForwardsICommandCanExecuteChangedEventToReferencedGtkButtonFieldIsSensitiveProperty(bool canExecute)
        {
            var returnFist = !canExecute;
            var returnLast = canExecute;

            var button = new Mock<IButton>();
            var command = new Mock<ICommand>();
            command.Setup(x => x.CanExecute(It.IsAny<object>())).Returns(returnFist);
            var viewModel = new Mock<ViewModel>();
            viewModel.Setup(x => x.CommandProperty).Returns(command.Object);

            var obj = new MVVMSharp.Gtk.BindToCommand(button.Object);
            obj.Bind(viewModel.Object,nameof(ViewModel.CommandProperty));

            //Setup again, to return a different value, if event is raised
            command.Setup(x => x.CanExecute(It.IsAny<object>())).Returns(returnLast);
            command.Raise(x => x.CanExecuteChanged += null, EventArgs.Empty);

            command.Verify(x => x.CanExecute(It.IsAny<object>()), Times.AtLeastOnce);
            button.VerifySet(x => x.Sensitive = returnLast, Times.Once);
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

            var obj = new MVVMSharp.Gtk.BindToCommand(button.Object);
            obj.Bind(viewModel.Object, nameof(ViewModel.CommandProperty));

            command.Verify(x => x.CanExecute(It.IsAny<object>()), Times.Once);
            button.VerifySet(b => b.Sensitive = canExecute);
        }

        [TestMethod]
        public void ForwardGtkButtonFieldClickedEventToReferencedICommandExecuteMethod()
        {
            var button = new Mock<IButton>();
            var command = new Mock<ICommand>();
            command.Setup(x => x.CanExecute(It.IsAny<object>())).Returns(true);
            var viewModel = new Mock<ViewModel>();
            viewModel.Setup(x => x.CommandProperty).Returns(command.Object);

            var obj = new MVVMSharp.Gtk.BindToCommand(button.Object);
            obj.Bind(viewModel.Object, nameof(ViewModel.CommandProperty));

            command.Verify(x => x.Execute(It.IsAny<object>()), Times.Never);
            button.Raise( x => x.Clicked += null, EventArgs.Empty);
            command.Verify(x => x.Execute(It.IsAny<object>()), Times.Once);
        }
    }
}
