/* 
 * ETML
 * Autrice : Morgane Lebre
 * Date : du 13 mai au 8 juin 2022
 */

using EcoleData.Tree;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace EcoleData
{
    /// <summary>
    /// Classe permettant, une fois instanciée, de stocker le résultat des filtres sélectionnés par l’utilisateur.
    /// </summary>
    public class Filters
    {
        /// <summary>
        /// Objet School correspondant à l'école sélectionnée.
        /// </summary>
        public School SelectedSchool { get; set; }

        /// <summary>
        /// Nom de l'école sélectionnée.
        /// </summary>
        public string SelectedSchoolName { get; set; }

        /// <summary>
        /// Liste des éléments CheckBox de l'interface.
        /// </summary>
        public List<CheckBox> Floors { get; set; }

        /// <summary>
        /// Annuaire contenant les emplacements et leur booléen associé (Salle ou Couloir)
        /// </summary>
        public Dictionary<string, bool> Locations { get; set; }

        /// <summary>
        /// Annuaire des valeurs et leur booléen associé (Température, Humidité et Point de rosée)
        /// </summary>
        public Dictionary<string, bool> Values { get; set; }
        /// <summary>
        /// Limite de date inférieure.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Limite de date supérieure.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// (ctor) Initialise les filtres par défaut. 
        /// </summary>
        public Filters()
        {
            // Paramètres par défaut
            Locations = new()
            {
                ["Salle"] = true,
                ["Couloir"] = true
            };
            Values = new()
            {
                ["Température"] = true,
                ["Humidité"] = false,
                ["Point de rosée"] = false
            };
            Floors = new();
        }
    }
}
