using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
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

            Types.Items.Add("Custom");
            Types.Items.Add("Info");
            Types.Items.Add("Success");
            Types.Items.Add("Warning");
            Types.Items.Add("Error");

            Types.SelectionChanged += (s, e) =>
            {
                Properties.Settings.Default.type = Types.SelectedItem as string;
                Properties.Settings.Default.Save();

                var type = (string)Types.SelectedItem;
                if (type == "Custom")
                {
                    ToastSettings.IsEnabled = true;
                    ToasterSettings.IsEnabled = true;
                }
                else
                {
                    ToastSettings.IsEnabled = false;
                    ToasterSettings.IsEnabled = false;
                }
            };

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.type))
                Types.SelectedIndex = 0;
            else
            {
                var index = Types.Items.IndexOf(Properties.Settings.Default.type);
                if (index < 0)
                    index = 0;
                Types.SelectedIndex = index;
            }
            
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
            Show((string)Types.SelectedItem, Message.Message, Message.Title);
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

        void ShowToastThreaded_OnClick(object sender, RoutedEventArgs e)
        {
            var type = (string) Types.SelectedItem;
            var message = Message.Message;
            var title = Message.Title;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Show(type, message, title);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                    throw;
                }
            });
        }

        void Show(string selectedType, string message, string title)
        {
            switch (selectedType)
            {
                case "Custom":
                    var adorner = _toaster.Show(message, title);
                    if (adorner.ToastView is ToastView tv)
                        tv.CloseButtonClicked += (s, e) => adorner.RequestClose();
                    break;
                case "Info":
                    Toast.Info(this, message, title);
                    break;
                case "Success":
                    Toast.Success(this, message, title);
                    break;
                case "Warning":
                    Toast.Warning(this, message, title);
                    break;
                case "Error":
                    Toast.Error(this, message, title);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
