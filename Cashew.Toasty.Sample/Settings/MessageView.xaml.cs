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
            

            ToastTitle.Text = string.IsNullOrWhiteSpace(Properties.Settings.Default.title) ? "Sample Title" : Properties.Settings.Default.title;
            ToastMessage.Text = string.IsNullOrWhiteSpace(Properties.Settings.Default.message) ? "Sample Message" : Properties.Settings.Default.message;

            ToastTitle.TextChanged += (s, e) =>
            {
                Properties.Settings.Default.title = ToastTitle.Text;
                Properties.Settings.Default.Save();
            };

            ToastMessage.TextChanged += (s, e) =>
            {
                Properties.Settings.Default.message = ToastMessage.Text;
                Properties.Settings.Default.Save();
            };
        }

        public string Title => ToastTitle.Text;
        public string Message => ToastMessage.Text;
    }
}
