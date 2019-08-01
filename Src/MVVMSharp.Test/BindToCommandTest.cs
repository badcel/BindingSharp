using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gtk;
using Moq;
using MVVMSharp.Test.TestData;
using System.Windows.Input;
using MVVMSharp.Gtk;

namespace MVVMSharp.Test.Gtk.View
{
    [TestClass]
    public class BindToCommandTest : IBinderTest
    {
        
        protected override IBinder GetObject()
        {
            return new MVVMSharp.Gtk.BindToCommand(Mock.Of<IButton>());
        }

        [TestMethod]
        public void CreateWithoutGtkButtonThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(()=> new MVVMSharp.Gtk.BindToCommand(null));
        }

        [TestMethod]
        public void BindThrowsBindingExceptionIfAttributeDoesNotReferenceICommandInViewModel()
        {
            var viewModel = new Mock<TestData.ViewModel.WithCommandProperty>();
            viewModel.Setup(x => x.ObjectProperty).Returns(new object());

            var obj = new MVVMSharp.Gtk.BindToCommand(Mock.Of<IButton>());

            Assert.ThrowsException<BindingException>(() => obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithCommandProperty.ObjectProperty)));
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
            var viewModel = new Mock<TestData.ViewModel.WithCommandProperty>();
            viewModel.Setup(x => x.CommandProperty).Returns(command.Object);

            var obj = new MVVMSharp.Gtk.BindToCommand(button.Object);
            obj.Bind(viewModel.Object,nameof(TestData.ViewModel.WithCommandProperty.CommandProperty));

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
            var viewModel = new Mock<TestData.ViewModel.WithCommandProperty>();
            viewModel.Setup(x => x.CommandProperty).Returns(command.Object);

            var obj = new MVVMSharp.Gtk.BindToCommand(button.Object);
            obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithCommandProperty.CommandProperty));

            command.Verify(x => x.CanExecute(It.IsAny<object>()), Times.Once);
            button.VerifySet(b => b.Sensitive = canExecute);
        }

        [TestMethod]
        public void ForwardGtkButtonFieldClickedEventToReferencedICommandExecuteMethod()
        {
            var button = new Mock<IButton>();
            var command = new Mock<ICommand>();
            command.Setup(x => x.CanExecute(It.IsAny<object>())).Returns(true);
            var viewModel = new Mock<TestData.ViewModel.WithCommandProperty>();
            viewModel.Setup(x => x.CommandProperty).Returns(command.Object);

            var obj = new MVVMSharp.Gtk.BindToCommand(button.Object);
            obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithCommandProperty.CommandProperty));

            command.Verify(x => x.Execute(It.IsAny<object>()), Times.Never);
            button.Raise( x => x.Clicked += null, EventArgs.Empty);
            command.Verify(x => x.Execute(It.IsAny<object>()), Times.Once);
        }

        [TestMethod]
        public void DisposeDeregistersButtonClickedEvent()
        {
            var button = new TestButton();
            var obj = new MVVMSharp.Gtk.BindToCommand(button);

            obj.Dispose();

            Assert.IsTrue(button.ClickedEventWasRemoved);
        }

        [TestMethod]
        public void DisposeDeregistersCommandCanExecuteChanged()
        {
            var command = new TestCommand();
            var viewModel = new Mock<TestData.ViewModel.WithCommandProperty>();
            viewModel.Setup(x => x.CommandProperty).Returns(command);

            var obj = new MVVMSharp.Gtk.BindToCommand(Mock.Of<IButton>());
            obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithCommandProperty.CommandProperty));

            obj.Dispose();

            Assert.IsTrue(command.CanExecuteChangedWasRemoved);
        }
    }
}
