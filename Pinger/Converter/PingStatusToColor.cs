using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Pinger.Enum;

namespace Pinger.Converter {
	public class PingStatusToColor : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value == null ? Brushes.Transparent : new SolidColorBrush(((PingStatus)value).ToColor());
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}