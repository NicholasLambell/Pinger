using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Pinger.Container;
using Pinger.DataController;
using Pinger.Enum;
using Pinger.Extensions;

namespace Pinger {
	public class ViewModel : BindableBase {
		public ObservableCollection<PingSite> Sites {get; set;}

		private PingSite _selectedSite;
		public PingSite SelectedSite {
			get => _selectedSite;
			set => SetProperty(ref _selectedSite, value);
		}

		public CommandHandler CommandAdd {get; set;}
		public CommandHandler CommandRemove {get; set;}
		public CommandHandler CommandSiteNameSubmit {get; set;}
		public CommandHandler CommandSelectedSiteChanged { get; set; }

		public DispatcherTimer RefreshTimer {get; set;}

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
				SetTimerDelay();
			}
		}

		private string _siteName;
		public string SiteName {
			get => _siteName;
			set => SetProperty(ref _siteName, value);
		}

		public ViewModel() {
			Sites = new ObservableCollection<PingSite>();
			Sites.Add(new PingSite(
				new Uri("https://www.google.com")
			));

			ChartSeriesController = new ChartSeriesController();

			RefreshTimer = new DispatcherTimer();
			RefreshTimer.Tick += RefreshTimer_Tick;
			RefreshDelay = 2;

			CommandAdd = new CommandHandler(
				AddSite
			);

			CommandSiteNameSubmit = new CommandHandler(
				AddSite
			);

			CommandRemove = new CommandHandler(
				BtnRemove_Clicked
			);
			CommandSelectedSiteChanged = new CommandHandler(LstSites_SelectionChanged);

			// If we have any pre-loaded sites trigger an initial refresh
			if (Sites.Count > 0) {
				SelectedSite = Sites.First();
				RefreshSites();
			}
		}

		private void SetTimerDelay() {
			if (RefreshDelay == 0) {
				RefreshTimer.Stop();
				return;
			}

			RefreshTimer.Interval = TimeSpan.FromSeconds(RefreshDelay);
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

			IEnumerable<PingSiteHistory> trimmedHistory = site.PingHistory.Reverse().TakeLast(ChartSeriesController.PointCount);
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
			)
				siteName = param.ToString();

			Uri newUri;
			try {
				newUri = new UriBuilder(siteName).Uri;
			} catch (UriFormatException) {
				MessageBox.Show("Please enter a valid Site Name");
				return;
			}

			SiteName = string.Empty;
			Sites.Add(new PingSite(newUri));
		}

		private void BtnRemove_Clicked(object param) {
			if (!(param is PingSite site))
				return;

			MessageBoxResult confirmResult = MessageBox.Show(
				"Do you really want to remove this Site?",
				"Confirm Removal",
				MessageBoxButton.YesNo
			);

			if (confirmResult == MessageBoxResult.Yes)
				Sites.Remove(site);
		}

		private void LstSites_SelectionChanged(object param) {
			if (!(param is PingSite site)) {
				return;
			}

			UpdateChartSeriesFromSite(site);
		}
	}
}