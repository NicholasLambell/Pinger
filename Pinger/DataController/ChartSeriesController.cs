using System.Windows.Media;
using LiveCharts;

namespace Pinger.DataController {
    public class ChartSeriesController : BindableBase {
        #region Props

        public int PointCount { get; }

        private SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection {
            get => _seriesCollection;
            set => SetProperty(ref _seriesCollection, value);
        }

        #endregion

        public ChartSeriesController() : this(50) {}

        public ChartSeriesController(int pointCount) {
            PointCount = pointCount;
            SeriesCollection = new SeriesCollection();
        }

        public void Clear() {
            SeriesCollection.Clear();
        }

        public ChartValuesController AddSeries(Color color) {
            ChartValuesController valuesController = new ChartValuesController(PointCount, color);

            SeriesCollection.Add(valuesController.ChartSeries);

            return valuesController;
        }

        public void RemoveSeries(ChartValuesController valuesController) {
            SeriesCollection.Remove(valuesController.ChartSeries);
        }
    }
}