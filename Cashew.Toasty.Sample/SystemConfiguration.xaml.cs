using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cashew.Toasty.Sample
{
    /// <summary>
    /// Interaction logic for SystemConfiguration.xaml
    /// </summary>
    public partial class SystemConfiguration : UserControl
    {
        public SystemConfiguration()
        {
            InitializeComponent();
            foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>())
                FromDirection.Items.Add(dir);

            foreach (var loc in Enum.GetValues(typeof(Location)).Cast<Location>())
                ToastLocation.Items.Add(loc);

            InitValues();
        }

        void InitValues()
        {
            MoveDuration.Text = Toaster.MoveDuration.ToString();
            VerticalPadding.Text = Toaster.VerticalPadding.ToString();
            VerticalAdjustment.Text = Toaster.VerticalAdjustment.ToString();
            HorizontalPadding.Text = Toaster.HorizontalPadding.ToString();
            HorizontalAdjustment.Text = Toaster.HorizontalAdjustment.ToString();
            FromDirection.SelectedItem = Toaster.FromDirection;
            ToastLocation.SelectedItem = Toaster.ToastLocation;
        }

        void Save()
        {
            Toaster.MoveDuration = int.Parse(MoveDuration.Text);
            Toaster.VerticalPadding = double.Parse(VerticalPadding.Text);
            Toaster.VerticalAdjustment = double.Parse(VerticalAdjustment.Text);
            Toaster.HorizontalPadding = double.Parse(HorizontalPadding.Text);
            Toaster.HorizontalAdjustment = double.Parse(HorizontalAdjustment.Text);
            Toaster.FromDirection = (Direction) FromDirection.SelectedItem;
            Toaster.ToastLocation = (Location) ToastLocation.SelectedItem;
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
