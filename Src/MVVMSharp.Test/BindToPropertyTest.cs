using System;
using System.ComponentModel;
using GLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MVVMSharp.Gtk;

namespace MVVMSharp.Test.Gtk.View
{
    [TestClass]
    public class BindToPropertyTest : IBinderTest
    {
        protected override IBinder GetObject()
        {
            return new MVVMSharp.Gtk.BindToProperty(Mock.Of<INotifyPropertyChanged>(), "");
        }

        [TestMethod]
        public void CreateWithoutViewThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MVVMSharp.Gtk.BindToProperty(null, ""));
        }

        [TestMethod]
        public void CreateWithoutPropertyThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MVVMSharp.Gtk.BindToProperty(Mock.Of<INotifyPropertyChanged>(), null));
        }

        [TestMethod]
        public void BindThrowsBindingExceptionIfViewModelDoesNotImplementINotifyPropertyChanged()
        {
            var viewModel = new Mock<TestData.ViewModel.WithoutINotifyPropertyChangedImplementation>();
            var view = new Mock<TestData.View.WithINotifyPropertyChanged>();

            var obj = new MVVMSharp.Gtk.BindToProperty(view.Object, nameof(TestData.View.WithObjectProperty.ObjectProperty));
            
            Assert.ThrowsException<BindingException>(() => obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithoutINotifyPropertyChangedImplementation.ObjectProperty)));
        }

        [TestMethod]
        public void ForwardsChangedPropertyFromViewToViewModel()
        {
            object newValue = "1";
            var viewModel = new Mock<TestData.ViewModel.WithINotifyPropertyChangedImplementation>();
            var view = new Mock<TestData.View.WithINotifyPropertyChanged>();
            view.Setup(x => x.ObjectProperty).Returns(newValue);

            var obj = new MVVMSharp.Gtk.BindToProperty(view.Object, nameof(view.Object.ObjectProperty));
            obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithINotifyPropertyChangedImplementation.ObjectProperty));

            view.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(view.Object.ObjectProperty)));

            viewModel.VerifySet(x => x.ObjectProperty = newValue);
        }

        [TestMethod]
        public void DisposeDeregistersPropertyChangedEvent()
        {
            var view = new TestView();
            var obj = new MVVMSharp.Gtk.BindToProperty(view, nameof(view.TestBool));

            obj.Dispose();

            Assert.IsTrue(view.PropertyChangedEventRemoved);
        }
    }
}