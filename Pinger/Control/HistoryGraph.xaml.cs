using System;
using System.Windows;
using LiveCharts;

namespace Pinger.Control {
	public partial class HistoryGraph {
		public Func<double, string> YFormatter {get; set;}

		public SeriesCollection GraphCollection {
			get=>(SeriesCollection)GetValue(GraphCollectionProperty);
			set=>SetValue(GraphCollectionProperty, value);
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

			YFormatter = value => string.Format("{0} ms", value);
			DataContext = this;
		}
	}
}