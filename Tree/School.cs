/* 
 * ETML
 * Autrice : Morgane Lebre
 * Date : du 13 mai au 8 juin 2022
 */

using System.Collections.Generic;
using System.Linq;

namespace EcoleData.Tree
{
    /// <summary>
    /// Représente une école.
    /// </summary>
    public class School
    {
        /// <summary>
        /// Contient tous les étages existants dans cette école.
        /// </summary>
        public Dictionary<string, Floor> Floors { get; set; }
        /// <summary>
        /// (CTOR) Initialise le dictionnaire dans Floors et relaie la création de l'arborescence à un niveau plus bas.
        /// </summary>
        /// <param name="schoolPath">Le chemin du dossier de l'école.</param>
        public School(string schoolPath)
        {
            string[] floorNames = Utils.GetFoldersNames(schoolPath);

            Floors = floorNames.ToDictionary(
                name => name, // Clés : Noms des étages
                name => new Floor(schoolPath + "\\" + name)); // Valeurs : Objets de type Floor
        }
        /// <summary>
        /// Pour l'afficher dans les filtres, en dessous du nom de l'école.
        /// </summary>
        /// <returns>Le nombre recherché</returns>
        public int GetNbOfSensors()
        {
            int number = 0;
            Floors.Values.ToList().ForEach(floor => number += floor.Locations.Count);
            return number;
        }
    }
}
