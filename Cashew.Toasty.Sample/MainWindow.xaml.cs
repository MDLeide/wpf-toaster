using System.Windows;

namespace Cashew.Toasty.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Types.Items.Add("Info");
            Types.Items.Add("Success");
            Types.Items.Add("Warning");
            Types.Items.Add("Error");
            Types.SelectedIndex = 0;

            ToastTitle.Text = "Sample Title";
            ToastMessage.Text = "Sample Message";
        }

        void Show_OnClick(object sender, RoutedEventArgs e)
        {
            switch ((string)Types.SelectedItem)
            {
                case "Info":
                    Toast.Info(this, ToastMessage.Text, ToastTitle.Text);
                    break;
                case "Warning":
                    Toast.Warning(this, ToastMessage.Text, ToastTitle.Text);
                    break;
                case "Success":
                    Toast.Success(this, ToastMessage.Text, ToastTitle.Text);
                    break;
                case "Error":
                    Toast.Error(this, ToastMessage.Text, ToastTitle.Text);
                    break;
            }
        }
    }
}
