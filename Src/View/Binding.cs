using System;

namespace MVVM
{
    public class Binding
    {
        public object View { get; }
        public string ViewProp { get; }
        public object ViewModel { get; }
        public string ViewModelProp { get; }

        public Binding(object view, string viewProp, object viewModel, string viewModelProp)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
            ViewProp = viewProp ?? throw new ArgumentNullException(nameof(viewProp));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            ViewModelProp = viewModelProp ?? throw new ArgumentNullException(nameof(viewModelProp));
        }
    }
}