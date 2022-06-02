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
using OxyPlot.Legends;

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
        public DateTimeAxis DateAxis { get; set; }
        public DateTimeAxis TimeAxis { get; set; }
        public LinearAxis CelsiusAxis { get; set; }
        public LinearAxis PercentageAxis { get; set; }
        public OxyViewModel()
        {
            this.DateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                IntervalLength = 60,
                StringFormat = string.Format(@"dd/MM/yyyy"),
                IsPanEnabled = false,
                IsZoomEnabled = false,
                PositionTier = 1,
                Title = "Date de l'enregistrement",
                AxisTitleDistance = 10,
                Tag = "Date"
            };
            this.TimeAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                IntervalLength = 60,
                StringFormat = string.Format(@"HH:mm:ss"),
                IsPanEnabled = false,
                IsZoomEnabled = false,
                PositionTier = 0,
                Tag = "Temps"
            };
            this.CelsiusAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Température et Point de rosée - Degrés [°C]",
                IsPanEnabled = false,
                IsZoomEnabled = false,
                AxisTitleDistance = 6,
                PositionTier = 1
            };
            this.PercentageAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Humidité - Pourcentage [%]",
                IsPanEnabled = false,
                IsZoomEnabled = false,
                AxisTitleDistance = 6,
                PositionTier = 0
            };
            this.PlotModel = new PlotModel();
            this.PlotModel.Axes.Add(this.DateAxis);
            this.PlotModel.Axes.Add(this.TimeAxis);
            this.PlotModel.Axes.Add(this.CelsiusAxis);
            this.PlotModel.Axes.Add(this.PercentageAxis);
            this.PlotModel.Legends.Add(new Legend()
            {
                LegendTitle = "Légende",
                LegendPosition = LegendPosition.RightTop,
                LegendPlacement = LegendPlacement.Outside
            });
        }

        public void RemoveCelsiusAxis()
        {
            if (this.PlotModel.Axes.Contains(this.CelsiusAxis))
                this.PlotModel.Axes.Remove(this.CelsiusAxis);
        }
        public void RemovePercentageAxis()
        {
            if (this.PlotModel.Axes.Contains(this.PercentageAxis))
                this.PlotModel.Axes.Remove(this.PercentageAxis);
        }
        public void ShowCelsiusAxis()
        {
            if (!this.PlotModel.Axes.Contains(this.CelsiusAxis))
                this.PlotModel.Axes.Add(this.CelsiusAxis);
        }
        public void ShowPercentageAxis()
        {
            if (!this.PlotModel.Axes.Contains(this.PercentageAxis))
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
                Title = locationPair.Key.Split('-')[0] + "-" + locationPair.Key.Split('-')[1] + ", " + (value == "Humidité" ? "[%]" : "[°C]"),
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

            // en fonction de value, définir ici quel axe doit s'afficher (
            if (value == "Humidité")
                this.ShowPercentageAxis();
            else
            {
                this.ShowCelsiusAxis();
            }
        }
        public void ClearAllSeries()
        {
            this.PlotModel.Series.Clear();
        }
        public void UpdateDateBounds(DateTime startTime, DateTime endTime)
        {
            PlotModel.Axes.First(axis => (string)axis.Tag == "Date").Maximum = endTime.ToOADate();
            PlotModel.Axes.First(axis => (string)axis.Tag == "Date").Minimum = startTime.ToOADate();
            PlotModel.Axes.First(axis => (string)axis.Tag == "Temps").Maximum = endTime.ToOADate();
            PlotModel.Axes.First(axis => (string)axis.Tag == "Temps").Minimum = startTime.ToOADate();
        }
    }
}
