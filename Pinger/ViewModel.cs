using System.Collections.ObjectModel;
using System.Security.Policy;
using Pinger.Container;

namespace Pinger {
	public class ViewModel {
		public ObservableCollection<PingSite> Sites {get; set;}

		public CommandHandler CommandRefresh {get; set;}
		public CommandHandler CommandAdd {get; set;}

		public ViewModel() {
			Sites = new ObservableCollection<PingSite>();

			Sites.Add(new PingSite(
				new Url("www.google.com")
			));

			CommandRefresh = new CommandHandler(
				BtnRefresh_Clicked
			);

			CommandAdd = new CommandHandler(
				BtnAdd_Clicked
			);
		}

		private void BtnRefresh_Clicked(object param) {
			foreach (PingSite site in Sites) {
				site.Refresh();
			}
		}

		private void BtnAdd_Clicked(object param) {
			Sites.Add(new PingSite(
				new Url("www.test.com")
			));
		}
	}
}