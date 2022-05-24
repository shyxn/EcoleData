using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoleData.Tree;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EcoleData
{
    public class MainController
    {
        private MainWindow _mainWindow;
        private MainModel _mainModel;

        private string _softwareTitle;
        public string SoftwareTitle { get; set; }
        public DataSchool DataSchool { get; set; }
        public MainController(MainWindow vue)
        {
            this._mainWindow = vue;
            this._mainModel = new MainModel(this);
        }

        public void SetNewFolderPath(string folderPath) => this._mainModel.SetNewFolderPath(folderPath);

        /// <summary>
        /// Permet de vérifier si le dossier existe toujours, si oui, appelle la méthode pour créer l'arborescence de données.
        /// </summary>
        public void CheckFolderValidity()
        {
            #nullable enable
            string? folderPath = this._mainModel.Settings.FolderPath;

            if (!Directory.Exists(folderPath))
            {
                this._mainWindow.FolderNameDisplayer.Text = "Dossier inexistant ou invalide.";
                return;
            }
            #nullable disable
            // Le chemin du dossier existe.
            this._mainWindow.FolderNameDisplayer.Text = folderPath;
            LoadTree();
        }
        public void LoadTree() // Le chemin du dossier sera toujours valide ici.
        {
            Debug.WriteLine("[LoadTree()] Chargement de l'arborescence des données...");
            try
            {
                DataSchool = new DataSchool(this._mainModel.Settings.FolderPath);

                //LIRE TOUS LES CSV( ~700k objets instanciés)
                DataSchool.Schools.ToList()
                .ForEach(x => x.Value.Floors.ToList()
                    .ForEach(x => x.Value.Locations.ToList().ForEach(x => x.Value.ReadCSV())));
            }
            catch (Exception)
            {
                this._mainWindow.FolderNameDisplayer.Text = "Dossier inexistant ou invalide.";
                return;
            }
            
            // Attention, ne vérifie pas la validité des fichiers CSV...
            if (!CheckTreeValidity())
            {
                this._mainWindow.FolderNameDisplayer.Text = "Dossier inexistant ou invalide.";
                return;
            }
            Debug.WriteLine("[LoadTree()] Arborescence terminée et validée.");
            UpdateComboBox();
        }

        private bool CheckTreeValidity()
        {
            return DataSchool.Schools.Count > 0
                && DataSchool.Schools.Values.ToList()
                .TrueForAll(school => school.Floors.Count > 0 && school.Floors.Values.ToList()
                    .TrueForAll(floor => floor.Locations.Count > 0));
        }

        private void UpdateComboBox()
        {
            this._mainWindow.SchoolsComboBox.Items.Clear();
            DataSchool.Schools.Keys.ToList().ForEach(schoolName => this._mainWindow.SchoolsComboBox.Items.Add(schoolName));
        }

        public void UpdateFilters(string schoolName)
        {
            // Nombre de capteurs
            int count = DataSchool.Schools[schoolName].GetNbOfSensors();
            this._mainWindow.SchoolCaptorsNb.Text = count + " capteur" + (count > 1 ? "s" : "");

            // Filtres étage
            foreach (string floorName in DataSchool.Schools[schoolName].Floors.Keys)
            {
                CheckBox newCB = new CheckBox()
                {
                    Content = floorName.Split(' ')[1],
                    IsChecked = true,
                    FontWeight = FontWeights.Normal
                };
                Grid.SetRow(newCB, Convert.ToInt32(newCB.Content) / 5);
                Grid.SetColumn(newCB, Convert.ToInt32(newCB.Content) % 5);
                this._mainWindow.FloorsGrid.Children.Add(newCB);
            }

            // Filtres Plage de date
            DateTime minStartDate;
            DateTime maxEndDate = minStartDate = DataSchool.Schools[schoolName].Floors.Values.First().Locations.Values.First().Records.First().Time;
            DataSchool.Schools[schoolName].Floors.Values.ToList().ForEach(floor => floor.Locations.Values.ToList().ForEach(location =>
            {
                DateTime minDate = location.Records.Min(i => i.Time);
                DateTime maxDate = location.Records.Max(i => i.Time);
                if (minDate < minStartDate)
                    minStartDate = minDate;
                if (maxDate > maxEndDate)
                    maxEndDate = maxDate;
            }));

            this._mainWindow.StartDatePicker.SelectedDate = minStartDate;
            this._mainWindow.EndDatePicker.SelectedDate = maxEndDate;

        }
    }
}
