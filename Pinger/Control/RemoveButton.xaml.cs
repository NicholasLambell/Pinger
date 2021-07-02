using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pinger.Control {
    public partial class RemoveButton : UserControl {
        public ICommand Command {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty
            .Register(
                "Command",
                typeof(ICommand),
                typeof(RemoveButton),
                new FrameworkPropertyMetadata(null)
            );

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty
            .Register(
                "CommandParameter",
                typeof(object),
                typeof(RemoveButton),
                new FrameworkPropertyMetadata(null)
            );

        public RemoveButton() {
            InitializeComponent();
        }
    }
}