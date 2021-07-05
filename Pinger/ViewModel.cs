using System;
using System.Collections;
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
using Pinger.Settings;

namespace Pinger {
    public class ViewModel : BindableBase, ITrackingAware {
        #region Props

        private string[] PingSiteRawHosts { get; set; }
        private Dictionary<PingSite, ChartValuesController> ChartValuesMap { get; }

        public DispatcherTimer RefreshTimer { get; }
        public AppSettings Settings { get; }
        public ObservableCollection<PingSite> Sites { get; }

        public CommandHandler CommandAdd { get; }
        public CommandHandler CommandRemove { get; }
        public CommandHandler CommandSiteNameSubmit { get; }

        private ChartSeriesController _chartSeriesController;
        public ChartSeriesController ChartSeriesController {
            get => _chartSeriesController;
            set => SetProperty(ref _chartSeriesController, value);
        }

        private int _selectedIndex;
        public int SelectedIndex {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        private PingSite _selectedSite;
        public PingSite SelectedSite {
            get => _selectedSite;
            set => SetProperty(ref _selectedSite, value);
        }

        private IList<PingSite> _selectedSites;
        public IList<PingSite> SelectedSites {
            get => _selectedSites;
            set => SetProperty(ref _selectedSites, value);
        }

        private string _siteName;
        public string SiteName {
            get => _siteName;
            set => SetProperty(ref _siteName, value);
        }

        #endregion

        public ViewModel() {
            Settings = App.Settings;
            Sites = new ObservableCollection<PingSite>();
            ChartSeriesController = new ChartSeriesController();
            ChartValuesMap = new Dictionary<PingSite, ChartValuesController>();
            SelectedSites = new List<PingSite>();

            RefreshTimer = new DispatcherTimer();
            RefreshTimer.Tick += RefreshTimer_Tick;
            Settings.PropertyValueChanged(nameof(Settings.RefreshDelay), value => SetTimerDelay((int)value));

            PropertyValueChanged(nameof(SelectedSites), SelectedSites_Changed);

            CommandAdd = new CommandHandler(AddSite);
            CommandSiteNameSubmit = new CommandHandler(AddSite);
            CommandRemove = new CommandHandler(BtnRemove_Clicked);

            Services.Tracker.Track(this);
            PostTrackerPropagation();

            // todo: this will have to be based off persisted selection
            foreach (PingSite site in Sites) {
                AddChartSeries(site);
            }

            SetTimerDelay(Settings.RefreshDelay);
            RefreshSites();
        }

        public void ConfigureTracking(TrackingConfiguration configuration) {
            configuration.AsGeneric<ViewModel>()
                .Properties(
                    viewModel => new {
                        viewModel.PingSiteRawHosts,
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

        private ChartValuesController GetChartValuesForSite(PingSite site) {
            return ChartValuesMap[site];
        }

        private bool SiteOnChart(PingSite site) {
            return ChartValuesMap.ContainsKey(site);
        }

        private void ClearChartSeries() {
            ChartSeriesController.Clear();
            ChartValuesMap.Clear();
        }

        private ChartValuesController AddChartSeries(PingSite site) {
            ChartValuesController valuesController = ChartSeriesController.AddSeries(site.ChartColor);
            ChartValuesMap[site] = valuesController;

            if (site.PingHistory.Count != 0) {
                PlotSiteHistory(site);
            }

            return valuesController;
        }

        private void PlotSiteHistory(PingSite site) {
            IEnumerable<PingSiteHistory> trimmedHistory = site.PingHistory
                .Reverse()
                .TakeLast(ChartSeriesController.PointCount);

            foreach (PingSiteHistory history in trimmedHistory) {
                PlotChartPoint(site, history.Ping, history.Status);
            }
        }

        private void PlotChartPoint(PingSite site, int ping, PingStatus status) {
            ChartValuesController valuesController = GetChartValuesForSite(site);

            if (status.IsError()) {
                valuesController.AddBlankPoint();
                return;
            }

            valuesController.AddPoint(ping);
        }

        private void UpdateChartSeriesFromSites(IEnumerable sites) {
            ClearChartSeries();

            foreach (PingSite site in sites) {
                AddChartSeries(site);
            }
        }

        private async void RefreshSite(PingSite site) {
            await site.Refresh();

            if (SiteOnChart(site)) {
                PlotChartPoint(site, site.Ping, site.Status);
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

        private void SelectedSites_Changed(object value) {
            if (!(value is IList sites)) {
                return;
            }

            UpdateChartSeriesFromSites(sites);
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
    }
}