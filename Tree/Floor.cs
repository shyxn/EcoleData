using System.Collections.Generic;
using System.Linq;

namespace EcoleData.Tree
{
    /// <summary>
    /// Représente un étage.
    /// </summary>
    public class Floor
    {
        /// <summary>
        /// Contient tous les emplacements présents dans cet étage. (1 emplacement par fichier CSV).
        /// </summary>
        public Dictionary<string, Location> Locations { get; set; }
        /// <summary>
        /// (CTOR) Initialise le dictionnaire dans Floors et relaie la création de l'arborescence à un niveau plus bas.
        /// </summary>
        /// <param name="floorPath">Le chemin du dossier de l'étage.</param>
        public Floor(string floorPath)
        {
            string[] locationNames = Utils.GetFilesNames(floorPath);
            Locations = locationNames.ToDictionary(
                name => name, // Clés : Noms des emplacements (noms en entier des fichiers csv)
                name => new Location() { CSVFilePath = floorPath + "\\" + name }); // Valeurs : Objets de type Location
        }
    }
}
