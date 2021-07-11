using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Pinger.Const {
    public static class ChartSeriesColors {

        private static Random _random = new Random();

        #region https://github.com/Live-Charts/Live-Charts/blob/80b687adfad0781703efb7355cfce957d4c22ca2/WpfView/Charts/Base/Chart.cs#L85

        private static IEnumerable<Color> ChartDefaults { get; } = new[] {
            Color.FromRgb(33, 149, 242),
            Color.FromRgb(243, 67, 54),
            Color.FromRgb(254, 192, 7),
            Color.FromRgb(96, 125, 138),
            Color.FromRgb(0, 187, 211),
            Color.FromRgb(232, 30, 99),
            Color.FromRgb(254, 87, 34),
            Color.FromRgb(63, 81, 180),
            Color.FromRgb(204, 219, 57),
            Color.FromRgb(0, 149, 135),
            Color.FromRgb(76, 174, 80),
        };

        #endregion

        #region https://github.com/Microsoft/referencesource/blob/master/System.Web.DataVisualization/Common/Utilities/ColorPalette.cs

        private static IEnumerable<Color> ColorsDefault { get; } = new[] {
            Colors.Green,
            Colors.Blue,
            Colors.Purple,
            Colors.Lime,
            Colors.Fuchsia,
            Colors.Teal,
            Colors.Yellow,
            Colors.Gray,
            Colors.Aqua,
            Colors.Navy,
            Colors.Maroon,
            Colors.Red,
            Colors.Olive,
            Colors.Silver,
            Colors.Tomato,
            Colors.Moccasin,
        };

        private static IEnumerable<Color> ColorsExcel { get; } = new[] {
            Color.FromRgb(153, 153, 255),
            Color.FromRgb(153, 51, 102),
            Color.FromRgb(255, 255, 204),
            Color.FromRgb(204, 255, 255),
            Color.FromRgb(102, 0, 102),
            Color.FromRgb(255, 128, 128),
            Color.FromRgb(0, 102, 204),
            Color.FromRgb(204, 204, 255),
            Color.FromRgb(0, 0, 128),
            Color.FromRgb(255, 0, 255),
            Color.FromRgb(255, 255, 0),
            Color.FromRgb(0, 255, 255),
            Color.FromRgb(128, 0, 128),
            Color.FromRgb(128, 0, 0),
            Color.FromRgb(0, 128, 128),
            Color.FromRgb(0, 0, 255),
        };

        private static IEnumerable<Color> ColorsBrightPastel { get; } = new[] {
            Color.FromRgb(65, 140, 240),
            Color.FromRgb(252, 180, 65),
            Color.FromRgb(224, 64, 10),
            Color.FromRgb(5, 100, 146),
            Color.FromRgb(191, 191, 191),
            Color.FromRgb(26, 59, 105),
            Color.FromRgb(255, 227, 130),
            Color.FromRgb(18, 156, 221),
            Color.FromRgb(202, 107, 75),
            Color.FromRgb(0, 92, 219),
            Color.FromRgb(243, 210, 136),
            Color.FromRgb(80, 99, 129),
            Color.FromRgb(241, 185, 168),
            Color.FromRgb(224, 131, 10),
            Color.FromRgb(120, 147, 190),
        };

        private static IEnumerable<Color> ColorsPastel { get; } = new[] {
            Colors.SkyBlue,
            Colors.LimeGreen,
            Colors.MediumOrchid,
            Colors.LightCoral,
            Colors.SteelBlue,
            Colors.YellowGreen,
            Colors.Turquoise,
            Colors.HotPink,
            Colors.Khaki,
            Colors.Tan,
            Colors.DarkSeaGreen,
            Colors.CornflowerBlue,
            Colors.Plum,
            Colors.CadetBlue,
            Colors.PeachPuff,
            Colors.LightSalmon,
        };

        #endregion

        public static Color[] All { get; } = ChartDefaults
            .Union(ColorsDefault)
            .Union(ColorsExcel)
            .Union(ColorsBrightPastel)
            .Union(ColorsPastel)
            .ToArray();

        public static Color Random() {
            return All[_random.Next(0, All.Length)];
        }
    }
}
