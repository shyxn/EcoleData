/* 
 * ETML
 * Autrice : Morgane Lebre
 * Date : du 13 mai au 8 juin 2022
 */

using System.IO;
using System.Xml.Serialization;

namespace EcoleData
{
    /// <summary>
    /// Classe représentant les paramètres ainsi que les méthodes appropriées pour les sauvegarder (ou les « sérialiser ») dans un fichier XML 
    /// et les récupérer (ou les « désérialiser »). Dans ce projet, elle est étroitement utilisée avec le fichier settings.xml. 
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Chemin du dossier à sauvegarder. Le dossier contient les données de toutes les écoles.
        /// </summary>
        public string FolderPath { get; set; }
        /// <summary>
        /// (ctor)
        /// </summary>
        public Settings() { }
        /// <summary>
        /// Sérialise "this" (l'instance actuelle de Settings.cs) dans un fichier xml pour sauvegarder les paramètres.
        /// </summary>
        /// <param name="filename">Nom du fichier XML dans lequel sauvegarder.</param>
        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                xmls.Serialize(sw, this);
                sw.Close();
            }
        }
        /// <summary>
        /// Désérialise le fichier xml renseigné pour récupérer les paramètres.
        /// </summary>
        /// <param name="filename">Nom du fichier XML à partir duquel désérialiser les données.</param>
        /// <returns>Les données sauvegardées sous forme d'objet Settings</returns>
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
