using System;
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
            return new MVVMSharp.Gtk.BindToProperty(new object(), "");
        }

        [TestMethod]
        public void CreateWithoutViewThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MVVMSharp.Gtk.BindToProperty(null, ""));
        }

        [TestMethod]
        public void CreateWithoutPropertyThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MVVMSharp.Gtk.BindToProperty(new object(), null));
        }

        [TestMethod]
        public void BindThrowsBindingExceptionIfViewModelDoesNotImplementINotifyPropertyChanged()
        {
            var viewModel = new Mock<TestData.ViewModel.WithoutINotifyPropertyChangedImplementation>();
            var view = new Mock<TestData.View.WithObjectProperty>();

            var obj = new MVVMSharp.Gtk.BindToProperty(view.Object, nameof(TestData.View.WithObjectProperty.ObjectProperty));
            
            Assert.ThrowsException<BindingException>(() => obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithoutINotifyPropertyChangedImplementation.ObjectProperty)));
        }

        [TestMethod]
        public void BindThrowsBindingExceptionIfViewIsNoGLibObject()
        {
            var viewModel = new Mock<TestData.ViewModel.WithINotifyPropertyChangedImplementation>();
            var view = new Mock<TestData.View.WithObjectProperty>();

            var obj = new MVVMSharp.Gtk.BindToProperty(view.Object, nameof(TestData.View.WithObjectProperty.ObjectProperty));
            
            Assert.ThrowsException<BindingException>(() => obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithINotifyPropertyChangedImplementation.ObjectProperty)));
        }

        [TestMethod]
        public void ForwardsChangedPropertyFromViewToViewModel()
        {

            var viewModel = new Mock<TestData.ViewModel.WithINotifyPropertyChangedImplementation>();
            var view = new Mock<TestData.View.WithGlibObjectParent>();
            view.Setup(x => x.ObjectProperty).Returns("1");
            
            //Add INotifyPropertyChangedSignals to GLib.Object weil die NotifyArgs des NotifyHandlers von GLib.Object nicht geändert werden können

            var obj = new MVVMSharp.Gtk.BindToProperty(view, nameof(view.Object.ObjectProperty));
            obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithINotifyPropertyChangedImplementation.ObjectProperty));


        }
    }
}