using System;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using Cashew.Toasty.Settings;

namespace Cashew.Toasty.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Toaster _toaster;
        readonly SavedSettings _savedSettings;
        public MainWindow()
        {
            InitializeComponent();
            _savedSettings = new SavedSettings(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "toast.settings"));
            _savedSettings.AutoSaveOnAddOrRemove = true;
            _savedSettings.Load();

            ToastSettings.SettingsUpdated += ToastSettingsOnSettingsUpdated;
            ToasterSettings.SettingsUpdated += ToastSettingsOnSettingsUpdated;

            Loaded += (s, e) =>
            {
                var toastSettings = GetToastSettings();
                var toasterSettings = GetToasterSettings();

                ToastSettings.Initialize(toastSettings, _savedSettings);
                ToasterSettings.Initialize(toasterSettings, _savedSettings);

                _toaster = new Toaster(this, ToasterSettings.Settings, null, ToastSettings.Settings);
            };
        }

        void ToastSettingsOnSettingsUpdated(object sender, EventArgs e)
        {
            _toaster = new Toaster(this, ToasterSettings.Settings, null, ToastSettings.Settings);
        }

        void Show_OnClick(object sender, RoutedEventArgs e)
        {
            switch (Message.SelectedType)
            {
                case "Custom":
                    _toaster.Show(Message.Title, Message.Message);
                    break;
                case "Info":
                    Toast.Info(this, Message.Message, Message.Title);
                    break;
                case "Success":
                    Toast.Success(this, Message.Message, Message.Title);
                    break;
                case "Warning":
                    Toast.Warning(this, Message.Message, Message.Title);
                    break;
                case "Error":
                    Toast.Error(this, Message.Message, Message.Title);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        ToastAdornerSettings GetToastSettings()
        {
            ToastAdornerSettings toastSettings = null;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.toastSettingsName))
                toastSettings = _savedSettings.GetToastSettings(Properties.Settings.Default.toastSettingsName);
            return toastSettings ?? Defaults.DefaultSettings.DefaultToastSettings;
        }

        ToasterSettings GetToasterSettings()
        {
            ToasterSettings toasterSettings = null;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.toasterSettingsName))
                toasterSettings = _savedSettings.GetToasterSettings(Properties.Settings.Default.toasterSettingsName);
            return toasterSettings ?? Defaults.DefaultSettings.DefaultToasterSettings;
        }
    }
}
