using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EcoleData
{
    public class MainModel
    {
        private MainController _controller;

        private const string USER_SETTINGS_FILENAME = "settings.xml";
        private string _defaultSettingsPath = Directory.GetCurrentDirectory() + "\\Settings\\" + USER_SETTINGS_FILENAME;
        
        public Settings Settings { get; private set; }

        public MainModel(MainController mainController)
        {
            this._controller = mainController;
            this.Settings = new Settings()
            {
                FolderPath = ""
            };
            
            GetUserSettings();
        }
        public void CreateSettingsPath()
        {
            // Si le dossier Settings\UserSettings n'existe pas, alors en créer un
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Settings\\");

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
