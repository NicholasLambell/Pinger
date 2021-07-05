using System;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using Pinger.Extensions;

namespace Pinger.DataController {
    public class ChartValuesController {
        private const double DefaultValue = double.NaN;

        #region Props

        private IChartValues ChartValues { get; }

        public ISeriesView ChartSeries { get; }

        #endregion

        public ChartValuesController(int pointCount, Color color) {
            SolidColorBrush fillBrush = new SolidColorBrush(color);
            fillBrush.Opacity = 0.15;

            ChartValues = new ChartValues<double>(new double[pointCount].Populate(DefaultValue));
            ChartSeries = new LineSeries {
                Values = ChartValues,
                Stroke = new SolidColorBrush(color),
                Fill = fillBrush,
            };
        }

        private void AddPointInternal(int value) {
            ChartValues.Add(Convert.ToDouble(value));
        }

        public void Clear() {
            for (int i = 0; i < ChartValues.Count; i++) {
                ChartValues.Insert(i, DefaultValue);
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