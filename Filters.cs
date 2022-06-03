﻿using EcoleData.Tree;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace EcoleData
{
    public class Filters
    {
        public School SelectedSchool { get; set; }
        public string SelectedSchoolName { get; set; }
        public List<CheckBox> Floors { get; set; }
        public Dictionary<string, bool> Locations { get; set; }
        public Dictionary<string, bool> Values { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
