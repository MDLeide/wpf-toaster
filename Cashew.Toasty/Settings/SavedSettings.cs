using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Cashew.Toasty.Settings
{
    public class SavedSettings
    {
        List<ToastAdornerSettings> _toastSettingsCollection = new List<ToastAdornerSettings>();
        List<ToasterSettings> _toasterSettingsCollection = new List<ToasterSettings>();

        public SavedSettings(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Must supply a valid file path.");
            
            FilePath = filePath;
            ToastSettings = new ReadOnlyCollection<ToastAdornerSettings>(_toastSettingsCollection);
            ToasterSettings = new ReadOnlyCollection<ToasterSettings>(_toasterSettingsCollection);
        }

        public string FilePath { get; }
        public bool AutoSaveOnAddOrRemove { get; set; }
        public ReadOnlyCollection<ToastAdornerSettings> ToastSettings { get; private set; }
        public ReadOnlyCollection<ToasterSettings> ToasterSettings { get; private set; }


        public ToastAdornerSettings GetToastSettings(string name)
        {
            return _toastSettingsCollection.FirstOrDefault(p => p.Name == name);
        }

        public ToasterSettings GetToasterSettings(string name)
        {
            return _toasterSettingsCollection.FirstOrDefault(p => p.Name == name);
        }

        public void AddSettings(ToastAdornerSettings toastAdornerSettings)
        {
            if (string.IsNullOrWhiteSpace(toastAdornerSettings.Name))
                throw new ArgumentException("The settings name cannot be empty or null.");

            if (ToastSettings.Any(p => p.Name == toastAdornerSettings.Name))
                throw new ArgumentException("A toast settings object already exists with that name.");

            _toastSettingsCollection.Add(toastAdornerSettings);

            if (AutoSaveOnAddOrRemove)
                Save();
        }

        public void AddSettings(ToasterSettings toasterSettings)
        {
            if (string.IsNullOrWhiteSpace(toasterSettings.Name))
                throw new ArgumentException("The settings name cannot be empty or null.");

            if (ToastSettings.Any(p => p.Name == toasterSettings.Name))
                throw new ArgumentException("A toast settings object already exists with that name.");

            _toasterSettingsCollection.Add(toasterSettings);

            if (AutoSaveOnAddOrRemove)
                Save();
        }

        public void RemoveToastSettings(string name)
        {
            var settings = _toastSettingsCollection.FirstOrDefault(p => p.Name == name);
            if (settings == null)
                throw new ArgumentException("No settings found with that name.");
            RemoveSettings(settings);
        }

        public void RemoveToasterSettings(string name)
        {
            var settings = _toasterSettingsCollection.FirstOrDefault(p => p.Name == name);
            if (settings == null)
                throw new ArgumentException("No settings found with that name.");
            RemoveSettings(settings);
        }

        public void RemoveSettings(ToastAdornerSettings toastAdornerSettings)
        {
            if (_toastSettingsCollection.Remove(toastAdornerSettings) && AutoSaveOnAddOrRemove)
                Save();
        }

        public void RemoveSettings(ToasterSettings toasterSettings)
        {
            if (_toasterSettingsCollection.Remove(toasterSettings) && AutoSaveOnAddOrRemove)
                Save();
        }

        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                ToastSettings = new ReadOnlyCollection<ToastAdornerSettings>(_toastSettingsCollection);
                ToasterSettings = new ReadOnlyCollection<ToasterSettings>(_toasterSettingsCollection);
                return;
            }

            var reader = new XmlSerializer(typeof(Settings));
            Settings settings;
            using (var file = new StreamReader(FilePath))
            {
                settings = (Settings) reader.Deserialize(file);
            }

            _toasterSettingsCollection = settings.ToasterSettings.ToList();
            _toastSettingsCollection = settings.ToastSettings.ToList();

            ToastSettings = new ReadOnlyCollection<ToastAdornerSettings>(_toastSettingsCollection);
            ToasterSettings = new ReadOnlyCollection<ToasterSettings>(_toasterSettingsCollection);
        }

        public void Save()
        {
            var settings = new Settings();
            settings.ToastSettings = _toastSettingsCollection.ToList();
            settings.ToasterSettings = _toasterSettingsCollection.ToList();

            var writer = new XmlSerializer(typeof(Settings));
            using (var file = File.Create(FilePath))
            {
                writer.Serialize(file, settings);
            }
        }
    }

    [Serializable]
    public class Settings
    {
        public List<ToastAdornerSettings> ToastSettings { get; set; } = new List<ToastAdornerSettings>();
        public List<ToasterSettings> ToasterSettings { get; set; } = new List<ToasterSettings>();
    }
}
