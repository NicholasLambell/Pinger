using System;
using System.Net.NetworkInformation;
using Pinger.Enum;

namespace Pinger.Container {
	public class PingSite:BindableBase {
		#region Props

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
		}

		public async void Refresh() {
			if (Refreshing)
				return;

			Refreshing = true;

			PingStatus pingStatus = PingStatus.Fail;

			try {
				using (Ping ping = new Ping()) {
					PingReply reply = await ping.SendPingAsync(Location.Host);

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
			Refreshing = false;
		}
	}
}