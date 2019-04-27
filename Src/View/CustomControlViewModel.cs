using System;

namespace MVVM
{
    public class CustomControlViewModel : IViewModel
    {
        public Type View => typeof(CustomControl);

        private string label;
        public string Label
        {
            get { return label; }
            set
            {
                label = value;
                Console.WriteLine("VIEWMODEL: " + value);
            }
        }
    }
}