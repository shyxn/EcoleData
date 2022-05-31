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
        public PlotModel PlotModel { get; private set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTimeAxis DateTimeAxis { get; set; }
        public LinearAxis CelsiusAxis { get; set; }
        public LinearAxis PercentageAxis { get; set; }
        public OxyViewModel()
        {
            this.DateTimeAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                IntervalLength = 60,
                StringFormat = string.Format(@"dd/MM/yyyy HH:mm:ss"),
                IsPanEnabled = false,
                IsZoomEnabled = false,
                Angle = 30,
                Title = "Date de l'enregistrement"
            };
            this.CelsiusAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Température et Point de rosée - Degrés [°C]",
                IsPanEnabled = false,
                IsZoomEnabled = false,
                PositionTier = 1
            };
            this.PercentageAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Humidité - Pourcentage [%]",
                IsPanEnabled = false,
                IsZoomEnabled = false,
                PositionTier = 0
            };
            this.PlotModel = new PlotModel { Title = "Relevé de sondes de [ville]" };
            this.PlotModel.Axes.Add(this.DateTimeAxis);
            this.PlotModel.Axes.Add(this.CelsiusAxis);
            this.PlotModel.Axes.Add(this.PercentageAxis);
        }

        public void RemoveCelsiusAxis()
        {
            this.PlotModel.Axes.Remove(this.PlotModel.Axes.Where(axis => axis.Title == "CelsiusAxis").First());
        }
        public void ShowCelsiusAxis()
        {
            this.PlotModel.Axes.Add(this.CelsiusAxis);
        }
        public void RemovePercentageAxis()
        {
            this.PlotModel.Axes.Remove(this.PlotModel.Axes.Where(axis => axis.Title == "PercentageAxis").First());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="sensorName"></param>
        /// <param name="value">Soit "Température", soit "Humidité", soit "Point de rosée"</param>
        /// <param name="locationValue">Soit "Salle", soit "Couloir"</param>
        /// <param name="locationObject"></param>

        public void ShowPercentageAxis()
        {
            this.PlotModel.Axes.Add(this.PercentageAxis);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">Couleur de la ligne</param>
        /// <param name="value">Soit "Température", soit "Humidité", soit "Point de rosée"</param>
        /// <param name="locationValue">Soit "Salle", soit "Couloir"</param>
        /// <param name="locationPair">Paire représentant l'emplacement</param>
        /// <param name="startTime">date minimum</param>
        /// <param name="endTime">date maximum</param>
        public void AddLineSerie(OxyColor color, string value, string locationValue, KeyValuePair<string, Location> locationPair, DateTime startTime, DateTime endTime)
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
                if (record.Time >= startTime && record.Time <= endTime)
                {
                    double dateDouble = DateTimeAxis.ToDouble(record.Time);
                    valueDataPoints.Add(new DataPoint(dateDouble, GetRecordProp(record)));
                }
            });

            serie.Points.AddRange(valueDataPoints);
            this.PlotModel.Series.Add(serie);

            // en fonction de sensorName ou de Location, peut-être définir ici quel axe doit s'afficher (
        }
        public void ClearAllSeries()
        {
            this.PlotModel.Series.Clear();
        }
        public void UpdateDateBounds(DateTime startTime, DateTime endTime)
        {
            this.PlotModel.Axes.Where(axis => axis.Title == "Date de l'enregistrement").First().Maximum = endTime.ToOADate();
            this.PlotModel.Axes.Where(axis => axis.Title == "Date de l'enregistrement").First().Minimum = startTime.ToOADate();
        }
    }
}
