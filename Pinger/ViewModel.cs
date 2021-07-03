using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Jot.Configuration;
using Pinger.Container;
using Pinger.DataController;
using Pinger.Enum;
using Pinger.Extensions;

namespace Pinger {
    public class ViewModel : BindableBase, ITrackingAware {
        public ObservableCollection<PingSite> Sites { get; }
        private string[] PingSiteRawHosts { get; set; }

        private PingSite _selectedSite;
        public PingSite SelectedSite {
            get => _selectedSite;
            set => SetProperty(ref _selectedSite, value);
        }

        private int _selectedIndex;
        public int SelectedIndex {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        public CommandHandler CommandAdd { get; }
        public CommandHandler CommandRemove { get; }
        public CommandHandler CommandSiteNameSubmit { get; }
        public CommandHandler CommandSelectedSiteChanged { get; }

        public DispatcherTimer RefreshTimer { get; }

        private ChartSeriesController _chartSeriesController;
        public ChartSeriesController ChartSeriesController {
            get => _chartSeriesController;
            set => SetProperty(ref _chartSeriesController, value);
        }

        private int _refreshDelay;
        public int RefreshDelay {
            get => _refreshDelay;
            set {
                _refreshDelay = value;
                SetTimerDelay(value);
            }
        }

        private string _siteName;
        public string SiteName {
            get => _siteName;
            set => SetProperty(ref _siteName, value);
        }

        private bool _graphExpanded;
        public bool GraphExpanded {
            get => _graphExpanded;
            set {
                _graphExpanded = value;
                SetProperty(ref _graphExpanded, value);
            }
        }

        public ViewModel() {
            Sites = new ObservableCollection<PingSite>();
            ChartSeriesController = new ChartSeriesController();

            RefreshTimer = new DispatcherTimer();
            RefreshTimer.Tick += RefreshTimer_Tick;
            RefreshDelay = 2;

            CommandAdd = new CommandHandler(AddSite);
            CommandSiteNameSubmit = new CommandHandler(AddSite);
            CommandRemove = new CommandHandler(BtnRemove_Clicked);
            CommandSelectedSiteChanged = new CommandHandler(LstSites_SelectionChanged);

            Services.Tracker.Track(this);
            PostTrackerPropagation();

            RefreshSites();
        }

        public void ConfigureTracking(TrackingConfiguration configuration) {
            configuration.AsGeneric<ViewModel>()
                .Properties(
                    viewModel => new {
                        viewModel.PingSiteRawHosts,
                        viewModel.RefreshDelay,
                        viewModel.GraphExpanded,
                        viewModel.SelectedIndex,
                    }
                );


            Application.Current.Exit += (sender, e) => {
                PopulateRawHosts();

                configuration.Tracker.Persist(this);
            };
        }

        private void PopulateRawHosts() {
            PingSiteRawHosts = Sites.Select(site => site.Location.AbsoluteUri).ToArray();
        }

        private void LoadRawHosts() {
            if (PingSiteRawHosts == null) {
                return;
            }

            foreach (string rawHost in PingSiteRawHosts) {
                Sites.Add(new PingSite(new Uri(rawHost)));
            }
        }

        private void PostTrackerPropagation() {
            LoadRawHosts();

            // Add default example site if none are found
            if (Sites.Count == 0) {
                Sites.Add(new PingSite(new Uri("https://www.google.com/")));
            }

            SelectedSite = LoadSelectedSite();
        }

        private PingSite LoadSelectedSite() {
            if (SelectedIndex < 0) {
                return null;
            }

            if (Sites.ElementAtOrDefault(SelectedIndex) != null) {
                return Sites.ElementAt(SelectedIndex);
            }

            if (Sites.Count >= 1) {
                return Sites.First();
            }

            return null;
        }

        private static Uri SiteStringToUri(string siteName) {
            try {
                return new UriBuilder(siteName).Uri;
            } catch (UriFormatException) {
                return null;
            }
        }

        private void SetTimerDelay(int delay) {
            if (delay == 0) {
                RefreshTimer.Stop();
                return;
            }

            RefreshTimer.Interval = TimeSpan.FromSeconds(delay);
            RefreshTimer.Start();
        }

        private void PlotChartPoint(int ping, PingStatus status) {
            if (status.IsError()) {
                ChartSeriesController.AddBlankPoint();
                return;
            }

            ChartSeriesController.AddPoint(ping);
        }

        private void UpdateChartSeriesFromSite(PingSite site) {
            ChartSeriesController.Clear();

            IEnumerable<PingSiteHistory> trimmedHistory = site.PingHistory
                .Reverse()
                .TakeLast(ChartSeriesController.PointCount);

            foreach (PingSiteHistory history in trimmedHistory) {
                PlotChartPoint(history.Ping, history.Status);
            }
        }

        private async void RefreshSite(PingSite site) {
            await site.Refresh();

            if (site == SelectedSite) {
                PlotChartPoint(site.Ping, site.Status);
            }
        }

        private void RefreshSites() {
            foreach (PingSite site in Sites) {
                RefreshSite(site);
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e) {
            RefreshSites();
        }

        private void AddSite(object param) {
            string siteName = SiteName;
            if (
                param is string &&
                !string.IsNullOrWhiteSpace(param.ToString())
            ) {
                siteName = param.ToString();
            }

            Uri siteUri = SiteStringToUri(siteName);
            if (siteUri == null) {
                MessageBox.Show("Please enter a valid Site Name");
                return;
            }

            SiteName = string.Empty;
            Sites.Add(new PingSite(siteUri));
        }

        private void BtnRemove_Clicked(object param) {
            if (!(param is PingSite site)) {
                return;
            }

            MessageBoxResult confirmResult = MessageBox.Show(
                "Do you really want to remove this Site?",
                "Confirm Removal",
                MessageBoxButton.YesNo
            );

            if (confirmResult == MessageBoxResult.Yes) {
                Sites.Remove(site);
            }
        }

        private void LstSites_SelectionChanged(object param) {
            if (!(param is PingSite site)) {
                return;
            }

            UpdateChartSeriesFromSite(site);
        }
    }
}