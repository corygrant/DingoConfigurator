using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot.Wpf;
using System.Windows.Media.Media3D;
using System.IO;

namespace DingoConfigurator.Plots
{
    public abstract class PlotBase
    {
        public PlotModel PlotModel { get; protected set; }
        protected Dictionary<string, LineSeries> SeriesDictionary { get; private set; }

        public double DefaultMinZoomValueAxis { get; private set; }
        public double DefaultMaxZoomValueAxis { get; private set; }

        public PlotBase(string title, bool showLegend)
        {
            PlotModel = new PlotModel
            {
                Title = title,
                TextColor = OxyColors.White,
                TitleColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White,
            };

            var legend = new Legend
            {
                LegendTitle = "Legend",
                IsLegendVisible = showLegend
            };

            PlotModel.Legends.Add(legend);

            SeriesDictionary = new Dictionary<string, LineSeries>();
        }

        protected void AddAxis(string key, AxisPosition position, string title, string unit, double absoluteMinimum, double absoluteMaximum = double.NaN)
        {
            var axis = new LinearAxis
            {
                Key = key,
                Position = position,
                Title = title,
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                MajorGridlineColor = OxyColors.White,
                MinorGridlineColor = OxyColors.White,
                AbsoluteMinimum = absoluteMinimum,
                Unit = unit
            };

            if (!double.IsNaN(absoluteMaximum))
            {
                axis.AbsoluteMaximum = absoluteMaximum;
            }

            PlotModel.Axes.Add(axis);

            if (key == "Value")
            {
                DefaultMinZoomValueAxis = axis.AbsoluteMinimum;
                DefaultMaxZoomValueAxis = axis.AbsoluteMaximum;
            }
        }

        public void AddSeries(string key, string title, LineStyle lineStyle = LineStyle.Solid)
        {
            if (!SeriesDictionary.ContainsKey(key))
            {
                var series = new LineSeries { Title = title, LineStyle = lineStyle };
                SeriesDictionary[key] = series;
                PlotModel.Series.Add(series);
            }
        }

        public void AddPoint(string key, double time, double val)
        {
            if (SeriesDictionary.ContainsKey(key))
            {
                SeriesDictionary[key].Points.Add(new DataPoint(time, val));
            }
        }

        public void RemoveSeries(string key)
        {
            if (SeriesDictionary.ContainsKey(key))
            {
                PlotModel.Series.Remove(SeriesDictionary[key]);
                SeriesDictionary.Remove(key);
            }
        }

        public void ClearData()
        {
            foreach (var series in SeriesDictionary.Values)
            {
                series.Points.Clear();
            }
        }

        public void Refresh()
        {
            PlotModel.InvalidatePlot(true);
        }

        public void UpdateTimeAxisZoom(double minimum, double maximum)
        {
            var timeAxis = PlotModel.Axes.FirstOrDefault(a => a.Key == "Time");
            if (timeAxis != null)
            {
                timeAxis.Zoom(minimum, maximum);
                PlotModel.InvalidatePlot(false);
            }
        }

        public void UpdateValueAxisZoom(double minimum, double maximum)
        {
            var valueAxis = PlotModel.Axes.FirstOrDefault(a => a.Key == "Value");
            if (valueAxis != null)
            {
                valueAxis.Zoom(minimum, maximum);
                PlotModel.InvalidatePlot(false);
            }
        }

        public void ResetValueZoom()
        {
            UpdateValueAxisZoom(DefaultMinZoomValueAxis, DefaultMaxZoomValueAxis);
        }

        public void Export(string folderPath)
        {
            string fullPath = folderPath + "\\" + PlotModel.Title + ".png";
            var pngExporter = new PngExporter { Width = 1920, Height = 400 };
            pngExporter.ExportToFile(PlotModel, fullPath);
        }
    }
}
