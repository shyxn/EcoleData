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
    /// Représente l'ensemble des écoles contenant des données pour le projet.
    /// </summary>
    public class DataSchool
    {
        /// <summary>
        /// Contient toutes les écoles existantes
        /// </summary>
        public Dictionary<string, School> Schools { get; set; }

        /// <summary>
        /// (ctor) Initialise le dictionnaire dans Schools et relaie la création de l'arborescence à un niveau plus bas.
        /// </summary>
        /// <param name="schoolPath">Le chemin du dossier de toutes les écoles.</param>
        public DataSchool(string foldersPath)
        {
            string[] schoolNames = Utils.GetFoldersNames(foldersPath);

            Schools = schoolNames.ToDictionary(
                    name => name, // Clés : Noms des écoles
                    name => new School(foldersPath + "\\" + name)); // Valeurs : Objets de type School
        }
    }
}
