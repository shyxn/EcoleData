/* 
 * ETML
 * Autrice : Morgane Lebre
 * Date : du 13 mai au 8 juin 2022
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EcoleData.Tree
{
    /// <summary>
    /// Représente un emplacement dans un étage (salle ou couloir).
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Chemin du fichier CSV.
        /// </summary>
        public string CSVFilePath { get; set; }

        /// <summary>
        /// Liste d'objets Records contenant toutes les mesures individuelles de l'emplacement.
        /// </summary>
        public List<Record> Records { get; set; }
        
        /// <summary>
        /// La création de l'arborescence s'arrête ici. Pas d'instructions à préciser dans le ctor.
        /// </summary>
        public Location() { }

        /// <summary>
        /// Reconstitue les données CSV en objets Record à partir du chemin contenu dans la propriété Location.FilePath
        /// </summary>
        public void ReadCSV()
        {
            if (String.IsNullOrEmpty(this.CSVFilePath)){ return; }

            // Il faut omettre les 5 premières lignes (0 - 4) qui ne contiennent pas de données
            List<string> lines = File.ReadAllLines(this.CSVFilePath, Encoding.Latin1).Skip(5).ToList();

            // Création de tous les enregistrements sous forme d'objets Record.
            Records = lines.Select(line =>
            {
                string[] data = line.Split(',');
                return new Record(data[0], data[1], data[2], data[3], data[4]);
            }).ToList();
        }
    }
}
