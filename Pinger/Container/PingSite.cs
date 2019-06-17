using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using Pinger.Enum;

namespace Pinger.Container {
	public class PingSite:BindableBase {
		#region Props

		public BackgroundWorker Worker {get;}

		private Uri _location;
		public Uri Location {
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

		public PingSite(Uri location) {
			Location = location;
			Ping = 0;
			Status = PingStatus.None;
			StatusMessage = Status.StatusMessage();

			Worker = new BackgroundWorker();
			Worker.DoWork += Worker_Refresh;
		}

		private void Worker_Refresh(object sender, DoWorkEventArgs e) {
			PingStatus pingStatus = PingStatus.Fail;

			try {
				using (Ping ping = new Ping()) {
					PingReply reply = ping.Send(Location.Host);

					if (
						reply != null &&
						reply.Status == IPStatus.Success
					) {
						Ping = (int)reply.RoundtripTime;

						pingStatus = PingStatus.Success;
						if (Ping >= 200) {
							pingStatus = PingStatus.Critical;
						} else if (Ping >= 100) {
							pingStatus = PingStatus.Warning;
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

			Worker.RunWorkerAsync();
		}
	}
}