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

            // LIRE TOUS LES CSV( ~ 700k objets instanciés)
            //DataSchool.Schools.ToList()
            //    .ForEach(x => x.Value.Floors.ToList()
            //        .ForEach(x => x.Value.Locations.ToList().ForEach(x => x.Value.ReadCSV())));
            try
            {
                DataSchool = new DataSchool(this._mainModel.Settings.FolderPath);
                DataSchool.Schools.Values.First().Floors.Values.First().Locations.Values.First().ReadCSV();
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
            }
            Debug.WriteLine("[LoadTree()] Arborescence terminée.");
        }

        private bool CheckTreeValidity()
        {
            return DataSchool.Schools.Count != 0
                && DataSchool.Schools.Values.ToList()
                .TrueForAll(school => school.Floors.Count != 0 && school.Floors.Values.ToList()
                    .TrueForAll(floor => floor.Locations.Count != 0));
        }
    }
}
