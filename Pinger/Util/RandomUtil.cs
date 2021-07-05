using System;
using System.Windows.Media;

namespace Pinger.Util {
    public static class RandomUtil {
        private static Random _random = new Random();

        public static byte NextByte() {
            return Convert.ToByte(_random.Next(0, 255));
        }

        public static Color NextColor() {
            return Color.FromRgb(NextByte(), NextByte(), NextByte());
        }
    }
}