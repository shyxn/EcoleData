
// TODO : En-tête

using EcoleData.Tree;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            this._mainViewModel = new OxyViewModel();
            this._filters = new Filters();
            this._mainViewModel.PlotModel.TitlePadding = 10;
            this._mainWindow.PlotView.Model = this._mainViewModel.PlotModel;
            this._mainWindow.PlotView.Visibility = Visibility.Hidden;
            this._mainViewModel.PlotModel.DefaultFont = "Roboto";
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
                this._mainWindow.folderNameTB.Text = "Dossier inexistant ou invalide.";
                return;
            }
            #nullable disable
            // Le chemin du dossier existe.
            this._mainWindow.folderNameTB.Text = folderPath;
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                this._mainWindow.folderNameTB.Text = "Dossier inexistant ou invalide.";
                return;
            }
            
            // Attention, ne vérifie pas la validité des fichiers CSV...
            if (!CheckTreeValidity())
            {
                this._mainWindow.folderNameTB.Text = "Dossier inexistant ou invalide.";
                return;
            }
            Debug.WriteLine("[LoadTree()] Arborescence terminée et validée.");
            UpdateComboBox();
        }

        private bool CheckTreeValidity()
        {
            return DataSchool.Schools.Count > 0
                && DataSchool.Schools.Values.ToList()
                .TrueForAll(school => school.Floors.Count > 0 
                    && school.Floors.Values.ToList()
                    .TrueForAll(floor => floor.Locations.Count > 0));
        }

        private void UpdateComboBox()
        {
            this._mainWindow.schoolsComboBox.Items.Clear();
            DataSchool.Schools.Keys.ToList().ForEach(schoolName => this._mainWindow.schoolsComboBox.Items.Add(schoolName));
            this._mainWindow.schoolsComboBox.IsEnabled = true;
        }

        /// <summary>
        /// Au moment où une école est choisie.
        /// </summary>
        public void UpdateFilters(string schoolName)
        {
            this._filters.SelectedSchool = DataSchool.Schools[schoolName];
            this._filters.SelectedSchoolName = schoolName;
            // Logo des étages
            this._mainWindow.PrintFloorLogo(this._filters.SelectedSchool.Floors.Count);

            // Nombre de capteurs
            int count = this._filters.SelectedSchool.GetNbOfSensors();
            this._mainWindow.schoolCaptorsNb.Text = count + " capteur" + (count > 1 ? "s" : "");

            // Filtres étage
            this._mainWindow.floorsGrid.Children.Clear();
            foreach (string floorName in this._filters.SelectedSchool.Floors.Keys)
            {
                CheckBox newCB = new CheckBox()
                {
                    Content = floorName.Split(' ')[1],
                    IsChecked = floorName == "Etage 0" ? true : false,
                    FontWeight = System.Windows.FontWeights.Normal
                };
                Grid.SetRow(newCB, Convert.ToInt32(newCB.Content) / 5);
                Grid.SetColumn(newCB, Convert.ToInt32(newCB.Content) % 5);
                this._mainWindow.floorsGrid.Children.Add(newCB);
                this._filters.Floors.Add(newCB);
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

            this._mainWindow.startDatePicker.SelectedDate = this._mainModel.MinDate = this._filters.StartDate = minStartDate;
            this._mainWindow.endDatePicker.SelectedDate = this._mainModel.MaxDate = this._filters.EndDate = maxEndDate;
            this._mainViewModel.UpdateDateBounds(minStartDate, maxEndDate);

            this.SetDefaultFilters();
        }

        /// <summary>
        /// Ne change pas l'école sélectionnée.
        /// </summary>
        public void SetDefaultFilters()
        {
            // Réinitialiser les checkboxes
            foreach (CheckBox? element in this._mainWindow.floorsGrid.Children)
            {
                element.IsChecked = element.Content.ToString() == "0" ? true : false;
            }

            // Réinitialiser les emplacements
            this._mainWindow.salleSensorCB.IsChecked = true;
            this._mainWindow.couloirSensorCB.IsChecked = false;

            // Réinitialiser les valeurs
            this._mainWindow.temperatureCB.IsChecked = true;
            this._mainWindow.humidityCB.IsChecked = false;
            this._mainWindow.dewPointCB.IsChecked = false;

            // Réinitialiser les dates
            this._mainWindow.startDatePicker.SelectedDate = this._mainModel.MinDate;
            this._mainWindow.endDatePicker.SelectedDate = this._mainModel.MaxDate;
            this._mainViewModel.UpdateDateBounds(this._mainModel.MinDate, this._mainModel.MaxDate);
            CheckAndApplyFilters();
        }
        public void CheckAndApplyFilters()
        {
            // Configuration de _filters
            SaveAllFilters();
            if (AreFiltersCorrect())
            {
                // Affichage des graphiques
                ShowGraph();
            }
        }
        private bool AreFiltersCorrect()
        {
            this._mainWindow.HideAllFiltersMessages();
            // Les conditions sont écrites de manière "Vrai si rien n'est coché"
            // Si aucun étage est coché
            if (this._filters.Floors.TrueForAll(checkbox => !(bool)checkbox.IsChecked))
            {
                this._mainWindow.floorsFiltersErrorMsg.Visibility = Visibility.Visible;
            }

            // Si aucun emplacement est coché
            if (this._filters.Locations.Values.ToList().TrueForAll(isChecked => !isChecked))
            {
                this._mainWindow.locationsFilterErrorMsg.Visibility = Visibility.Visible;
            }

            // Si aucune valeur est cochée
            if (this._filters.Values.Values.ToList().TrueForAll(isChecked => !isChecked))
            {
                this._mainWindow.valuesFiltersErrorMsg.Visibility = Visibility.Visible;
            }

            // Correction automatique si les dates dépassent les valeurs limites
            if (this._filters.EndDate > this._mainModel.MaxDate)
            {
                this._mainWindow.endDatePicker.SelectedDate = this._filters.EndDate = this._mainModel.MaxDate;
            }
            
            if (this._filters.StartDate < this._mainModel.MinDate)
            {
                this._mainWindow.startDatePicker.SelectedDate = this._filters.StartDate = this._mainModel.MinDate;
            }

            // Si la date minimale est plus récente que la date maximale
            if (this._filters.EndDate <= this._filters.StartDate)
            {
                this._mainWindow.datesFilterErrorMsg.Text = "La date de départ ne peut pas être supérieure ou égale à la date de fin.";
                this._mainWindow.datesFilterErrorMsg.Visibility = Visibility.Visible;
            }

            // Si tous les messages d'erreur sont cachés, les filtres sont validés
            if (this._mainWindow.AllFiltersErrorMessages.TrueForAll(message => message.Visibility == Visibility.Collapsed))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Récolte l'état des checkboxes et des dates pour les enregistrer dans MainController._filters.
        /// </summary>
        private void SaveAllFilters()
        {
            // Étages
            if (this._filters.Floors is not null) { this._filters.Floors.Clear(); };
            foreach (UIElement element in this._mainWindow.floorsGrid.Children)
            {
                if (element is CheckBox)
                    this._filters.Floors.Add((CheckBox)element);
            }
            // Emplacements
            this._filters.Locations["Salle"] = (bool)this._mainWindow.salleSensorCB.IsChecked;
            this._filters.Locations["Couloir"] = (bool)this._mainWindow.couloirSensorCB.IsChecked;

            // Valeurs
            this._filters.Values["Température"] = (bool)this._mainWindow.temperatureCB.IsChecked;
            this._filters.Values["Humidité"] = (bool)this._mainWindow.humidityCB.IsChecked;
            this._filters.Values["Point de rosée"] = (bool)this._mainWindow.dewPointCB.IsChecked;

            // Dates
            this._filters.StartDate = (DateTime)this._mainWindow.startDatePicker.SelectedDate;
            this._filters.EndDate = (DateTime)this._mainWindow.endDatePicker.SelectedDate;
        }
      
        public void ShowGraph()
        {
            this._mainViewModel.ClearAllSeries();
            this._mainWindow.graphPlace.Visibility = Visibility.Visible;

            this._mainViewModel.PlotModel.Title = "École de " + this._filters.SelectedSchoolName;
            
            // Pour tous les étages cochés...
            foreach (CheckBox floorBox in this._filters.Floors.Where(checkbox => (bool)checkbox.IsChecked))
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
            this._mainViewModel.UpdateDateBounds(this._filters.StartDate, this._filters.EndDate);

            // Actualise en même temps le graphique et les sets de données (performance?)
            // Todo : https://blog.bartdemeyer.be/2013/03/creating-graphs-in-wpf-using-oxyplot/
            this._mainWindow.PlotView.InvalidatePlot(true);
                
            this._mainWindow.PlotView.Visibility = Visibility.Visible;
            if (this._mainViewModel.PlotModel.Series.Count == 0)
            {
                this._mainWindow.noDataMsg.Visibility = Visibility.Visible;
                this._mainWindow.PlotView.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Va essayer de créer une série par emplacement (si les filtres le demandent)
        /// </summary>
        /// <param name="floorPair">Étage contenant tous les emplacements à traiter</param>
        public void CreateSeriesForEachLocation(KeyValuePair<string, Floor> floorPair)
        {
            floorPair.Value.Locations.ToList().ForEach(location =>
            {
                // string indiquant littéralement l'emplacement ("Salle" ou "Couloir")
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

                if (string.IsNullOrEmpty(locationValue))
                {
                    return;
                }
                this._mainViewModel.RemoveCelsiusAxis();
                this._mainViewModel.RemovePercentageAxis();
                // dataValue.Key est le nom littéral de la valeur ("Température" p.ex) et dataValue.Value est le booléen qui indique si la valeur est cochée dans les filtres
                this._filters.Values.ToList().ForEach(dataValue =>
                {
                    // Ajouter chaque valeur souhaitée (valeur du dictionnaire à true)
                    if (dataValue.Value)
                    {
                        OxyColor color = Utils.GetSerieColor(Convert.ToInt32(floorPair.Key.Split(" ").Last()), dataValue.Key);
                        
                        this._mainViewModel.AddLineSerie(color, dataValue.Key, locationValue, location, this._filters.StartDate, this._filters.EndDate);
                    }
                });
            });
        }
    }
}
