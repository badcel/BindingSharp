using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Binding.Core;
using Binding.Test.TestData;

namespace Binding.Test.Core
{
    [TestClass]
    public class BindTwoINotifyPropertyChangedObjectss : IBinderTest
    {
        internal override IBinder GetObject()
        {
            return new BindINotifyPropertyChanged(Mock.Of<TestData.View.WidgetWithObjectPropery>(), nameof(TestData.View.WidgetWithObjectPropery.ObjectProperty));
        }

        [TestMethod]
        public void CreateWithoutSourceThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BindINotifyPropertyChanged(null, ""));
        }

        [TestMethod]
        public void CreateWithoutPropertyThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BindINotifyPropertyChanged(Mock.Of<INotifyPropertyChanged>(), null));
        }

        [TestMethod]
        public void CreateWithUnknownPropertyThrowsBindingException()
        {
            var view = Mock.Of<TestData.View.WidgetWithObjectPropery>();

            Assert.ThrowsException<BindingException>(() => new BindINotifyPropertyChanged(view, "Wrong"));
        }

        [TestMethod]
        public void ForwardsChangedPropertyFromSourceToTarget()
        {
            object newValue = "1";
            var target = new Mock<TestData.ViewModel.WithINotifyPropertyChangedImplementation>();
            var source = new Mock<TestData.View.WidgetWithObjectPropery>();
            source.Setup(x => x.ObjectProperty).Returns(newValue);

            var obj = new BindINotifyPropertyChanged(source.Object, nameof(source.Object.ObjectProperty));
            obj.Bind(target.Object, nameof(target.Object.ObjectProperty));

            source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(source.Object.ObjectProperty)));

            target.VerifySet(x => x.ObjectProperty = newValue);
        }

        [TestMethod]
        public void ForwardsChangedPropertyFromTargetToSource()
        {
            object newValue = "1";
            var target = new Mock<TestData.ViewModel.WithINotifyPropertyChangedImplementation>();
            target.Setup(x => x.ObjectProperty).Returns(newValue);
            var source = new Mock<TestData.View.WidgetWithObjectPropery>();

            var obj = new BindINotifyPropertyChanged(source.Object, nameof(source.Object.ObjectProperty));
            obj.Bind(target.Object, nameof(target.Object.ObjectProperty));

            target.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(target.Object.ObjectProperty)));

            source.VerifySet(x => x.ObjectProperty = newValue);
        }

        [TestMethod]
        public void DisposeDeregistersPropertyChangedEventFromSource()
        {
            var source = new TestWidget();
            var obj = new BindINotifyPropertyChanged(source, nameof(source.TestBool));

            obj.Dispose();

            Assert.IsTrue(source.PropertyChangedEventRemoved);
        }

        [TestMethod]
        public void DisposeDeregistersPropertyChangedEventFromTarget()
        {
            var source = new TestWidget();
            var target = new TestViewModel();
            var obj = new BindINotifyPropertyChanged(source, nameof(source.TestBool));
            obj.Bind(target, nameof(target.TestBool));

            obj.Dispose();

            Assert.IsTrue(target.PropertyChangedEventRemoved);
        }
    }
}