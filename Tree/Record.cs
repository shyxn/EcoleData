/* 
 * ETML
 * Autrice : Morgane Lebre
 * Date : du 13 mai au 8 juin 2022
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace EcoleData.Tree
{
    /// <summary>
    /// Modèle décrivant la manière de stocker un enregistrement (converti depuis un fichier CSV).
    /// </summary>
    public class Record
    {
        public int ID { get; set; }
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double DewPoint { get; set; }
        /// <summary>
        /// (ctor) Constitue un enregistrement avec les données fournies en paramètres. Ce constructeur formate les données.
        /// </summary>
        /// <param name="id">ID de l'enregistrement</param>
        /// <param name="datetime">Date de l'enregistrement en string</param>
        /// <param name="temperature">Température (non formaté).</param>
        /// <param name="humidity">Humidité (non formaté)</param>
        /// <param name="dewpoint">Point de rosée (non formaté)</param>
        public Record(string id, string datetime, string temperature, string humidity, string dewpoint)
        {
            var ci = CultureInfo.InvariantCulture;
            try
            {
                ID = Convert.ToInt32(id);
                Time = DateTime.Parse(datetime);
                Temperature = Convert.ToDouble(temperature.Split(' ').First(), ci);
                Humidity = Convert.ToDouble(humidity.Split(' ').First(), ci);
                DewPoint = Convert.ToDouble(dewpoint.Split(' ').First(), ci);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RECORD : " + ex.Message);
            }
        }
    }
}
