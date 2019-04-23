using System;

namespace MVVM
{
    public class CustomControlViewModel : IViewModel
    {
        public Type View => typeof(CustomControl);
    }
}