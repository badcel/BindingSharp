using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MVVMSharp.Test.Gtk.View
{
    [TestClass]
    public class BindToProperty
    {
        [TestMethod]
        public void ReportsErrorIfAttributeIsNotAssignedToAField()
        {
        }

        [TestMethod]
        public void ReportsErrorIfAttributeDoesNotReferenceViewGlibProperty()
        {}

        [TestMethod]
        public void ReportsErrorIfAttributeDoesNotReferenceAnViewModelINotifyPropertyChangedObject()
        {}

        [TestMethod]
        public void ReportsErrorIfAttributeDoesNotReferenceAnPropertyOnViewModel()
        {}

        [TestMethod]
        public void InitializeViewWithValueFromViewModel()
        {}

        [TestMethod]
        public void ViewModelPropertyChangedUpdatesConnectedViewProperty()
        {}

        [TestMethod]
        public void ViewNotifyEventUpdatesConnectedViewModelProperty()
        {}

    }
}