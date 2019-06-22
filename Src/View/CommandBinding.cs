using System.Windows.Input;

namespace MVVM
{
    public class CommandBinding
    {
        public Gtk.Button Button { get; }
        public ICommand Command { get; }

        public CommandBinding(Gtk.Button button, ICommand command)
        {
            Button = button ?? throw new System.ArgumentNullException(nameof(button));
            Command = command ?? throw new System.ArgumentNullException(nameof(command));
        }
    }
}