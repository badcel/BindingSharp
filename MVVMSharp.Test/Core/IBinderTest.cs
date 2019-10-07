using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Binding.Core;

namespace Binding.Test.Core
{
    [TestClass]
    public abstract class IBinderTest
    {
        protected abstract IBinder GetObject();

        [TestMethod]
        public void BindThrowsArgumentNullExceptionIfTargetIsNull()
        {
            var obj = GetObject();
            Assert.ThrowsException<ArgumentNullException>(() => obj.Bind(null, ""));
        }

        [TestMethod]
        public void BindThrowsBindingExceptionIfAttributeIsNotFoundInTarget()
        {
            var obj = GetObject();

            Assert.ThrowsException<BindingException>(() => obj.Bind(new object(), "Invalid"));
        }
    }
}