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
                FromDirection.Items.Add(dir);
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
            FromDirection.SelectedItem = Settings.FromDirection;
            ToastLocation.SelectedItem = Settings.EnterLocation;
            InvertStacking.IsChecked = Settings.InvertStacking;
            LeaveStyle.SelectedItem = Settings.LeaveStyle;
            LeaveDirection.SelectedItem = Settings.LeaveDirection;
        }

        void Save()
        {
            Settings.MoveDuration = int.Parse(MoveDuration.Text);
            Settings.VerticalPadding = double.Parse(VerticalPadding.Text);
            Settings.VerticalAdjustment = double.Parse(VerticalAdjustment.Text);
            Settings.HorizontalPadding = double.Parse(HorizontalPadding.Text);
            Settings.HorizontalAdjustment = double.Parse(HorizontalAdjustment.Text);
            Settings.FromDirection = (Direction) FromDirection.SelectedItem;
            Settings.EnterLocation = (Location) ToastLocation.SelectedItem;
            Settings.InvertStacking = InvertStacking.IsChecked ?? false;
            Settings.LeaveStyle = (LeaveStyle) LeaveStyle.SelectedItem;
            Settings.LeaveDirection = (Direction) LeaveDirection.SelectedItem;
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
