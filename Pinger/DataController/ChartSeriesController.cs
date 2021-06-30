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

        public void AddPoint(int value) {
            ChartValues.RemoveAt(0);
            AddPointInternal(value);
        }

        public void PopulateFromArray(int[] array) {
            ChartValues.Clear();

            int startIndex = PointCount - array.Length;
            for (int i = 0; i < PointCount; i++) {
                if (i < startIndex) {
                    ChartValues.Add(DefaultValue);
                    continue;
                }

                AddPointInternal(array[i - startIndex]);
            }
        }
    }
}