using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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

namespace EcoleData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainController _controller;
        public MainWindow()
        {
            InitializeComponent();
            this._controller = new MainController(this);
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
    }
}
