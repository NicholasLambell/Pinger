using System.Windows;
using Jot;

namespace Pinger {
    public static class Services {
        public static readonly Tracker Tracker = new Tracker();

        static Services() {
            Tracker.Configure<Window>()
                .Id(window => window.Name, SystemParameters.VirtualScreenWidth + SystemParameters.VirtualScreenHeight)
                .Properties(
                    window => new {
                        window.Top,
                        window.Left,
                        window.Width,
                        window.Height,
                        window.WindowState,
                    }
                )
                .PersistOn(nameof(Window.Closing))
                .StopTrackingOn(nameof(Window.Closing));
        }
    }
}