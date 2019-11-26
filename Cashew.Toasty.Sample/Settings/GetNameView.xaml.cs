using System.Windows;
using System.Windows.Controls;

namespace Cashew.Toasty.Sample.Settings
{
    /// <summary>
    /// Interaction logic for GetNameView.xaml
    /// </summary>
    public partial class GetNameView : Window
    {
        public GetNameView()
        {
            InitializeComponent();
            Okay.IsEnabled = false;
        }

        void Name_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Name.Text) || Name.Text.ToLower() == "default")
                Okay.IsEnabled = false;
            else
                Okay.IsEnabled = true;

            SelectedName = Name.Text;
        }

        public string SelectedName { get; private set; }

        void Okay_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedName = null;
            Close();
        }
    }
}
