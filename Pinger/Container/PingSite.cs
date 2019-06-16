using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Security.Policy;
using Pinger.Enum;

namespace Pinger.Container {
	public class PingSite:BindableBase {
		#region Props

		private Url _location;
		public Url Location {
			get => _location;
			set => SetProperty(ref _location, value);
		}

		private int _ping;
		public int Ping {
			get => _ping;
			set => SetProperty(ref _ping, value);
		}

		private PingStatus _status;
		public PingStatus Status {
			get => _status;
			set => SetProperty(ref _status, value);
		}

		private string _statusMessage;
		public string StatusMessage {
			get => _statusMessage;
			set => SetProperty(ref _statusMessage, value);
		}

		public bool Refreshing {get; set;}

		#endregion

		public PingSite(Url location) {
			Location = location;
			Ping = 0;
			Status = PingStatus.None;
			StatusMessage = Status.StatusMessage();
		}

		private void Worker_Refresh(object sender, DoWorkEventArgs e) {
			PingStatus pingStatus = PingStatus.Fail;

			try {
				using (Ping ping = new Ping()) {
					PingReply reply = ping.Send(Location.Value);

					if (
						reply != null &&
						reply.Status == IPStatus.Success
					) {
						Ping = (int)reply.RoundtripTime;

						pingStatus = PingStatus.Success;
						if (Ping >= 100) {
							pingStatus = PingStatus.Warning;
						} else if (Ping >= 200) {
							pingStatus = PingStatus.Critical;
						}
					}
				}
			} catch {
				// Nothing
			}

			Status = pingStatus;
			StatusMessage = Status.StatusMessage();

			Refreshing = false;
		}

		public void Refresh() {
			if (Refreshing)
				return;

			Refreshing = true;

			Status = PingStatus.Pinging;
			StatusMessage = Status.StatusMessage();

			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += Worker_Refresh;
			worker.RunWorkerAsync();
		}
	}
}