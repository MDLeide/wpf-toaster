using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cashew.Toasty.Settings;

namespace Cashew.Toasty.Sample.Settings
{
    /// <summary>
    /// Interaction logic for ToastConfigView.xaml
    /// </summary>
    public partial class ToastSettingsView : UserControl
    {
        SavedSettings _savedSettings;

        public ToastSettingsView()
            : this(Defaults.DefaultSettings.DefaultToastSettings, null) { }

        public ToastSettingsView(ToastAdornerSettings settings, SavedSettings savedSettings)
        {
            InitializeComponent();

            if (_savedSettings != null)
                foreach (var setting in _savedSettings.ToastSettings)
                    ToastSettings.Items.Add(setting);

            _savedSettings = savedSettings;
            Settings = settings;
            InitValues();
        }


        public event EventHandler SettingsUpdated;
        
        public ToastAdornerSettings Settings { get; private set; }


        public void Initialize(ToastAdornerSettings settings, SavedSettings savedSettings)
        {
            Settings = settings;
            _savedSettings = savedSettings;
            foreach (var setting in _savedSettings.ToastSettings)
                ToastSettings.Items.Add(setting);
            ToastSettings.SelectedItem = Settings;

            InitValues();
        }

        void InitValues()
        {
            if (Settings == null)
                return;

            SettingsName.Text = Settings.Name;

            CanUserClose.IsChecked = Settings.CanUserClose;
            CloseOnRightClick.IsChecked = Settings.CloseOnRightClick;
            CloseAfterClickAction.IsChecked = Settings.CloseAfterClickAction;

            Lifetime.Text = Settings.Lifetime.ToString();
            RefreshLifetimeOnMouseOver.IsChecked = Settings.RefreshLifetimeOnMouseOver;
            DynamicLifetime.IsChecked = Settings.DynamicLifetime;
            DynamicLifetimeBase.Text = Settings.DynamicLifetimeBase.ToString();
            DynamicLifetimeMillisecondsPerCharacter.Text = Settings.DynamicLifetimeMillisecondsPerCharacter.ToString();
            DynamicLifetimeMinimum.Text = Settings.DynamicLifetimeMinimum.ToString();
            DynamicLifetimeMaximum.Text = Settings.DynamicLifetimeMaximum.ToString();
            FadeTime.Text = Settings.LeaveTime.ToString();
        }

        void DoSave()
        {
            Settings.CanUserClose = CanUserClose.IsChecked ?? false;
            Settings.CloseOnRightClick = CloseOnRightClick.IsChecked ?? false;
            Settings.CloseAfterClickAction = CloseAfterClickAction.IsChecked ?? false;

            Settings.Lifetime = int.Parse(Lifetime.Text);
            Settings.RefreshLifetimeOnMouseOver = RefreshLifetimeOnMouseOver.IsChecked ?? false;

            Settings.DynamicLifetime = DynamicLifetime.IsChecked ?? false;
            Settings.DynamicLifetimeBase = int.Parse(DynamicLifetimeBase.Text);
            Settings.DynamicLifetimeMillisecondsPerCharacter = int.Parse(DynamicLifetimeMillisecondsPerCharacter.Text);
            Settings.DynamicLifetimeMinimum = int.Parse(DynamicLifetimeMinimum.Text);
            Settings.DynamicLifetimeMaximum = int.Parse(DynamicLifetimeMaximum.Text);

            Settings.LeaveTime = int.Parse(FadeTime.Text);

            if (Settings.Name.ToLower() != "default")
                _savedSettings.Save();

            SettingsUpdated?.Invoke(this, new EventArgs());
        }

        void Save_OnClick(object sender, RoutedEventArgs e)
        {
            DoSave();
        }
        
        void SaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            var getName = new GetNameView();
            getName.ShowDialog();
            if (string.IsNullOrWhiteSpace(getName.SelectedName))
                return;

            if (_savedSettings.ToastSettings.Any(p => p.Name == getName.SelectedName))
            {
                MessageBox.Show("A settings file with that name already exists.");
                return;
            }

            Settings = Settings.Clone(getName.SelectedName);
            SettingsName.Text = getName.SelectedName;
            DoSave();
            _savedSettings.AddSettings(Settings);
            ToastSettings.Items.Add(Settings);
            OnSettingChanged();
        }

        void ToastSettings_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(ToastSettings.SelectedItem is ToastAdornerSettings settings))
                return;
            Settings = settings;
            OnSettingChanged();
        }

        void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings.Name == "default")
                return;

            _savedSettings.RemoveSettings(Settings);
            ToastSettings.Items.Remove(Settings); 
            Settings = _savedSettings.ToastSettings.FirstOrDefault();
            ToastSettings.SelectedItem = Settings;
            OnSettingChanged();
        }

        void OnSettingChanged()
        {
            Delete.IsEnabled = Settings != null && Settings.Name != "default";

            InitValues();
            SettingsUpdated?.Invoke(this, new EventArgs());
            Properties.Settings.Default.toastSettingsName = Settings.Name;
            Properties.Settings.Default.Save();
            ToastSettings.SelectedItem = Settings;
        }
    }
}
