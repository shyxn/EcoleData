using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EcoleData
{
    public class Settings
    {
        public string FolderPath { get; set; }

        public Settings()
        {
            
        }
        public void Save(string filename)
        {
            // Check si le fichier est utilisé...
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                xmls.Serialize(sw, this);
                sw.Close();
            }
        }
        public Settings Read(string filename)
        {
            using (StreamReader sw = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                Settings newSettings = xmls.Deserialize(sw) as Settings; 
                sw.Close();
                return newSettings;
            }
        }
    }
}
