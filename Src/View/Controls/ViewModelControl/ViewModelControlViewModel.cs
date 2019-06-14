using System;

namespace MVVM
{
    public class ViewModelControlViewModel : IViewModel
    {
        public Type View => typeof(ViewModelControl);

        private string label = "MEIN LABEL NEU";
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