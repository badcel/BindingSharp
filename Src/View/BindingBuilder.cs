using System.Reflection;
using Gtk;

namespace MVVM
{
    public class BindingBuilder : Builder
    {
        public BindingBuilder(string resource_name) : base(Assembly.GetCallingAssembly().GetManifestResourceStream(resource_name))
        {
        }
    }
}