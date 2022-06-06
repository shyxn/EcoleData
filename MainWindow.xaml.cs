/* 
 * ETML
 * Autrice : Morgane Lebre
 * Date : du 13 mai au 8 juin 2022
 */

using Microsoft.WindowsAPICodePack.Dialogs;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EcoleData
{
    /// <summary>
    /// Contient les opérations logiques de MainWindows.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Contrôleur du schéma MVC.
        /// </summary>
        private MainController _controller;
        /// <summary>
        /// Liste d'éléments UI dont la visibilité est à gérer en fonction de la validité du dossier choisi
        /// </summary>
        private List<UIElement> _elementsToHideIfNoFolder;
        /// <summary>
        /// Liste de tous les objets Polygon servant de base à chaque étage dans le logo. (faces antérieures de la tour)
        /// </summary>
        private List<Polygon> _allRectanglePolygons { get; set; }
        /// <summary>
        /// Liste de tous les TextBlock de messages d'erreurs pour l'utilisateur dans les filtres.
        /// </summary>
        public List<TextBlock> AllFiltersErrorMessages { get; set; }
        /// <summary>
        /// Composant visuel des graphiques (Oxyplot)
        /// </summary>
        public PlotView PlotView { get; set; }
        /// <summary>
        /// (ctor) Initialise les propriétés par défaut.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.PlotView = new PlotView() { Padding = new Thickness(0, 0, 200, 0)};
            this.PlotView.IsVisibleChanged += PlotView_IsVisibleChanged;
            this.graphPlace.Children.Add(this.PlotView);
            this._controller = new MainController(this);
            this.loadMessageTB.LayoutUpdated += LoadMessageTextBox_LayoutUpdated;
            this._allRectanglePolygons = new();

            this._elementsToHideIfNoFolder = new()
            {
                this.schoolTitle,
                this.schoolCaptorsNb,
                this.resetFiltersBtn,
                this.PlotView
            };
            this.AllFiltersErrorMessages = new()
            {
                this.datesFilterErrorMsg,
                this.floorsFiltersErrorMsg,
                this.locationsFilterErrorMsg,
                this.valuesFiltersErrorMsg
            };
            HideUIElements();
        }

        /// <summary>
        /// Permet de déclencher le chargement des données cheu le contrôleur. Le message doit être visible à l'utilisateur.
        /// </summary>
        /// <param name="sender">Objet concerné par l'événement (ici le TextBlock du message)</param>
        /// <param name="e">Informations sur l'événement déclenché</param>
        private void LoadMessageTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.loadMessageTB.Visibility == Visibility.Visible)
            {
                // Commencer ici le chargement des données
                this._controller.CheckFolderValidity();
                this.loadMessageTB.Visibility = Visibility.Hidden;
                Debug.WriteLine("Fin de la recherche des données.");
            }
        }

        /// <summary>
        /// Événement de sélection du dossier.
        /// </summary>
        /// <param name="sender">Button de sélection du dossier</param>
        /// <param name="e">Informations sur l'événement déclenché</param>
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
                // Afficher le chemin du dossier - ça déclenchera la suite grâce à LoadMessageTextBox_LayoutUpdated
                this._controller.SetNewFolderPath(dlg.FileName);
                ClearAllUIElements();
            }
        }
        /// <summary>
        /// Au commencement du programme - lorsque la fenêtre a fini de charger
        /// </summary>
        /// <param name="sender">L'élément Window en question.</param>
        /// <param name="e">Informations sur l'événement déclenché</param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Debug.WriteLine("La fenêtre a fini de charger. Recherche des données...");
            this.loadMessageTB.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Lorsqu'un élément est choisi dans la liste déroulante des écoles. 
        /// Du côté de la vue, des informations doivent être affichées et du côté du contrôleur, les filtres doivent être actualisés.
        /// </summary>
        /// <param name="sender">L'élément de liste déroulante (ComboBox) sur lequel l'événement s'est déclenché</param>
        /// <param name="e">Informations sur l'événement déclenché</param>
        private void SchoolsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Items.Count > 0)
            {
                ShowUIElements();
                string selectedSchoolName = ((ComboBox)sender).SelectedItem.ToString();
                this.schoolTitle.Text = "École de " + selectedSchoolName;

                this._controller.UpdateFilters(selectedSchoolName);
            }
        }
        /// <summary>
        /// Cacher les éléments à faire disparaître (lorsqu'aucun dossier n'est sélectionné). Les éléments ne sont pas Collapsed.
        /// </summary>
        public void HideUIElements() => this._elementsToHideIfNoFolder.ForEach(element => element.Visibility = Visibility.Hidden);
        /// <summary>
        /// Afficher les éléments lorsqu'un dossier est sélectionné.
        /// </summary>
        public void ShowUIElements() => this._elementsToHideIfNoFolder.ForEach(element => element.Visibility = Visibility.Visible);
        /// <summary>
        /// Collapse tous les messages d'erreurs des filtres.
        /// </summary>
        public void HideAllFiltersMessages() => this.AllFiltersErrorMessages.ForEach(message => message.Visibility = Visibility.Collapsed);

        /// <summary>
        /// Pour quand la sélection de dossiers change.
        /// Commence à tout charger et finit par afficher le message de chargement des données. (c'est son apparition qui déclenchera la suite)
        /// </summary>
        public void ClearAllUIElements()
        {
            // Supprimer tous les polygones
            this.logoPlace.Children.Clear();

            // Supprimer tous les checkboxes étage
            this.floorsGrid.Children.Clear();

            // Décocher toutes les checkboxes
            this.salleSensorCB.IsChecked = false;
            this.couloirSensorCB.IsChecked = false;
            this.temperatureCB.IsChecked = false;
            this.dewPointCB.IsChecked = false;
            this.humidityCB.IsChecked = false;
            this.schoolTitle.Text = "";
            this.schoolCaptorsNb.Text = "";

            // Désafficher le graphique. 
            this.graphPlace.Visibility = Visibility.Hidden;

            // Vider le combobox
            this.schoolsComboBox.Items.Clear();

            // Afficher le chargement des données
            this.loadMessageTB.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Affiche le message indiquant qu'il n'y a aucune donnée si le PlotView venait à être invisible.
        /// </summary>
        /// <param name="sender">Le PlotView en question</param>
        /// <param name="e">Informations sur l'événement déclenché.</param>
        private void PlotView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) =>
            this.noDataMsg.Visibility = this.PlotView.Visibility != Visibility.Visible ? Visibility.Visible : Visibility.Hidden;

        /// <summary>
        /// Se déclenche lorsque l'utilisateur veut valider ses filtres. Délègue cette tâche au contrôleur.
        /// </summary>
        /// <param name="sender">Button applyFiltersBtn</param>
        /// <param name="e">Informations sur l'événement déclenché</param>
        private void ApplyFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            this._controller.CheckAndApplyFilters();
        }

        /// <summary>
        /// Se déclenche lorsque l'utilisateur veut réinitialiser les filtres pendant la sélection. 
        /// Les filtres sont donc réinitialisés par défaut par le contrôleur.
        /// </summary>
        /// <param name="sender">Button resetFiltersBtn</param>
        /// <param name="e">Informations sur l'événement déclenché</param>
        private void ResetFiltersBtn_Click(object sender, RoutedEventArgs e)
        {
            this._controller.SetDefaultFilters();
        }

        /// <summary>
        /// Affichage du logo dynamique des étages dans logoPlace.
        /// </summary>
        /// <param name="floorsNb">Nombre d'étages à afficher</param>
        public void PrintFloorLogo(int floorsNb)
        {
            this.logoPlace.Children.Clear();
            this._allRectanglePolygons.Clear();

            int scaler = 7;
            int nextFloorYStep = 3 * scaler;

            // Coordonnées { y ; x }
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

            // Dessiner la face du devant (contient le chiffre)
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
            this.logoPlace.Children.Add(upFace);

            // Itération pour chaque étage
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
                // Décalage des points pour le prochain étage
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

                this.logoPlace.Children.Add(frontFace);
                this.logoPlace.Children.Add(sideFace);
                this._allRectanglePolygons.Add(frontFace);
            }
            PrintNumbersOnFloorsLogo();
        }
        /// <summary>
        /// Afficher le chiffre de l'étage pour chaque Polygon de face antérieure généré et stocké dans _allRectanglePolygons.
        /// </summary>
        public void PrintNumbersOnFloorsLogo()
        {
            int floorNb = this._allRectanglePolygons.Count;

            // Création du contenant (StackPanel) de tous les futurs TextBlocks.
            StackPanel textBlocksStackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            this.logoPlace.Children.Add(textBlocksStackPanel);

            // Sur la base de chaque face antérieure 
            foreach (Polygon frontFace in this._allRectanglePolygons)
            { 
                if (floorNb == this._allRectanglePolygons.Count)
                {
                    textBlocksStackPanel.Margin = new Thickness(0, frontFace.Points[0].Y, 0, 0);
                }
                floorNb--;

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

                // Être sûr qu'il soit bien visible par dessus les polygones
                Grid.SetZIndex(tb, 10);
            }
        }
    }
}
