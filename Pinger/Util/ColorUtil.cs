using System.Windows.Media;

namespace Pinger.Util {
    public static class ColorUtil {
        public static string ToHex(Color color) {
            // ColorConverter.ConvertFromString expects hex values to be ARGB so we must save it as such
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
        }

        public static Color FromHex(string hex) {
            return (Color?)ColorConverter.ConvertFromString(hex) ?? Colors.Transparent;
        }
    }
}