using System;
using System.ComponentModel;
using System.Collections.Generic;
using Gtk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Binding.Gtk;
using Binding.Test.TestData;

namespace Binding.Test.Gtk
{
    [TestClass]
    public class BindStyleContextToNotifyDataErrorInfoTest
    {
        protected BindStyleContextToNotifyDataErrorInfo GetObject(IStyleContext styleContext = null, string cssClassName = null)
        {
            if(styleContext == null)
                styleContext = Mock.Of<IStyleContext>();

            if(cssClassName == null)
                cssClassName = "";

            return new BindStyleContextToNotifyDataErrorInfo(styleContext, cssClassName);
        }
        [TestMethod]
        public void CreateWithoutStyleContextThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BindStyleContextToNotifyDataErrorInfo(null, ""));
        }

        [TestMethod]
        public void CreateWithoutPropertyThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BindStyleContextToNotifyDataErrorInfo(Mock.Of<IStyleContext>(), null));
        }

        [TestMethod]
        public void BindWithoutTargetThrowsArgumentNullException()
        {
            var obj = GetObject();
            Assert.ThrowsException<ArgumentNullException>(() => obj.Bind(null, ""));
        }

        [TestMethod]
        public void BindWithTargetWhichIsNoINotifyDataErrorInfoThrowsArgumentException()
        {
            var obj = GetObject();
            Assert.ThrowsException<ArgumentException>(() => obj.Bind(new object(), ""));
        }

        [TestMethod]
        public void GenericBindWithoutTargetThrowsArgumentNullException()
        {
            var obj = GetObject();
            Assert.ThrowsException<ArgumentNullException>(() => obj.Bind(default(INotifyDataErrorInfo), ""));
        }

        [TestMethod]
        public void GenericBindWithoutPropertyThrowsArgumentNullException()
        {
            var obj = GetObject();
            Assert.ThrowsException<ArgumentNullException>(() => obj.Bind(Mock.Of<INotifyDataErrorInfo>(), null));
        }

        [TestMethod]
        public void OnErrorsChangedAddsClassToStyleContextIfNotPresent()
        {
            var cssClasName = "cssClassName";
            var property = "para";

            var styleContext = new Mock<IStyleContext>();
            styleContext.Setup(x => x.HasClass(cssClasName)).Returns(false);
            
            var notifyDataErrorInfo = new Mock<INotifyDataErrorInfo>();
            notifyDataErrorInfo.Setup(x => x.GetErrors(property)).Returns(new List<string>(){"Error"});

            var obj = GetObject(styleContext.Object, cssClasName);
            obj.Bind(notifyDataErrorInfo.Object, property);

            notifyDataErrorInfo.Raise(x => x.ErrorsChanged += null, new DataErrorsChangedEventArgs(property));

            styleContext.Verify(x => x.AddClass(cssClasName));
        }

        [TestMethod]
        public void OnErrorsChangedRemovesClassFromStyleContextIfPresent()
        {
            var cssClasName = "cssClassName";
            var property = "para";

            var styleContext = new Mock<IStyleContext>();
            styleContext.Setup(x => x.HasClass(cssClasName)).Returns(true);
            
            var notifyDataErrorInfo = new Mock<INotifyDataErrorInfo>();
            notifyDataErrorInfo.Setup(x => x.GetErrors(property)).Returns(new List<string>());

            var obj = GetObject(styleContext.Object, cssClasName);
            obj.Bind(notifyDataErrorInfo.Object, property);

            notifyDataErrorInfo.Raise(x => x.ErrorsChanged += null, new DataErrorsChangedEventArgs(property));

            styleContext.Verify(x => x.RemoveClass(cssClasName));
        }

        [TestMethod]
        public void DisposeDeregistersErrorsChangedEvent()
        {
            var viewModel = new TestViewModel();
            var obj = GetObject();
            obj.Bind(viewModel, "");

            obj.Dispose();

            Assert.IsTrue(viewModel.ErrorsChangedEventRemoved);
        }
    }
}