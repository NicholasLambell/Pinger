using System;
using System.Windows;
using LiveCharts;

namespace Pinger.Control {
    public partial class HistoryGraph {
        public Func<double, string> YFormatter { get; }

        public SeriesCollection GraphCollection {
            get => (SeriesCollection)GetValue(GraphCollectionProperty);
            set => SetValue(GraphCollectionProperty, value);
        }

        public static readonly DependencyProperty GraphCollectionProperty = DependencyProperty
            .Register(
                "GraphCollection",
                typeof(SeriesCollection),
                typeof(HistoryGraph),
                new FrameworkPropertyMetadata(null)
            );

        public HistoryGraph() {
            InitializeComponent();

            YFormatter = FormatYAxisLabel;
            DataContext = this;
        }

        private static string FormatYAxisLabel(double value) {
            if (value < 0) {
                return "";
            }

            return string.Format("{0:N0}ms", value);
        }
    }
}