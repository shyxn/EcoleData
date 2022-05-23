using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoleData.Tree;
using System.Linq;

namespace EcoleData
{
    public class MainController
    {
        private MainWindow _mainWindow;
        private MainModel _mainModel;

        private string _softwareTitle;
        public string SoftwareTitle { get; set; }
        public MainController(MainWindow vue)
        {
            this._mainWindow = vue;
            this._mainModel = new MainModel(this);
            CheckFolderValidity();
        }

        public void SetNewFolderPath(string folderPath) => this._mainModel.SetNewFolderPath(folderPath);

        /// <summary>
        /// Permet de vérifier si le dossier existe toujours, si oui, permet de créer l'arboresence de données.
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
            Debug.WriteLine("Le programme va charger les données...");
            string foldersPath = this._mainModel.Settings.FolderPath;

            string[] schoolNames = Utils.GetFoldersNames(foldersPath);

            // Création du niveau 0 - Regroupement de toutes les écoles dans un dictionnaire
            DataSchool dataSchool = new DataSchool()
            {
                // Dictionnaire avec les noms des écoles en Keys, et des objets School en Values.
                Schools = schoolNames.ToDictionary(
                    name => name, 
                    name =>
                    {
                        School school = new School();
                        // Pour chaque étage, on cherche ensuite, à un niveau plus bas, à configurer le dictionnaire Locations...
                        school.SetFloors(foldersPath + "\\" + name);
                        return school;
                    })
            };
        }
            
        
        
        public void GetFloors()
        {

        }
        public void SetFloors()
        {

        }
        public void GetLocations()
        {

        }
        public void SetLocations()
        {
            
        }
    }
}
