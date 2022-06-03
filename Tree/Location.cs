﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EcoleData.Tree
{
    /// <summary>
    /// Représente un emplacement.
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
        /// La création de l'arborescence s'arrête ici.
        /// </summary>
        public Location()
        {
        }
        /// <summary>
        /// Reconstitue les données CSV en objets Record à partir du chemin contenu dans la propriété Location.FilePath
        /// </summary>
        public void ReadCSV()
        {
            if (String.IsNullOrEmpty(this.CSVFilePath)){ return; }

            // Rappel: il faut omettre les 5 premières lignes(0 - 4) qui ne contiennent pas de données
            List<string> lines = File.ReadAllLines(this.CSVFilePath, Encoding.Latin1).Skip(5).ToList();

            // lines.Select allows me to project each line as a Person. 
            // This will give me an IEnumerable<Person> back.
            Records = lines.Select(line =>
            {
                string[] data = line.Split(',');
                return new Record(data[0], data[1], data[2], data[3], data[4]);
            }).ToList();
        }
    }
}
