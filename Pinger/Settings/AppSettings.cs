using System.Windows;
using Jot.Configuration;

namespace Pinger.Settings {
    public class AppSettings : BindableBase, ITrackingAware {

        #region Props

        private int _refreshDelay = 2;
        public int RefreshDelay {
            get => _refreshDelay;
            set => SetProperty(ref _refreshDelay, value);
        }

        private bool _graphExpanded;
        public bool GraphExpanded {
            get => _graphExpanded;
            set => SetProperty(ref _graphExpanded, value);
        }

        #endregion

        public void ConfigureTracking(TrackingConfiguration configuration) {
            configuration.AsGeneric<AppSettings>()
                .Properties(
                    appSettings => new {
                        appSettings.RefreshDelay,
                        appSettings.GraphExpanded,
                    }
                );

            Application.Current.Exit += (sender, e) => {
                configuration.Tracker.Persist(this);
            };
        }
    }
}