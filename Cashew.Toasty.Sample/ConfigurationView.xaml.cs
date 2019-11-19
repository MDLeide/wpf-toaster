using System;
using System.Windows.Controls;

namespace Cashew.Toasty.Sample
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
            Type.Items.Add("Info");
            Type.Items.Add("Warning");
            Type.Items.Add("Error");
            Type.Items.Add("Success");
            Type.SelectedItem = "Info";
        }

        void Type_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToastConfigView toast;

            switch ((string)Type.SelectedItem)
            {
                case "Info":
                    toast = new ToastConfigView(Toast.InfoConfiguration);
                    break;
                case "Warning":
                    toast = new ToastConfigView(Toast.WarningConfiguration);
                    break;
                case "Error":
                    toast = new ToastConfigView(Toast.ErrorConfiguration);
                    break;
                case "Success":
                    toast = new ToastConfigView(Toast.SuccessConfiguration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ConfigGrid.Children.Remove(ToastConfig);
            ConfigGrid.Children.Add(toast);
        }
    }
}
