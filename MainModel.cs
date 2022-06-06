/* 
 * ETML
 * Autrice : Morgane Lebre
 * Date : du 13 mai au 8 juin 2022
 */

using System;
using System.IO;
using System.Reflection;

namespace EcoleData
{
    /// <summary>
    /// Modèle principale du schéma MVC. 
    /// Cette classe s’occupe de la gestion du fichier settings.xml. 
    /// Elle stocke également des informations pour le contrôleur, comme par exemple les dates limites calculées par le contrôleur lors de sa recherche dans les enregistrements.
    /// </summary>
    public class MainModel
    {
        private MainController _controller;

        private const string USER_SETTINGS_FILENAME = "settings.xml";
        private string _defaultSettingsPath;

        /// <summary>
        /// Date maximum possible déterminée pendant la création de l'arborescence.
        /// </summary>
        public DateTime MaxDate { get; set; }
        /// <summary>
        /// Date minimum possible déterminée pendant la création de l'arborescence.
        /// </summary>
        public DateTime MinDate { get; set; }

        /// <summary>
        /// Tous les Settings à sauvegarder.
        /// </summary>
        public Settings Settings { get; private set; }
        /// <summary>
        /// (ctor) Initialisation des propriétés et recherche des settings par défaut.
        /// </summary>
        /// <param name="mainController">Contrôleur fourni à relier pour le schéma MVC.</param>
        public MainModel(MainController mainController)
        {
            this._defaultSettingsPath = this.GetAssemblyDirectory() + "\\settings\\" + USER_SETTINGS_FILENAME;
            this._controller = mainController;
            this.Settings = new Settings(){ FolderPath = "" };
            
            GetUserSettings();
        }

        // https://stackoverflow.com/a/283917
        /// <summary>
        /// Obtient le dossier courant dans lequel s'exécute le programme.
        /// </summary>
        /// <returns>Chemin du dossier souhaité.</returns>
        public string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
        /// <summary>
        /// 
        /// </summary>
        public void CreateSettingsPath()
        {
            // Si le dossier Settings n'existe pas, alors en créer un
            string settingsPath = GetAssemblyDirectory() + "\\settings\\";
            Directory.CreateDirectory(settingsPath);

            // Si le fichier Settings\settings.xml n'existe pas, alors en créer un
            if (!File.Exists(_defaultSettingsPath))
            {
                File.Create(_defaultSettingsPath);
                this.Settings.Save(_defaultSettingsPath);
            }
        }
        /// <summary>
        /// Obtient le chemin du dossier des données et le sauvegarde.
        /// </summary>
        /// <param name="folderPath">Le chemin en question.</param>
        public void SetNewFolderPath(string folderPath)
        {
            this.Settings.FolderPath = folderPath;
            this.SetUserSettings();
        }

        /// <summary>
        /// Obtient les paramètres sauvées dans le fichier settings.xml
        /// </summary>
        public void GetUserSettings()
        {
            CreateSettingsPath();
            if (File.Exists(this._defaultSettingsPath) && File.ReadAllText(this._defaultSettingsPath).Length > 0)
                this.Settings = Settings.Read(_defaultSettingsPath);
        }
        /// <summary>
        /// Appelle la fonction de la classe Settings pour sauvegarder les paramètres.
        /// </summary>
        public void SetUserSettings()
        {
            this.Settings.Save(this._defaultSettingsPath);
        }
    }
}
