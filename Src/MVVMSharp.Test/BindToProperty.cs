    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MVVMSharp.Test.Gtk.View
{
    [TestClass]
    public class BindToProperty
    {
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
        public void BindThrowsArgumentNullExceptionIfViewModelIsNull()
        {
        }

    }
}