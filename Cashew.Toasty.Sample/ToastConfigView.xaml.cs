using System.Windows;
using System.Windows.Controls;

namespace Cashew.Toasty.Sample
{
    /// <summary>
    /// Interaction logic for ToastConfigView.xaml
    /// </summary>
    public partial class ToastConfigView : UserControl
    {
        public ToastConfigView()
            : this(null) { }

        public ToastConfigView(ToastSettings config)
        {
            Configuration = config;
            InitializeComponent();
            InitValues();
        }

        
        public ToastSettings Configuration { get; }


        void InitValues()
        {
            if (Configuration == null)
                return;

            CanUserClose.IsChecked = Configuration.CanUserClose;
            CloseOnRightClick.IsChecked = Configuration.CloseOnRightClick;
            CloseAfterClickAction.IsChecked = Configuration.CloseAfterClickAction;

            Lifetime.Text = Configuration.Lifetime.ToString();
            RefreshLifetimeOnMouseOver.IsChecked = Configuration.RefreshLifetimeOnMouseOver;
            DynamicLifetime.IsChecked = Configuration.DynamicLifetime;
            DynamicLifetimeBase.Text = Configuration.DynamicLifetimeBase.ToString();
            DynamicLifetimeMillisecondsPerCharacter.Text = Configuration.DynamicLifetimeMillisecondsPerCharacter.ToString();
            DynamicLifetimeMinimum.Text = Configuration.DynamicLifetimeMinimum.ToString();
            DynamicLifetimeMaximum.Text = Configuration.DynamicLifetimeMaximum.ToString();
            FadeTime.Text = Configuration.FadeTime.ToString();
        }

        void DoSave()
        {
            Configuration.CanUserClose = CanUserClose.IsChecked ?? false;
            Configuration.CloseOnRightClick = CloseOnRightClick.IsChecked ?? false;
            Configuration.CloseAfterClickAction = CloseAfterClickAction.IsChecked ?? false;

            Configuration.Lifetime = int.Parse(Lifetime.Text);
            Configuration.RefreshLifetimeOnMouseOver = RefreshLifetimeOnMouseOver.IsChecked ?? false;

            Configuration.DynamicLifetime = DynamicLifetime.IsChecked ?? false;
            Configuration.DynamicLifetimeBase = int.Parse(DynamicLifetimeBase.Text);
            Configuration.DynamicLifetimeMillisecondsPerCharacter = int.Parse(DynamicLifetimeMillisecondsPerCharacter.Text);
            Configuration.DynamicLifetimeMinimum = int.Parse(DynamicLifetimeMinimum.Text);
            Configuration.DynamicLifetimeMaximum = int.Parse(DynamicLifetimeMaximum.Text);

            Configuration.FadeTime = int.Parse(FadeTime.Text);
        }

        void Save_OnClick(object sender, RoutedEventArgs e)
        {
            DoSave();
        }

        void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            InitValues();
        }
    }
}
