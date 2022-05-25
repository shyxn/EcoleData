
// TODO : En-tête

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoleData.Tree;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;

namespace EcoleData
{
    public class MainController
    {
        private MainWindow _mainWindow;
        private MainModel _mainModel;
        private OxyViewModel _mainViewModel;
        private Filters _filters;

        private string _softwareTitle;
        public string SoftwareTitle { get; set; }
        public DataSchool DataSchool { get; set; }

        public MainController(MainWindow vue)
        {
            this._mainWindow = vue;
            this._mainModel = new MainModel(this);
            this._filters = new Filters();
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
            //LIRE TOUS LES CSV( ~700k objets instanciés)
            
            try
            {
                DataSchool = new DataSchool(this._mainModel.Settings.FolderPath);
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
            this._filters.SelectedSchool = DataSchool.Schools[schoolName];

            // Nombre de capteurs
            int count = this._filters.SelectedSchool.GetNbOfSensors();
            this._mainWindow.SchoolCaptorsNb.Text = count + " capteur" + (count > 1 ? "s" : "");

            // Filtres étage
            this._mainWindow.FloorsGrid.Children.Clear();
            foreach (string floorName in this._filters.SelectedSchool.Floors.Keys)
            {
                CheckBox newCB = new CheckBox()
                {
                    Content = floorName.Split(' ')[1],
                    IsChecked = true,
                    FontWeight = System.Windows.FontWeights.Normal
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

        public void ApplyFilters()
        {
            // Configuration de _filters
            // Étages
            foreach (UIElement element in this._mainWindow.FloorsGrid.Children)
            {
                if (element is CheckBox)
                    this._filters.Floors.Add((CheckBox)element);
            }

            // Emplacements
            this._filters.Locations["Salle"] = (bool)this._mainWindow.SalleSensorCB.IsChecked;
            this._filters.Locations["Couloir"] = (bool)this._mainWindow.CouloirSensorCB.IsChecked;

            // Valeurs
            this._filters.Values["Température"] = (bool)this._mainWindow.TemperatureCB.IsChecked;
            this._filters.Values["Humidité"] = (bool)this._mainWindow.HumidityCB.IsChecked;
            this._filters.Values["Point de rosée"] = (bool)this._mainWindow.DewPointCB.IsChecked;
        }
        public void ShowGraph()
        {
            // Pour tous les étages cochés...
            foreach (CheckBox floorBox in this._filters.Floors)
            {
                this._filters.SelectedSchool.Floors.ToList().ForEach(floor =>
                {
                    // Si le nom de l'étage présent dans les données (ex. "Etage 3") contient bien le numéro de la checkbox
                    if (floor.Key.Contains(floorBox.Content.ToString()))
                    {
                        CreateSeriesForEachLocation(floor);
                    }
                });
            }
            // Actualise en même temps le graphique et les sets de données (performance? -> sinon RefreshPlot())
            this._mainViewModel.MyModel.InvalidatePlot(true);
        }

        /// <summary>
        /// Va essayer de créer une série par emplacement (si les filtres le demandent)
        /// </summary>
        /// <param name="floor">Étage contenant tous les emplacements à traiter</param>
        public void CreateSeriesForEachLocation(KeyValuePair<string, Floor> floorPair)
        {
            floorPair.Value.Locations.ToList().ForEach(location =>
            {
                string locationValue = string.Empty;
                string recordValue = string.Empty;
                if (location.Key.Contains("Salle") && this._filters.Locations["Salle"])
                {
                    locationValue = "Salle";
                }
                else if (location.Key.Contains("Couloir") && this._filters.Locations["Couloir"])
                {
                    locationValue = "Couloir";
                }
                // Pas mieux d'utiliser une liste de strings ?? Idée : faire un enum statique contenant "Température", "Humidité", et "Point de rosée"
                if (this._filters.Values["Température"])
                {
                    recordValue = "Température";   
                }
                if (this._filters.Values["Humidité"])
                {
                    recordValue = "Humidité";
                }
                if (this._filters.Values["Point de rosée"])
                {
                    recordValue = "Point de rosée";
                }

                this._mainViewModel.AddLineSerie(Utils.GetSerieColor(floorPair.Key.Split(" ").Last()), recordValue, locationValue, location);
            });
        }
    }
}
