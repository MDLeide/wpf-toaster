using System.Windows.Controls;

namespace Cashew.Toasty.Sample.Settings
{
    /// <summary>
    /// Interaction logic for MessageView.xaml
    /// </summary>
    public partial class MessageView : UserControl
    {
        public MessageView()
        {
            InitializeComponent();
            Types.Items.Add("Custom");
            Types.Items.Add("Info");
            Types.Items.Add("Success");
            Types.Items.Add("Warning");
            Types.Items.Add("Error");
            Types.SelectedIndex = 0;

            ToastTitle.Text = "Sample Title";
            ToastMessage.Text = "Sample Message";
        }

        public string Title => ToastTitle.Text;
        public string Message => ToastMessage.Text;
        public string SelectedType => (string) Types.SelectedItem;
    }
}
