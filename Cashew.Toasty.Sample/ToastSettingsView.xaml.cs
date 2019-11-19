using System.Windows;
using System.Windows.Controls;
using Cashew.Toasty.Config;

namespace Cashew.Toasty.Sample
{
    /// <summary>
    /// Interaction logic for ToastConfigView.xaml
    /// </summary>
    public partial class ToastSettingsView : UserControl
    {
        public ToastSettingsView()
            : this(Defaults.DefaultSettings.DefaultToastSettings) { }

        public ToastSettingsView(ToastSettings settings)
        {
            Settings = settings;
            InitializeComponent();
            InitValues();
        }

        
        public ToastSettings Settings { get; }


        void InitValues()
        {
            if (Settings == null)
                return;

            CanUserClose.IsChecked = Settings.CanUserClose;
            CloseOnRightClick.IsChecked = Settings.CloseOnRightClick;
            CloseAfterClickAction.IsChecked = Settings.CloseAfterClickAction;

            Lifetime.Text = Settings.Lifetime.ToString();
            RefreshLifetimeOnMouseOver.IsChecked = Settings.RefreshLifetimeOnMouseOver;
            DynamicLifetime.IsChecked = Settings.DynamicLifetime;
            DynamicLifetimeBase.Text = Settings.DynamicLifetimeBase.ToString();
            DynamicLifetimeMillisecondsPerCharacter.Text = Settings.DynamicLifetimeMillisecondsPerCharacter.ToString();
            DynamicLifetimeMinimum.Text = Settings.DynamicLifetimeMinimum.ToString();
            DynamicLifetimeMaximum.Text = Settings.DynamicLifetimeMaximum.ToString();
            FadeTime.Text = Settings.LeaveTime.ToString();
        }

        void DoSave()
        {
            Settings.CanUserClose = CanUserClose.IsChecked ?? false;
            Settings.CloseOnRightClick = CloseOnRightClick.IsChecked ?? false;
            Settings.CloseAfterClickAction = CloseAfterClickAction.IsChecked ?? false;

            Settings.Lifetime = int.Parse(Lifetime.Text);
            Settings.RefreshLifetimeOnMouseOver = RefreshLifetimeOnMouseOver.IsChecked ?? false;

            Settings.DynamicLifetime = DynamicLifetime.IsChecked ?? false;
            Settings.DynamicLifetimeBase = int.Parse(DynamicLifetimeBase.Text);
            Settings.DynamicLifetimeMillisecondsPerCharacter = int.Parse(DynamicLifetimeMillisecondsPerCharacter.Text);
            Settings.DynamicLifetimeMinimum = int.Parse(DynamicLifetimeMinimum.Text);
            Settings.DynamicLifetimeMaximum = int.Parse(DynamicLifetimeMaximum.Text);

            Settings.LeaveTime = int.Parse(FadeTime.Text);
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
