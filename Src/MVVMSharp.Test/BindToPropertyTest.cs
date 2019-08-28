using System;
using System.ComponentModel;
using GLib;
using Gtk;
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
            return new MVVMSharp.Gtk.BindWidgetToProperty(Mock.Of<TestData.View.WidgetWithObjectPropery>(), nameof(TestData.View.WidgetWithObjectPropery.ObjectProperty));
        }

        [TestMethod]
        public void CreateWithoutViewThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MVVMSharp.Gtk.BindWidgetToProperty(null, ""));
        }

        [TestMethod]
        public void CreateWithoutPropertyThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MVVMSharp.Gtk.BindWidgetToProperty(Mock.Of<IWidget>(), null));
        }

        [TestMethod]
        public void ForwardsChangedPropertyFromViewToViewModel()
        {
            object newValue = "1";
            var viewModel = new Mock<TestData.ViewModel.WithINotifyPropertyChangedImplementation>();
            var view = new Mock<TestData.View.WidgetWithObjectPropery>();
            view.Setup(x => x.ObjectProperty).Returns(newValue);

            var obj = new MVVMSharp.Gtk.BindWidgetToProperty(view.Object, nameof(view.Object.ObjectProperty));
            obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithINotifyPropertyChangedImplementation.ObjectProperty));

            view.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(view.Object.ObjectProperty)));

            viewModel.VerifySet(x => x.ObjectProperty = newValue);
        }

        [TestMethod]
        public void ForwardsChangedPropertyFromViewModelToView()
        {
            object newValue = "1";
            var viewModel = new Mock<TestData.ViewModel.WithINotifyPropertyChangedImplementation>();
            viewModel.Setup(x => x.ObjectProperty).Returns(newValue);
            var view = new Mock<TestData.View.WidgetWithObjectPropery>();

            var obj = new MVVMSharp.Gtk.BindWidgetToProperty(view.Object, nameof(view.Object.ObjectProperty));
            obj.Bind(viewModel.Object, nameof(TestData.ViewModel.WithINotifyPropertyChangedImplementation.ObjectProperty));

            viewModel.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(viewModel.Object.ObjectProperty)));

            view.VerifySet(x => x.ObjectProperty = newValue);
        }

        [TestMethod]
        public void DisposeDeregistersPropertyChangedEventFromView()
        {
            var view = new TestWidget();
            var obj = new MVVMSharp.Gtk.BindWidgetToProperty(view, nameof(view.TestBool));

            obj.Dispose();

            Assert.IsTrue(view.PropertyChangedEventRemoved);
        }

        [TestMethod]
        public void DisposeDeregistersPropertyChangedEventFromViewModel()
        {
            var view = new TestWidget();
            var viewModel = new TestViewModel();
            var obj = new MVVMSharp.Gtk.BindWidgetToProperty(view, nameof(view.TestBool));
            obj.Bind(viewModel, nameof(viewModel.TestBool));

            obj.Dispose();

            Assert.IsTrue(viewModel.PropertyChangedEventRemoved);
        }
    }
}