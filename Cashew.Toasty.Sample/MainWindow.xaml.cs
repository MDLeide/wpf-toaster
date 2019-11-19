using System.Windows;

namespace Cashew.Toasty.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Toaster _toaster;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => _toaster = new Toaster(this, ToasterSettings.Settings, null, ToastSettings.Settings);
        }

        

        void Show_OnClick(object sender, RoutedEventArgs e)
        {
            _toaster.Show(Message.Title, Message.Message);
        }
    }
}
