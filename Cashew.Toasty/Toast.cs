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
            GetInfoView = (t, m) => InfoTemplate?.GetToastView(t, m);
            GetWarningView = (t, m) => WarningTemplate?.GetToastView(t, m);
            GetSuccessView = (t, m) => SuccessTemplate?.GetToastView(t, m);
            GetErrorView = (t, m) => ErrorTemplate?.GetToastView(t, m);
        }

        /// <summary>
        /// A function which returns a <see cref="UIElement"/> used when rending an Info toast.
        /// </summary>
        public Func<string, string, UIElement> GetInfoView { get; set; } 

        /// <summary>
        /// A <see cref="ToastTemplate"/> used to generate a <see cref="UIElement"/> with the
        /// default layout for an Info toast when <see cref="GetInfoView"/> is not set.
        /// </summary>
        public ToastTemplate InfoTemplate { get; set; } =
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
        /// A <see cref="ToastTemplate"/> used to generate a <see cref="UIElement"/> with the
        /// default layout for a Success toast when <see cref="GetSuccessView"/> is not set.
        /// </summary>
        public ToastTemplate SuccessTemplate { get; set; } =
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
        /// A <see cref="ToastTemplate"/> used to generate a <see cref="UIElement"/> with the
        /// default layout for an Warning toast when <see cref="GetWarningView"/> is not set.
        /// </summary>
        public ToastTemplate WarningTemplate { get; set; } =
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
        /// A <see cref="ToastTemplate"/> used to generate a <see cref="UIElement"/> with the
        /// default layout for an Error toast when <see cref="GetErrorView"/> is not set.
        /// </summary>
        public ToastTemplate ErrorTemplate { get; set; } =
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
            ToasterManager.Show(
                title ?? string.Empty,
                message,
                window,
                Settings.InfoSettings,
                Settings.GetInfoView(title ?? string.Empty, message));
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
            ToasterManager.Show(
                title ?? string.Empty,
                message,
                window,
                Settings.SuccessSettings,
                Settings.GetSuccessView(title ?? string.Empty, message));
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
            ToasterManager.Show(
                title ?? string.Empty,
                message,
                window,
                Settings.ErrorSettings,
                Settings.GetErrorView(title ?? string.Empty, message));
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
            ToasterManager.Show(
                title ?? string.Empty,
                message,
                window,
                Settings.ErrorSettings,
                Settings.GetErrorView(title ?? string.Empty, message));
        }

        #endregion // Warning
    }
}
