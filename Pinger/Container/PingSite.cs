using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Media;
using Pinger.Enum;
using Pinger.Util;

namespace Pinger.Container {
    public class PingSite : BindableBase {
        #region Props

        public ObservableCollection<PingSiteHistory> PingHistory { get; }

        private Color _chartColor;
        public Color ChartColor {
            get => _chartColor;
            set => SetProperty(ref _chartColor, value);
        }

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

        public bool Refreshing { get; set; }

        #endregion

        public PingSite(Uri location) {
            Location = location;
            ChartColor = RandomUtil.NextColor();
            Ping = 0;
            Status = PingStatus.None;
            PingHistory = new ObservableCollection<PingSiteHistory>();
        }

        private static PingStatus PingTimeToStatus(int pingTime) {
            if (pingTime < 100) {
                return PingStatus.Success;
            }

            if (pingTime < 200) {
                return PingStatus.Warning;
            }

            return PingStatus.Critical;
        }

        private void RecordHistory(PingSiteHistory history) {
            PingHistory.Insert(0, history);

            if (PingHistory.Count <= 100) {
                return;
            }

            for (int i = 100; i < PingHistory.Count; i++) {
                PingHistory.RemoveAt(i);
            }
        }

        private async Task<PingSiteHistory> SendPing() {
            try {
                using (Ping ping = new Ping()) {
                    PingReply reply = await ping.SendPingAsync(Location.Host);

                    if (reply == null) {
                        return new PingSiteHistory(0, PingStatus.Fail);
                    }

                    int pingTime = (int)reply.RoundtripTime;
                    if (reply.Status == IPStatus.TimedOut) {
                        return new PingSiteHistory(pingTime, PingStatus.Timeout);
                    }

                    return new PingSiteHistory(pingTime, PingTimeToStatus(pingTime));
                }
            } catch {
                // ignored
            }

            return new PingSiteHistory(0, PingStatus.Fail);
        }

        public async Task Refresh() {
            if (Refreshing) {
                return;
            }

            Refreshing = true;

            PingSiteHistory pingResult = await SendPing();

            Ping = pingResult.Ping;
            Status = pingResult.Status;

            RecordHistory(pingResult);

            Refreshing = false;
        }
    }
}