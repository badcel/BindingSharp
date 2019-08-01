using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVMSharp.Gtk;

namespace MVVMSharp.Test.Gtk.View
{
    [TestClass]
    public abstract class IBinderTest
    {
        protected abstract IBinder GetObject();

        [TestMethod]
        public void BindThrowsArgumentNullExceptionIfViewModelIsNull()
        {
            var obj = GetObject();
            Assert.ThrowsException<ArgumentNullException>(() => obj.Bind(null, ""));
        }

        [TestMethod]
        public void BindThrowsBindingExceptionIfAttributeIsNotFoundInViewModel()
        {
            var obj = GetObject();

            Assert.ThrowsException<BindingException>(() => obj.Bind(new object(), "Invalid"));
        }
    }
}