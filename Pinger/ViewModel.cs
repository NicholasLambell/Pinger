using System;
using System.Collections.ObjectModel;
using System.Windows;
using Pinger.Container;

namespace Pinger {
	public class ViewModel : BindableBase {
		public ObservableCollection<PingSite> Sites {get; set;}

		public CommandHandler CommandRefresh {get; set;}
		public CommandHandler CommandAdd {get; set;}
		public CommandHandler CommandRemove {get; set;}

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

			CommandRefresh = new CommandHandler(
				BtnRefresh_Clicked
			);

			CommandAdd = new CommandHandler(
				BtnAdd_Clicked
			);

			CommandRemove = new CommandHandler(
				BtnRemove_Clicked
			);
		}

		private void BtnRefresh_Clicked(object param) {
			foreach (PingSite site in Sites) {
				site.Refresh();
			}
		}

		private void BtnAdd_Clicked(object param) {
			Uri newUri;
			try {
				newUri = new UriBuilder(SiteName).Uri;
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