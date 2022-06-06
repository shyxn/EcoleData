/* 
 * ETML
 * Autrice : Morgane Lebre
 * Date : du 13 mai au 8 juin 2022
 */

namespace EcoleData
{
    /// <summary>
    /// Petite classe représentant une association d’un étage avec 3 teintes de couleurs (normal, plus claire et plus foncée) 
    /// Permet d'être utilisée à la fois pour le logo mais aussi pour les graphes.
    /// </summary>
    public class ColorFamily
    {
        /// <summary>
        ///´Étage correspondant à la famille.
        /// </summary>
        public int FloorNumber { get; set; }
        /// <summary>
        ///´Nuance foncée de la couleur.
        /// </summary>
        public string DarkColor { get; set; }
        /// <summary>
        /// Nuance normale de la couleur.
        /// </summary>
        public string NormalColor { get; set; }
        /// <summary>
        /// Nuance claire de la couleur.
        /// </summary>
        public string LightColor { get; set; }
    }
}
