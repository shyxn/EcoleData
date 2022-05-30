using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot.Wpf;

namespace EcoleData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainController _controller;
        private List<UIElement> _elementsToHideIfNoFolder;
        public List<TextBlock> AllFiltersErrorMessages { get; set; }
        public PlotView PlotView { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.PlotView = new PlotView() { Padding = new Thickness(0, 0, 200, 0)};
            this.GraphPlace.Children.Add(this.PlotView);
            this._controller = new MainController(this);

            this._elementsToHideIfNoFolder = new()
            {
                this.SchoolTitle,
                this.SchoolCaptorsNb,
                this.ResetFiltersBtn    
            };
            this.AllFiltersErrorMessages = new()
            {
                this.DatesFilterErrorMessage,
                this.FloorsFiltersErrorMessage,
                this.LocationsFilterErrorMessage,
                this.ValuesFilterErrorMessage
            };
            HideUIElements();
        }

        private void ChooseFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            // https://stackoverflow.com/a/58569707
            var dlg = new CommonOpenFileDialog();
            dlg.Title = this._controller.SoftwareTitle;
            dlg.IsFolderPicker = true;
            
            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedFolderName = dlg.FileName;
                // Afficher le chemin du dossier
                this._controller.SetNewFolderPath(selectedFolderName);
                this._controller.CheckFolderValidity();
            }
        }

        private void SchoolsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowUIElements();
            string selectedSchoolName = ((ComboBox)sender).SelectedItem.ToString();
            this.SchoolTitle.Text = "École de " + selectedSchoolName;
            
            this._controller.UpdateFilters(selectedSchoolName);
        }

        public void HideUIElements() => this._elementsToHideIfNoFolder.ForEach(element => element.Visibility = Visibility.Hidden);
        public void ShowUIElements() => this._elementsToHideIfNoFolder.ForEach(element => element.Visibility = Visibility.Visible);
        public void HideAllFiltersMessages() => this.AllFiltersErrorMessages.ForEach(message => message.Visibility = Visibility.Collapsed);

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Debug.WriteLine("La fenêtre a fini de charger. Recherche des données...");
            this.loadHintTextBox.Visibility = Visibility.Visible;
            this._controller.CheckFolderValidity();
            this.loadHintTextBox.Visibility = Visibility.Hidden;
            Debug.WriteLine("Fin de la recherche des données.");
        }

        private void ApplyFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            this._controller.CheckAndApplyFilters();
        }

        private void ResetFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            this._controller.SetDefaultFilters();
        }
    }
}
