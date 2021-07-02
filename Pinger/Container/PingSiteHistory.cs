using Pinger.Enum;

namespace Pinger.Container {
    public class PingSiteHistory {
        public int Ping { get; set; }
        public PingStatus Status { get; set; }
        public string StatusMessage { get; set; }

        public PingSiteHistory(int ping, PingStatus status) {
            Ping = ping;
            Status = status;
            StatusMessage = status.StatusMessage();
        }
    }
}