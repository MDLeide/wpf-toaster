using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cashew.Toasty.Settings;

namespace Cashew.Toasty.Sample.Settings
{
    /// <summary>
    /// Interaction logic for SystemConfiguration.xaml
    /// </summary>
    public partial class ToasterSettingsView : UserControl
    {
        SavedSettings _savedSettings;

        public ToasterSettingsView()
            : this(Defaults.DefaultSettings.DefaultToasterSettings, null)
        {
        }

        public ToasterSettingsView(ToasterSettings settings, SavedSettings savedSettings)
        {
            InitializeComponent();

            _savedSettings = savedSettings;

            if (_savedSettings != null)
                foreach (var setting in _savedSettings.ToasterSettings)
                    ToasterSettings.Items.Add(setting);

            foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>())
            {
                EnterFromDirection.Items.Add(dir);
                LeaveDirection.Items.Add(dir);
                MoveDirection.Items.Add(dir);
            }

            foreach (var style in Enum.GetValues(typeof(MoveStyle)).Cast<MoveStyle>())
                MoveStyle.Items.Add(style);

            foreach (var style in Enum.GetValues(typeof(EnterStyle)).Cast<EnterStyle>())
                EnterStyle.Items.Add(style);

            foreach (var loc in Enum.GetValues(typeof(Location)).Cast<Location>())
                ToastLocation.Items.Add(loc);

            foreach (var style in Enum.GetValues(typeof(LeaveStyle)).Cast<LeaveStyle>())
                LeaveStyle.Items.Add(style);

            Settings = settings;
            if (Settings != null)
                InitValues();
        }

        public event EventHandler SettingsUpdated;

        public void Initialize(ToasterSettings settings, SavedSettings savedSettings)
        {
            Settings = settings;
            _savedSettings = savedSettings;

            foreach (var setting in savedSettings.ToasterSettings)
                ToasterSettings.Items.Add(setting);
            ToasterSettings.SelectedItem = Settings;

            InitValues();
        }

        public ToasterSettings Settings { get; private set; }

        void InitValues()
        {
            SettingsName.Text = Settings.Name;

            //system
            QueueToasts.IsChecked = Settings.QueueToasts;

            // positioning
            VerticalPadding.Text = Settings.VerticalPadding.ToString();
            VerticalAdjustment.Text = Settings.VerticalAdjustment.ToString();
            HorizontalPadding.Text = Settings.HorizontalPadding.ToString();
            HorizontalAdjustment.Text = Settings.HorizontalAdjustment.ToString();

            //movement
            MoveDuration.Text = Settings.MoveDuration.ToString();
            MoveStyle.SelectedItem = Settings.MoveStyle;
            MoveDirection.SelectedItem = Settings.MoveDirection;

            //enter
            EnterFromDirection.SelectedItem = Settings.EnterFromDirection;
            ToastLocation.SelectedItem = Settings.EnterLocation;
            EnterStyle.SelectedItem = Settings.EnterStyle;

            //leave
            LeaveStyle.SelectedItem = Settings.LeaveStyle;
            LeaveDirection.SelectedItem = Settings.LeaveDirection;
        }

        void DoSave()
        {
            //system
            Settings.QueueToasts = QueueToasts.IsChecked ?? false;

            //positioning
            Settings.VerticalPadding = double.Parse(VerticalPadding.Text);
            Settings.VerticalAdjustment = double.Parse(VerticalAdjustment.Text);
            Settings.HorizontalPadding = double.Parse(HorizontalPadding.Text);
            Settings.HorizontalAdjustment = double.Parse(HorizontalAdjustment.Text);

            //movement
            Settings.MoveDuration = int.Parse(MoveDuration.Text);
            Settings.MoveStyle = (MoveStyle) MoveStyle.SelectedItem;
            Settings.MoveDirection = (Direction) MoveDirection.SelectedItem;

            //enter
            Settings.EnterFromDirection = (Direction) EnterFromDirection.SelectedItem;
            Settings.EnterLocation = (Location) ToastLocation.SelectedItem;
            Settings.EnterStyle = (EnterStyle) EnterStyle.SelectedItem;

            //leave
            Settings.LeaveStyle = (LeaveStyle) LeaveStyle.SelectedItem;
            Settings.LeaveDirection = (Direction) LeaveDirection.SelectedItem;

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
            ToasterSettings.Items.Add(Settings);
            OnSettingChanged();
        }

        void ToasterSettings_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(ToasterSettings.SelectedItem is ToasterSettings setting))
                return;
            Settings = setting;
            OnSettingChanged();
        }

        void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings.Name == "default")
                return;

            _savedSettings.RemoveSettings(Settings);
            ToasterSettings.Items.Remove(Settings);
            Settings = _savedSettings.ToasterSettings.FirstOrDefault();
            ToasterSettings.SelectedItem = Settings;
            OnSettingChanged();
        }

        void OnSettingChanged()
        {
            Delete.IsEnabled = Settings != null && Settings.Name != "default";

            InitValues();
            SettingsUpdated?.Invoke(this, new EventArgs());
            Properties.Settings.Default.toasterSettingsName = Settings.Name;
            Properties.Settings.Default.Save();
            ToasterSettings.SelectedItem = Settings;
        }
    }
}
