using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Pinger.Container;

namespace Pinger {
	public class ViewModel : BindableBase {
		public ObservableCollection<PingSite> Sites {get; set;}

		private PingSite _selectedSite;
		public PingSite SelectedSite {
			get => _selectedSite;
			set => SetProperty(ref _selectedSite, value);
		}

		public CommandHandler CommandRefresh {get; set;}
		public CommandHandler CommandAdd {get; set;}
		public CommandHandler CommandRemove {get; set;}
		public CommandHandler CommandSiteNameSubmit {get; set;}

		public DispatcherTimer RefreshTimer {get; set;}

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

			RefreshTimer = new DispatcherTimer();
			RefreshTimer.Tick += RefreshTimer_Tick;
			RefreshDelay = 2;

			CommandRefresh = new CommandHandler(
				BtnRefresh_Clicked
			);

			CommandAdd = new CommandHandler(
				AddSite
			);

			CommandSiteNameSubmit = new CommandHandler(
				AddSite
			);

			CommandRemove = new CommandHandler(
				BtnRemove_Clicked
			);

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

		private void RefreshSites() {
			foreach (PingSite site in Sites) {
				site.Refresh();
			}
		}

		private void RefreshTimer_Tick(object sender, EventArgs e) {
			RefreshSites();
		}

		private void BtnRefresh_Clicked(object param) {
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
				"Do you really want to delete this Site?",
				"Confirm Deletion",
				MessageBoxButton.YesNo
			);

			if (confirmResult == MessageBoxResult.Yes)
				Sites.Remove(site);
		}
	}
}