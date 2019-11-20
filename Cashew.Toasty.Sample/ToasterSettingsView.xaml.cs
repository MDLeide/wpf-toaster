using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cashew.Toasty.Config;

namespace Cashew.Toasty.Sample
{
    /// <summary>
    /// Interaction logic for SystemConfiguration.xaml
    /// </summary>
    public partial class ToasterSettingsView : UserControl
    {
        public ToasterSettingsView()
            : this(Defaults.DefaultSettings.DefaultToasterSettings) { }

        public ToasterSettingsView(ToasterSettings settings)
        {
            InitializeComponent();

            foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>())
            {
                EnterFromDirection.Items.Add(dir);
                LeaveDirection.Items.Add(dir);
            }

            foreach (var loc in Enum.GetValues(typeof(Location)).Cast<Location>())
                ToastLocation.Items.Add(loc);

            foreach (var style in Enum.GetValues(typeof(LeaveStyle)).Cast<LeaveStyle>())
                LeaveStyle.Items.Add(style);

            Settings = settings;
            if (Settings != null)
                InitValues();
        }

        public ToasterSettings Settings { get; }

        void InitValues()
        {
            MoveDuration.Text = Settings.MoveDuration.ToString();
            VerticalPadding.Text = Settings.VerticalPadding.ToString();
            VerticalAdjustment.Text = Settings.VerticalAdjustment.ToString();
            HorizontalPadding.Text = Settings.HorizontalPadding.ToString();
            HorizontalAdjustment.Text = Settings.HorizontalAdjustment.ToString();
            EnterFromDirection.SelectedItem = Settings.EnterFromDirection;
            ToastLocation.SelectedItem = Settings.EnterLocation;
            LeaveStyle.SelectedItem = Settings.LeaveStyle;
            LeaveDirection.SelectedItem = Settings.LeaveDirection;
            QueueToasts.IsChecked = Settings.QueueToasts;
        }

        void Save()
        {
            Settings.MoveDuration = int.Parse(MoveDuration.Text);
            Settings.VerticalPadding = double.Parse(VerticalPadding.Text);
            Settings.VerticalAdjustment = double.Parse(VerticalAdjustment.Text);
            Settings.HorizontalPadding = double.Parse(HorizontalPadding.Text);
            Settings.HorizontalAdjustment = double.Parse(HorizontalAdjustment.Text);
            Settings.EnterFromDirection = (Direction) EnterFromDirection.SelectedItem;
            Settings.EnterLocation = (Location) ToastLocation.SelectedItem;
            Settings.LeaveStyle = (LeaveStyle) LeaveStyle.SelectedItem;
            Settings.LeaveDirection = (Direction) LeaveDirection.SelectedItem;
            Settings.QueueToasts = QueueToasts.IsChecked ?? false;
        }

        void Save_OnClick(object sender, RoutedEventArgs e)
        {
            Save();
        }

        void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            InitValues();
        }
    }
}
