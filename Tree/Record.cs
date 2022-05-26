using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoleData.Tree
{
    public class Record
    {
        public int ID { get; set; }
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double DewPoint { get; set; }
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
