using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Pinger.Enum;

namespace Pinger.Container {
	public class PingSite:BindableBase {
		#region Props

		public ObservableCollection<PingSiteHistory> PingHistory {get; set;}

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
			set {
				SetProperty(ref _status, value);

				// Update status message
				SetProperty(
					ref _statusMessage,
					Status.StatusMessage(),
					nameof(StatusMessage)
				);
			}
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
			PingHistory = new ObservableCollection<PingSiteHistory>();
		}

		private void RecordHistory() {
			PingHistory.Insert(
				0,
				new PingSiteHistory(
					Ping,
					Status,
					StatusMessage
				)
			);

			if (PingHistory.Count <= 100)
				return;

			for (int i = 100; i < PingHistory.Count; i++) {
				PingHistory.RemoveAt(i);
			}
		}

		public async Task Refresh() {
			if (Refreshing)
				return;

			Refreshing = true;

			int targetPing = 0;
			PingStatus pingStatus = PingStatus.Fail;

			try {
				using (Ping ping = new Ping()) {
					PingReply reply = await ping.SendPingAsync(Location.Host);

					if (
						reply != null &&
						reply.Status == IPStatus.Success
					) {
						targetPing = (int)reply.RoundtripTime;

						pingStatus = PingStatus.Success;
						if (targetPing >= 200) {
							pingStatus = PingStatus.Critical;
						} else if (targetPing >= 100) {
							pingStatus = PingStatus.Warning;
						}
					}
				}
			} catch {
				// Nothing
			}

			Ping = targetPing;
			Status = pingStatus;
			RecordHistory();

			Refreshing = false;
		}
	}
}