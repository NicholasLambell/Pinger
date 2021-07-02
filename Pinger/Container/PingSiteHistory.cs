using Pinger.Enum;

namespace Pinger.Container {
    public class PingSiteHistory {
        public int Ping { get; }
        public PingStatus Status { get; }
        public string StatusMessage { get; }

        public PingSiteHistory(int ping, PingStatus status) {
            Ping = ping;
            Status = status;
            StatusMessage = status.StatusMessage();
        }
    }
}