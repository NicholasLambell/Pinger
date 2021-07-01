using System;
using System.Globalization;
using System.Windows.Data;
using Pinger.Enum;

namespace Pinger.Converter {
    public class PingTimeoutToString : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (
                values.Length != 2 ||
                !(values[0] is int) ||
                !(values[1] is PingStatus)
            ) {
                return null;
            }

            PingStatus status = (PingStatus)values[1];
            if (status.ShowStatusMessage()) {
                return status.StatusMessage();
            }

            return string.Format("{0}ms", values[0]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}