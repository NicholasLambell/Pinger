using System;
using System.Windows.Media;

namespace Pinger.Enum {
	public enum PingStatus {
		None,
		Pinging,
		Success,
		Warning,
		Critical,
		Fail,
		Timeout
	}

	static class PingStatusExtensions {
		public static bool ShowStatusMessage(this PingStatus status) {
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (status) {
				case PingStatus.None:
				case PingStatus.Fail:
				case PingStatus.Timeout:
					return true;
				default:
					return false;
			}
		}

		public static string StatusMessage(this PingStatus status) {
			switch (status) {
				case PingStatus.None:
					return "Not Pinged";
				case PingStatus.Pinging:
					return "Currently Pinging";
				case PingStatus.Success:
					return "Responded Within 100ms";
				case PingStatus.Warning:
					return "Responded Within 200ms";
				case PingStatus.Critical:
					return "Took Longer Than 200ms To Respond";
				case PingStatus.Fail:
					return "No Response";
				case PingStatus.Timeout:
					return "Timed Out";
				default:
					throw new ArgumentOutOfRangeException(nameof(status));
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
					return Colors.Gold;
				case PingStatus.Critical:
					return Colors.Orange;
				case PingStatus.Fail:
				case PingStatus.Timeout:
					return Colors.DarkRed;
				default:
					throw new ArgumentOutOfRangeException(nameof(status));
			}
		}
	}
}