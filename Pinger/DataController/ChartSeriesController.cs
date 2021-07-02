using System;
using LiveCharts;
using LiveCharts.Wpf;
using Pinger.Extensions;

namespace Pinger.DataController {
    public class ChartSeriesController: BindableBase {
        private const double DefaultValue = double.NaN;

        public int PointCount { get; }

        private IChartValues _chartValues;
        public IChartValues ChartValues {
            get => _chartValues;
            set => SetProperty(ref _chartValues, value);
        }

        private SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection {
            get => _seriesCollection;
            set => SetProperty(ref _seriesCollection, value);
        }

        public ChartSeriesController() : this(50) {}

        public ChartSeriesController(int pointCount) {
            PointCount = pointCount;

            ChartValues = new ChartValues<double>(new double[PointCount].Populate(DefaultValue));

            SeriesCollection = new SeriesCollection { new LineSeries { Values = ChartValues } };
        }

        private void AddPointInternal(int value) {
            ChartValues.Add(Convert.ToDouble(value));
        }

        public void Clear() {
            ChartValues.Clear();

            for (int i = 0; i < PointCount; i++) {
                ChartValues.Add(DefaultValue);
            }
        }

        public void AddPoint(int value) {
            ChartValues.RemoveAt(0);
            AddPointInternal(value);
        }

        public void AddBlankPoint() {
            ChartValues.RemoveAt(0);
            ChartValues.Add(DefaultValue);
        }
    }
}