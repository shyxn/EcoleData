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
        private List<Polygon> _allRectanglePolygons { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.PlotView = new PlotView() { Padding = new Thickness(0, 0, 200, 0)};
            this.PlotView.IsVisibleChanged += PlotView_IsVisibleChanged;
            this.GraphPlace.Children.Add(this.PlotView);
            this._controller = new MainController(this);
            this.LoadMessageTextBox.LayoutUpdated += LoadMessageTextBox_LayoutUpdated;
            this._allRectanglePolygons = new();

            this._elementsToHideIfNoFolder = new()
            {
                this.SchoolTitle,
                this.SchoolCaptorsNb,
                this.ResetFiltersBtn,
                this.PlotView
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

        private void LoadMessageTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.LoadMessageTextBox.Visibility == Visibility.Visible)
            {
                // Commencer ici le chargement des données
                this._controller.CheckFolderValidity();
                this.LoadMessageTextBox.Visibility = Visibility.Hidden;
                Debug.WriteLine("Fin de la recherche des données.");
            }
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
                // Afficher le chemin du dossier
                this._controller.SetNewFolderPath(dlg.FileName);
                ClearAllUIElements();
            }
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Debug.WriteLine("La fenêtre a fini de charger. Recherche des données...");
            this.LoadMessageTextBox.Visibility = Visibility.Visible;
        }
        private void SchoolsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Items.Count > 0)
            {
                ShowUIElements();
                string selectedSchoolName = ((ComboBox)sender).SelectedItem.ToString();
                this.SchoolTitle.Text = "École de " + selectedSchoolName;

                this._controller.UpdateFilters(selectedSchoolName);
            }
        }

        public void HideUIElements() => this._elementsToHideIfNoFolder.ForEach(element => element.Visibility = Visibility.Hidden);
        public void ShowUIElements() => this._elementsToHideIfNoFolder.ForEach(element => element.Visibility = Visibility.Visible);
        public void HideAllFiltersMessages() => this.AllFiltersErrorMessages.ForEach(message => message.Visibility = Visibility.Collapsed);

        /// <summary>
        /// Pour quand la sélection de dossiers change.
        /// Commence à tout charger et finit par afficher le message de chargement des données. (c'est son apparition qui déclenchera la suite)
        /// </summary>
        public void ClearAllUIElements()
        {
            // Supprimer tous les polygones
            this.LogoPlace.Children.Clear();
            // Supprimer tous les checkboxes étage
            this.FloorsGrid.Children.Clear();
            // Décocher toutes les checkboxes
            this.SalleSensorCB.IsChecked = false;
            this.CouloirSensorCB.IsChecked = false;
            this.TemperatureCB.IsChecked = false;
            this.DewPointCB.IsChecked = false;
            this.HumidityCB.IsChecked = false;
            this.SchoolTitle.Text = "";
            this.SchoolCaptorsNb.Text = "";
            // Désafficher le graphique. 
            this.GraphPlace.Visibility = Visibility.Hidden;
            // Vider le combobox
            this.SchoolsComboBox.Items.Clear();
            // Afficher le chargement des données
            this.LoadMessageTextBox.Visibility = Visibility.Visible;
        }

        // Affiche le message indiquant qu'il n'y a aucune donnée si le PlotView venait à être invisible
        private void PlotView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) =>
            this.NoDataLabel.Visibility = this.PlotView.Visibility != Visibility.Visible ? Visibility.Visible : Visibility.Hidden;

        private void ApplyFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            this._controller.CheckAndApplyFilters();
        }

        private void ResetFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            this._controller.SetDefaultFilters();
        }

        public void PrintFloorLogo(int floorsNb)
        {
            this.LogoPlace.Children.Clear();
            this._allRectanglePolygons.Clear();

            int scaler = 7;
            int nextFloorYStep = 3 * scaler;

            // { y ; x }
            Point logoAnchor = new Point(0, 0);
            List<Point> upFacePoints = new()
            {
                new Point(2, 0),
                new Point(0, 2),
                new Point(5, 2),
                new Point(7, 0),
            };

            // Calibrage & mise à l'échelle
            upFacePoints = upFacePoints.Select<Point, Point>(p => 
            {
                p.X += logoAnchor.X;
                p.Y += logoAnchor.Y;
                p.X *= scaler;
                p.Y *= scaler;
                return p;
            }
            ).ToList();

            // Dessiner la face du devant - contient le chiffre
            List<Point> frontFacePoints = new()
            {
                upFacePoints[1],
                new Point(upFacePoints[1].X, upFacePoints[1].Y + nextFloorYStep),
                new Point(upFacePoints[2].X, upFacePoints[2].Y + nextFloorYStep),
                upFacePoints[2],
            };
            // Dessiner la face de côté
            List<Point> sideFacePoints = new()
            {
                upFacePoints[2],
                new Point(upFacePoints[2].X, upFacePoints[2].Y + nextFloorYStep),
                new Point(upFacePoints[3].X, upFacePoints[3].Y + nextFloorYStep),
                upFacePoints[3],
            };

            // Dessiner la première face : face du dessus
            Polygon upFace = new Polygon()
            {
                Stroke = Brushes.Black,
                Fill = (Brush)Utils.GetFaceColor(floorsNb - 1, family => family.LightColor),
                StrokeThickness = 1,
                Points = new PointCollection(upFacePoints),
            };
            this.LogoPlace.Children.Add(upFace);

            // BOUCLE
            for (int i = floorsNb - 1; i >= 0; i--)
            {
                Polygon frontFace = new Polygon()
                {
                    Stroke = Brushes.Black,
                    Fill = (Brush)Utils.GetFaceColor(i, family => family.NormalColor),
                    StrokeThickness = 1,
                    Points = new PointCollection(frontFacePoints)
                };
                Polygon sideFace = new Polygon()
                {
                    Stroke = Brushes.Black,
                    Fill = (Brush)Utils.GetFaceColor(i, family => family.DarkColor),
                    StrokeThickness = 1,
                    Points = new PointCollection(sideFacePoints),
                };
                
                frontFacePoints = frontFacePoints.Select<Point, Point>(p =>
                {
                    p.Y += nextFloorYStep;
                    return p;
                }).ToList();
                sideFacePoints = sideFacePoints.Select<Point, Point>(p =>
                {
                    p.Y += nextFloorYStep;
                    return p;
                }).ToList();

                this.LogoPlace.Children.Add(frontFace);
                this.LogoPlace.Children.Add(sideFace);
                this._allRectanglePolygons.Add(frontFace);
            }
            PrintNumbersOnFloorsLogo();
        }
        public void PrintNumbersOnFloorsLogo()
        {
            int floorNb = this._allRectanglePolygons.Count;
            StackPanel textBlocksStackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            this.LogoPlace.Children.Add(textBlocksStackPanel);

            foreach (Polygon frontFace in this._allRectanglePolygons)
            {
                if (floorNb == this._allRectanglePolygons.Count)
                {
                    textBlocksStackPanel.Margin = new Thickness(0, frontFace.Points[0].Y, 0, 0);
                }
                floorNb--;
                // Ajouter le chiffre
                TextBlock tb = new TextBlock();
                tb.Margin = new Thickness(0);
                tb.Padding = new Thickness(0);
                tb.Width = frontFace.Points[3].X - frontFace.Points[0].X;
                tb.Height = frontFace.Points[1].Y - frontFace.Points[0].Y;
                tb.Text = floorNb.ToString();
                tb.TextAlignment = TextAlignment.Center;
                tb.Foreground = Brushes.White;
                tb.FontSize = 15;
                textBlocksStackPanel.Children.Add(tb);
                Grid.SetZIndex(tb, 10);
            }
        }
    }
}
