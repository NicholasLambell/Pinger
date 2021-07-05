using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pinger.Converter {
    public class ColorToBrush : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) {
                return Brushes.Transparent;
            }

            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}