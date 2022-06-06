/* 
 * ETML
 * Autrice : Morgane Lebre
 * Date : du 13 mai au 8 juin 2022
 */

using EcoleData.Tree;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcoleData
{
    /// <summary>
    /// Contient toutes les méthodes relatives à la librairie Oxyplot pour le projet.
    /// Toutes les configurations du graphique, de ses axes, de ses séries de données, etc. se retrouvent ici.
    /// </summary>
    public class OxyViewModel
    {
        /// <summary>
        /// Objet contenant toutes les configurations du graphique Oxyplot.
        /// </summary>
        public PlotModel PlotModel { get; private set; }
        /// <summary>
        /// Axe (abcisse) échelonnant le DateTime de l'enregistrement (formaté en date).
        /// </summary>
        public DateTimeAxis DateAxis { get; set; }
        /// <summary>
        /// Axe (abcisse) échelonnant la DateTime de l'enregistrement (formaté en heure).
        /// </summary>
        public DateTimeAxis TimeAxis { get; set; }
        /// <summary>
        /// Axe (ordonnée) échelonnant les enregistrements sur les degrés Celsius (pour la température et le point de rosée)
        /// </summary>
        public LinearAxis CelsiusAxis { get; set; }
        /// <summary>
        /// Axe (ordonnée) échelonnant les enregistrements sur les pourcentages (pour l'humidité).
        /// </summary>
        public LinearAxis PercentageAxis { get; set; }
        /// <summary>
        /// (ctor) Initialisation de tous les axes (en propriétés) et ajout de ceux-ci au PlotModel.
        /// </summary>
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
        /// <summary>
        /// Permet de supprimer l'axe des degrés Celsius de la configuration du PlotModel.
        /// </summary>
        public void RemoveCelsiusAxis()
        {
            if (this.PlotModel.Axes.Contains(this.CelsiusAxis))
                this.PlotModel.Axes.Remove(this.CelsiusAxis);
        }
        /// <summary>
        /// Permet de supprimer l'axe des pourcentages de la configuration du PlotModel.
        /// </summary>
        public void RemovePercentageAxis()
        {
            if (this.PlotModel.Axes.Contains(this.PercentageAxis))
                this.PlotModel.Axes.Remove(this.PercentageAxis);
        }
        /// <summary>
        /// Permet d'ajouter l'axe des degrés Celsius à la configuration du PlotModel - Permet ainsi de l'afficher à la prochaine actualisation.
        /// </summary>
        public void ShowCelsiusAxis()
        {
            if (!this.PlotModel.Axes.Contains(this.CelsiusAxis))
                this.PlotModel.Axes.Add(this.CelsiusAxis);
        }
        /// <summary>
        /// Permet d'ajouter l'axe des pourcentages à la configuration du PlotModel - Permet ainsi de l'afficher à la prochaine actualisation.
        /// </summary>
        public void ShowPercentageAxis()
        {
            if (!this.PlotModel.Axes.Contains(this.PercentageAxis))
                this.PlotModel.Axes.Add(this.PercentageAxis);
        }
        
        /// <summary>
        /// Ajoute une LineSerie à la configuration - Une ligne du graphique représentant un lot de données (pour une valeur à la fois ; Température, Point de Rosée ou Humidité)
        /// </summary>
        /// <param name="color">Couleur de la ligne</param>
        /// <param name="value">Soit "Température", soit "Humidité", soit "Point de rosée"</param>
        /// <param name="locationValue">Soit "Salle", soit "Couloir"</param>
        /// <param name="locationPair">Paire représentant l'emplacement</param>
        /// <param name="startTime">Date minimum</param>
        /// <param name="endTime">Date maximum</param>
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

            // En fonction de value, définir ici quel axe doit s'afficher
            if (value == "Humidité")
                this.ShowPercentageAxis();
            else
            {
                this.ShowCelsiusAxis();
            }
        }
        /// <summary>
        /// Vider le graphique de toutes ses séries.
        /// </summary>
        public void ClearAllSeries()
        {
            this.PlotModel.Series.Clear();
        }
        /// <summary>
        /// Actualise les dates limites à afficher pour les axes temporels.
        /// </summary>
        /// <param name="startTime">Date minimum</param>
        /// <param name="endTime">Date maximum</param>
        public void UpdateDateBounds(DateTime startTime, DateTime endTime)
        {
            PlotModel.Axes.First(axis => (string)axis.Tag == "Date").Maximum = endTime.ToOADate();
            PlotModel.Axes.First(axis => (string)axis.Tag == "Date").Minimum = startTime.ToOADate();
            PlotModel.Axes.First(axis => (string)axis.Tag == "Temps").Maximum = endTime.ToOADate();
            PlotModel.Axes.First(axis => (string)axis.Tag == "Temps").Minimum = startTime.ToOADate();
        }
    }
}
