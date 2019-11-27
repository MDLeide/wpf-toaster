using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cashew.Toasty.Defaults;
using Cashew.Toasty.Properties;
using Cashew.Toasty.Settings;

namespace Cashew.Toasty
{
    //todo: choose toast location
    //todo: choose toast direction

    public class ToastSettings
    {
        public ToastSettings()
        {
            GetInfoView = (m, t) => InfoTemplate?.GetToastView(m, t);
            GetWarningView = (m, t) => WarningTemplate?.GetToastView(m, t);
            GetSuccessView = (m, t) => SuccessTemplate?.GetToastView(m, t);
            GetErrorView = (m, t) => ErrorTemplate?.GetToastView(m, t);
        }

        /// <summary>
        /// A function which returns a <see cref="UIElement"/> used when rending an Info toast.
        /// </summary>
        public Func<string, string, UIElement> GetInfoView { get; set; } 

        /// <summary>
        /// A <see cref="ToastViewTemplate"/> used to generate a <see cref="UIElement"/> with the
        /// default layout for an Info toast when <see cref="GetInfoView"/> is not set.
        /// </summary>
        public ToastViewTemplate InfoTemplate { get; set; } =
            DefaultSettings.DefaultInfoToastViewTemplate;

        /// <summary>
        /// A <see cref="ToastSettings"/> used to configure presentation details of an Info toast.
        /// </summary>
        public ToastAdornerSettings InfoSettings { get; set; } =
            DefaultSettings.DefaultInfoToastSettings;


        /// <summary>
        /// A function which returns a <see cref="UIElement"/> used when rending a Success toast.
        /// </summary>
        public Func<string, string, UIElement> GetSuccessView { get; set; } 

        /// <summary>
        /// A <see cref="ToastViewTemplate"/> used to generate a <see cref="UIElement"/> with the
        /// default layout for a Success toast when <see cref="GetSuccessView"/> is not set.
        /// </summary>
        public ToastViewTemplate SuccessTemplate { get; set; } =
            DefaultSettings.DefaultSuccessToastViewTemplate;

        /// <summary>
        /// A <see cref="ToastSettings"/> used to configure presentation details of a Success toast.
        /// </summary>
        public ToastAdornerSettings SuccessSettings { get; set; } =
            DefaultSettings.DefaultSuccessToastSettings;


        /// <summary>
        /// A function which returns a <see cref="UIElement"/> used when rending a Warning toast.
        /// </summary>
        public Func<string, string, UIElement> GetWarningView { get; set; } 

        /// <summary>
        /// A <see cref="ToastViewTemplate"/> used to generate a <see cref="UIElement"/> with the
        /// default layout for an Warning toast when <see cref="GetWarningView"/> is not set.
        /// </summary>
        public ToastViewTemplate WarningTemplate { get; set; } =
            DefaultSettings.DefaultWarningToastViewTemplate;

        /// <summary>
        /// A <see cref="ToastSettings"/> used to configure presentation details of a Warning toast.
        /// </summary>
        public ToastAdornerSettings WarningSettings { get; set; } =
            DefaultSettings.DefaultWarningToastSettings;


        /// <summary>
        /// A function which returns a <see cref="UIElement"/> used when rending an Error toast.
        /// </summary>
        public Func<string, string, UIElement> GetErrorView { get; set; } 

        /// <summary>
        /// A <see cref="ToastViewTemplate"/> used to generate a <see cref="UIElement"/> with the
        /// default layout for an Error toast when <see cref="GetErrorView"/> is not set.
        /// </summary>
        public ToastViewTemplate ErrorTemplate { get; set; } =
            DefaultSettings.DefaultErrorToastViewTemplate;

        /// <summary>
        /// A <see cref="ToastSettings"/> used to configure presentation details of an Error toast.
        /// </summary>
        public ToastAdornerSettings ErrorSettings { get; set; } =
            DefaultSettings.DefaultErrorToastSettings;
    }

    public static class Toast
    {
        static ToasterManager ToasterManager { get; set; } = new ToasterManager(ToasterSettings);

        public static Func<object, Window> DefaultGetWindowFromContextFunction { get; set; } = o => null;

        static ToasterSettings _toasterSettings = Cashew.Toasty.Defaults.DefaultSettings.DefaultToasterSettings;
        public static ToasterSettings ToasterSettings
        {
            get { return _toasterSettings; }
            set
            {
                _toasterSettings = value;
                ToasterManager = new ToasterManager(value);
            }
        }

        public static ToastSettings Settings { get; } = new ToastSettings();


        #region Info

        public static void Info(object context, string message, string title = null, Func<object, Window> getWindow = null)
        {
            if (getWindow == null)
                getWindow = DefaultGetWindowFromContextFunction;

            Info(getWindow(context), message, title);
        }

        public static void Info(Window window, string message, string title = null)
        {
            Show(
                message,
                title,
                window,
                Settings.InfoSettings, GetUiElement(message, title, Settings.GetInfoView));
        }

        #endregion // Info

        #region Success

        public static void Success(object context, string message, string title = null, Func<object, Window> getWindow = null)
        {
            if (getWindow == null)
                getWindow = DefaultGetWindowFromContextFunction;

            Success(getWindow(context), message, title);
        }

        public static void Success(Window window, string message, string title = null)
        {
            Show(
                message,
                title,
                window,
                Settings.SuccessSettings, GetUiElement(message, title, Settings.GetSuccessView));
        }

        #endregion // Success

        #region Error

        public static void Error(object context, string message, string title = null, Func<object, Window> getWindow = null)
        {
            if (getWindow == null)
                getWindow = DefaultGetWindowFromContextFunction;

            Error(getWindow(context), message, title);
        }

        public static void Error(Window window, string message, string title = null)
        {
            Show(
                message,
                title,
                window,
                Settings.ErrorSettings, GetUiElement(message, title, Settings.GetErrorView));
        }

        #endregion // Error

        #region Warning

        public static void Warning(object context, string title, string message = null, Func<object, Window> getWindow = null)
        {
            if (getWindow == null)
                getWindow = DefaultGetWindowFromContextFunction;

            Warning(getWindow(context), message, title);
        }

        public static void Warning(Window window, string message, string title = null)
        {
            Show(
                message,
                title,
                window,
                Settings.WarningSettings, GetUiElement(message, title, Settings.GetWarningView));
        }

        #endregion // Warning

        static void Show(
            string message, 
            string title, 
            Window window, 
            ToastAdornerSettings settings,
            UIElement uiElement)
        {
            var adorner = ToasterManager.Show(
                message ?? string.Empty,
                title ?? string.Empty,
                window,
                settings, 
                uiElement);

            if (adorner.ToastView is ToastView tv)
                tv.CloseButtonClicked += (s, e) => adorner.RequestClose();
        }

        delegate void ActionDelegate();

        static UIElement GetUiElement(string message, string title, Func<string, string, UIElement> func)
        {
            UIElement element = null;

            void GetElement()
            {
                element = func(message, title);
            }

            Application.Current?.Dispatcher?.Invoke((ActionDelegate)GetElement);
            return element;
        }
    }
}
