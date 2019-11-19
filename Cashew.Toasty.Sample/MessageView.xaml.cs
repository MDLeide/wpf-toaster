using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cashew.Toasty.Sample
{
    /// <summary>
    /// Interaction logic for MessageView.xaml
    /// </summary>
    public partial class MessageView : UserControl
    {
        public MessageView()
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

        public string Title => ToastTitle.Text;
        public string Message => ToastMessage.Text;
        public string SelectedType => (string) Types.SelectedItem;
    }
}
