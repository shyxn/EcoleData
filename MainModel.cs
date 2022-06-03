using System;
using System.IO;
using System.Reflection;

namespace EcoleData
{
    public class MainModel
    {
        private MainController _controller;

        private const string USER_SETTINGS_FILENAME = "settings.xml";
        private string _defaultSettingsPath;

        /// <summary>
        /// Date maximum possible déterminée dans la création de l'arborescence.
        /// </summary>
        public DateTime MaxDate { get; set; }
        /// <summary>
        /// Date minimum possible déterminée dans la création de l'arborescence.
        /// </summary>
        public DateTime MinDate { get; set; }

        public Settings Settings { get; private set; }

        public MainModel(MainController mainController)
        {
            this._defaultSettingsPath = this.GetAssemblyDirectory() + "\\settings\\" + USER_SETTINGS_FILENAME;
            this._controller = mainController;
            this.Settings = new Settings()
            {
                FolderPath = ""
            };
            
            GetUserSettings();
        }
        // https://stackoverflow.com/a/283917
        public string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
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

        public void SetNewFolderPath(string folderPath)
        {
            this.Settings.FolderPath = folderPath;
            this.SetUserSettings();
        }

        public void GetUserSettings()
        {
            CreateSettingsPath();
            if (File.Exists(this._defaultSettingsPath) && File.ReadAllText(this._defaultSettingsPath).Length > 0)
                this.Settings = Settings.Read(_defaultSettingsPath);
        }
        public void SetUserSettings()
        {
            this.Settings.Save(this._defaultSettingsPath);
        }
    }
}
