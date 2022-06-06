/* 
 * ETML
 * Autrice : Morgane Lebre
 * Date : du 13 mai au 8 juin 2022
 */

using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace EcoleData
{
    /// <summary>
    /// Classe statique contenant des méthodes relatives à la récupération des noms de fichiers ou de dossiers (pour la création de l’arborescence des données) 
    /// et à la gestion des couleurs des étages de l’application (pour le logo et les lignes des graphes).
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Obtient tous les noms des dossiers directement dans un chemin donné (n'obtient pas les noms des sous-dossiers).
        /// </summary>
        /// <param name="folderPath">Le dossier dans lequel obtenir les noms.</param>
        /// <returns>Un tableau de strings contenant tous les chemins.</returns>
        public static string[] GetFoldersNames(string folderPath) => 
            Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly)
            .Select(path => path = path.Substring(folderPath.Length + 1)).ToArray();

        /// <summary>
        /// Obtient tous les noms des fichiers directement dans un chemin donné (n'obtient pas les noms dans les sous-dossiers).
        /// </summary>
        /// <param name="foldersPath">Le dossier dans lequel obtenir les noms.</param>
        /// <returns>Un tableau de strings contenant tous les chemins.</returns>
        public static string[] GetFilesNames(string foldersPath) =>
            Directory.GetFiles(foldersPath, "*.csv", SearchOption.TopDirectoryOnly)
            .Select(path => path = path.Substring(foldersPath.Length + 1)).ToArray();

        /// <summary>
        /// Obtenir la couleur correspondante en fonction de l'étage et de la nuance de couleur renseignée en paramètre. À utiliser pour les graphiques.
        /// </summary>
        /// <param name="floorNb">L'étage associé à la couleur</param>
        /// <param name="value">Filtre indiquant la nuance ("Température" -> foncée, "Humidité" -> normale, "Point de rosée" -> clair)</param>
        /// <returns>Une couleur de type OxyColor.</returns>
        public static OxyColor GetSerieColor(int floorNb, string value)
        {
            ColorFamily floorFamily = GetColorFamily(floorNb);
            return OxyColor.Parse(value switch
            {
                "Température" => floorFamily.DarkColor,
                "Humidité" => floorFamily.NormalColor,
                "Point de rosée" => floorFamily.LightColor,
            });
        }

        /// <summary>
        /// Obtient la couleur sous forme de Brush. À utiliser pour les Polygon pour le logo.
        /// </summary>
        /// <param name="floorNb">L'étage associé à la couleur</param>
        /// <param name="familyProperty">Propriété de la classe ColorFamily indiquant la nuance de couleur (DarkColor, NormalColor ou LightColor)</param>
        /// <returns>Brush à utiliser pour la génération du logo.</returns>
        public static Brush GetFaceColor(int floorNb, Func<ColorFamily, string> familyProperty) => (Brush)new BrushConverter().ConvertFromString(familyProperty(GetColorFamily(floorNb)));

        /// <summary>
        /// Obtenir la ColorFamily (le trio de couleurs) associé à l'étage renseigné en paramètre.
        /// </summary>
        /// <param name="floorNb">L'étage associé au trio de couleur.</param>
        /// <returns>Un trio de couleur (objet ColorFamily)</returns>
        private static ColorFamily GetColorFamily(int floorNb) => ColorFamilies.Where(family => family.FloorNumber == floorNb).First();
            
        /// <summary>
        /// Annuaire d'objets ColorFamily pour 10 étages. Chaque famille de couleurs est associé à un étage.
        /// </summary>
        private static List<ColorFamily> ColorFamilies { get; set; } = new()
        {
            new ColorFamily()
            {
                FloorNumber = 0,
                DarkColor = "#20637F",
                NormalColor = "#287CA1",
                LightColor = "#3198C4"
            },
            new ColorFamily()
            {
                FloorNumber = 1,
                DarkColor = "#475F74",
                NormalColor = "#587591",
                LightColor = "#6E8BA6"
            },
            new ColorFamily()
            {
                FloorNumber = 2,
                DarkColor = "#3F7570",
                NormalColor = "#4E908B",
                LightColor = "#61AAA4"
            },
            new ColorFamily()
            {
                FloorNumber = 3,
                DarkColor = "#38886F",
                NormalColor = "#45AA8A",
                LightColor = "#5CBC9E"
            },
            new ColorFamily()
            {
                FloorNumber = 4,
                DarkColor = "#73A349",
                NormalColor = "#92BD6D",
                LightColor = "#A2C781"
            },
            new ColorFamily()
            {
                FloorNumber = 5,
                DarkColor = "#F4B711",
                NormalColor = "#F7C94F",
                LightColor = "#F8D26A"
            },
            new ColorFamily()
            {
                FloorNumber = 6,
                DarkColor = "#F58A10",
                NormalColor = "#F8A74D",
                LightColor = "#F9B669"
            },
            new ColorFamily()
            {
                FloorNumber = 7,
                DarkColor = "#F7590A",
                NormalColor = "#F9854A",
                LightColor = "#FA9663"
            },
            new ColorFamily()
            {
                FloorNumber = 8,
                DarkColor = "#D7540E",
                NormalColor = "#F2722B",
                LightColor = "#F4864C"
            },
            new ColorFamily()
            {
                FloorNumber = 9,
                DarkColor = "#F80509",
                NormalColor = "#FB4145",
                LightColor = "#FC5E60"
            },
        };
    }
}
