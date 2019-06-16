using System;
using System.Windows.Media;

namespace Pinger.Enum {
	public enum PingStatus {
		None,
		Pinging,
		Success,
		Warning,
		Critical,
		Fail
	}

	static class PingStatusExtensions {
		public static string StatusMessage(this PingStatus status) {
			switch (status) {
				case PingStatus.None:
					return "Site not pinged";
				case PingStatus.Pinging:
					return "Currently pinging site";
				case PingStatus.Success:
					return "Site responded within 100ms";
				case PingStatus.Warning:
					return "Site responded within 200ms";
				case PingStatus.Critical:
					return "Site took longer than 200ms to respond";
				case PingStatus.Fail:
					return "Site failed to respond";
				default:
					throw new ArgumentOutOfRangeException("status");
			}
		}

		public static Color ToColor(this PingStatus status) {
			switch (status) {
				case PingStatus.None:
				case PingStatus.Pinging:
					return Colors.DarkGray;
				case PingStatus.Success:
					return Colors.Green;
				case PingStatus.Warning:
					return Colors.Yellow;
				case PingStatus.Critical:
					return Colors.Orange;
				case PingStatus.Fail:
					return Colors.DarkRed;
				default:
					throw new ArgumentOutOfRangeException("status");
			}
		}
	}
}