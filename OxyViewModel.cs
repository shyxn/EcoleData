using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Annotations;
using OxyPlot.Utilities;
using OxyPlot.Wpf;
using OxyPlot.Axes;
using System.Linq;
using EcoleData.Tree;

namespace EcoleData
{
    /// <summary>
    /// 
    /// </summary>
    public class OxyViewModel
    {
        public PlotModel MyModel { get; private set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Axis CelsiusYAxis { get; set; }
        public Axis PercentageYAxis { get; set; }
        public Axis DateXAxis { get; set; }
        public OxyViewModel()
        {
            this.DateXAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(StartTime),
                Maximum = DateTimeAxis.ToDouble(EndTime),
                IntervalLength = 60,
                StringFormat = String.Format(@"dd/MM/yyyy HH:mm:ss"),
                IsPanEnabled = false,
                Angle = 30
            };
            this.MyModel = new PlotModel { Title = "Relevé de sondes de [ville]" };
            this.MyModel.Axes.Add(this.DateXAxis);
        }

        public void RemoveCelsiusAxis()
        {
            this.MyModel.Axes.Remove(CelsiusYAxis);
        }
        public void ShowCelsiusAxis()
        {
            this.MyModel.Axes.Add(CelsiusYAxis);
        }
        public void RemovePercentageAxis()
        {
            this.MyModel.Axes.Remove(PercentageYAxis);
        }
        public void ShowPercentageAxis()
        {
            this.MyModel.Axes.Add(PercentageYAxis);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="sensorName"></param>
        /// <param name="value">Soit "Température", soit "Humidité", soit "Point de rosée"</param>
        /// <param name="locationValue">Soit "Salle", soit "Couloir"</param>
        /// <param name="locationObject"></param>
        public void AddLineSerie(OxyColor color, string value, string locationValue, KeyValuePair<string, Location> locationPair)
        {
            LineSeries serie = new LineSeries
            {
                Title = locationPair.Key,
                Color = color,
                StrokeThickness = 1,
                LineStyle = locationValue == "Salle" ? LineStyle.Solid : LineStyle.Dash
            };
            List<DataPoint> valueDataPoints = new List<DataPoint>();

            Func<Record, double> GetRecordProp = record => value switch
            {
                "Température" => record.Temperature,
                "Humidité" => record.Humidity,
                "Point de rosée" => record.DewPoint
            };

            locationPair.Value.Records.ForEach(record =>
            {
                double dateDouble = DateTimeAxis.ToDouble(record.Time);
                valueDataPoints.Add(new DataPoint(dateDouble, GetRecordProp(record)));
            });

            serie.Points.AddRange(valueDataPoints);
            this.MyModel.Series.Add(serie);

            // en fonction de sensorName ou de Location, peut-être définir ici quel axe doit s'afficher (
        }
        public void ClearAllSeries()
        {
            this.MyModel.Series.Clear();
        }
    }
}
