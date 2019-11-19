//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Windows;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using Cashew.Toasty.Config;
//using Cashew.Toasty.Properties;

//namespace Cashew.Toasty
//{
//    //todo: choose toast location
//    //todo: choose toast direction

//    public static class Toast
//    {
//        public static Func<object, Window> DefaultGetWindowFromContextFunction { get; set; } = o => null;

//        /// <summary>
//        /// A function which returns a <see cref="UIElement"/> used when rending an Info toast.
//        /// </summary>
//        public static Func<string, string, UIElement> GetInfoView { get; set; } =
//            (t, m) => InfoTemplate?.GetToastView(t, m);

//        /// <summary>
//        /// A <see cref="ToastTemplate"/> used to generate a <see cref="UIElement"/> with the
//        /// default layout for an Info toast when <see cref="GetInfoView"/> is not set.
//        /// </summary>
//        public static ToastTemplate InfoTemplate { get; set; } = Defaults.InfoTemplate;

//        /// <summary>
//        /// A <see cref="ToastSettings"/> used to configure presentation details of an Info toast.
//        /// </summary>
//        public static ToastSettings InfoConfiguration { get; set; } = Defaults.InfoConfiguration;


//        /// <summary>
//        /// A function which returns a <see cref="UIElement"/> used when rending a Success toast.
//        /// </summary>
//        public static Func<string, string, UIElement> GetSuccessView { get; set; } =
//            (t, m) => SuccessTemplate?.GetToastView(t, m);
        
//        /// <summary>
//        /// A <see cref="ToastTemplate"/> used to generate a <see cref="UIElement"/> with the
//        /// default layout for a Success toast when <see cref="GetSuccessView"/> is not set.
//        /// </summary>
//        public static ToastTemplate SuccessTemplate { get; set; } = Defaults.SuccessTemplate;

//        /// <summary>
//        /// A <see cref="ToastSettings"/> used to configure presentation details of a Success toast.
//        /// </summary>
//        public static ToastSettings SuccessConfiguration { get; set; } = Defaults.SuccessConfiguration;


//        /// <summary>
//        /// A function which returns a <see cref="UIElement"/> used when rending a Warning toast.
//        /// </summary>
//        public static Func<string, string, UIElement> GetWarningView { get; set; } =
//            (t, m) => WarningTemplate?.GetToastView(t, m);

//        /// <summary>
//        /// A <see cref="ToastTemplate"/> used to generate a <see cref="UIElement"/> with the
//        /// default layout for an Warning toast when <see cref="GetWarningView"/> is not set.
//        /// </summary>
//        public static ToastTemplate WarningTemplate { get; set; } = Defaults.WarningTemplate;

//        /// <summary>
//        /// A <see cref="ToastSettings"/> used to configure presentation details of a Warning toast.
//        /// </summary>
//        public static ToastSettings WarningConfiguration { get; set; } = Defaults.WarningConfiguration;


//        /// <summary>
//        /// A function which returns a <see cref="UIElement"/> used when rending an Error toast.
//        /// </summary>
//        public static Func<string, string, UIElement> GetErrorView { get; set; } =
//            (t, m) => ErrorTemplate?.GetToastView(t, m);

//        /// <summary>
//        /// A <see cref="ToastTemplate"/> used to generate a <see cref="UIElement"/> with the
//        /// default layout for an Error toast when <see cref="GetErrorView"/> is not set.
//        /// </summary>
//        public static ToastTemplate ErrorTemplate { get; set; } = Defaults.ErrorTemplate;

//        /// <summary>
//        /// A <see cref="ToastSettings"/> used to configure presentation details of an Error toast.
//        /// </summary>
//        public static ToastSettings ErrorConfiguration { get; set; } = Defaults.ErrorConfiguration;


//        #region Info
        
//        public static void Info(object context, string message, string title = null, Func<object, Window> getWindow = null)
//        {
//            if (getWindow == null)
//                getWindow = DefaultGetWindowFromContextFunction;

//            Info(getWindow(context), message, title);
//        }

//        public static void Info(Window window, string message, string title = null)
//        {
//            ToasterManager.Show(
//                title ?? string.Empty,
//                message,
//                window,
//                InfoConfiguration,
//                GetInfoView(title ?? string.Empty, message));
//        }

//        #endregion // Info

//        #region Success

//        public static void Success(object context, string message, string title = null, Func<object, Window> getWindow = null)
//        {
//            if (getWindow == null)
//                getWindow = DefaultGetWindowFromContextFunction;

//            Success(getWindow(context), message, title);
//        }

//        public static void Success(Window window, string message, string title = null)
//        {
//            ToasterManager.Show(
//                title ?? string.Empty,
//                message,
//                window,
//                SuccessConfiguration,
//                GetSuccessView(title ?? string.Empty, message));
//        }

//        #endregion // Success

//        #region Error

//        public static void Error(object context, string message, string title = null, Func<object, Window> getWindow = null)
//        {
//            if (getWindow == null)
//                getWindow = DefaultGetWindowFromContextFunction;

//            Error(getWindow(context), message, title);
//        }

//        public static void Error(Window window, string message, string title = null)
//        {
//            ToasterManager.Show(
//                title ?? string.Empty,
//                message,
//                window,
//                ErrorConfiguration,
//                GetErrorView(title ?? string.Empty, message));
//        }

//        #endregion // Error

//        #region Warning

//        public static void Warning(object context, string title, string message = null, Func<object, Window> getWindow = null)
//        {
//            if (getWindow == null)
//                getWindow = DefaultGetWindowFromContextFunction;

//            Warning(getWindow(context), message, title);
//        }
        
//        public static void Warning(Window window, string message, string title = null)
//        {
//            ToasterManager.Show(
//                title ?? string.Empty,
//                message,
//                window,
//                ErrorConfiguration,
//                GetErrorView(title ?? string.Empty, message));
//        }

//        #endregion // Warning

//        static class Defaults
//        {
//            public static ToastTemplate InfoTemplate { get; } = GetInfoTemplate();
//            public static ToastSettings InfoConfiguration { get; } = GetBaseConfiguration();

//            public static ToastTemplate SuccessTemplate { get; } = GetSuccessTemplate();
//            public static ToastSettings SuccessConfiguration { get; } = GetBaseConfiguration();

//            public static ToastTemplate WarningTemplate { get; } = GetWarningTemplate();
//            public static ToastSettings WarningConfiguration { get; } = GetBaseConfiguration();

//            public static ToastTemplate ErrorTemplate { get; } = GetErrorTemplate();
//            public static ToastSettings ErrorConfiguration { get; } = GetBaseConfiguration();

//            static BitmapImage ImageFromBitmap(Bitmap bitmap)
//            {
//                using (var mem = new MemoryStream())
//                {
//                    bitmap.Save(mem, ImageFormat.Png);
//                    mem.Position = 0;

//                    var image = new BitmapImage();
//                    image.BeginInit();
//                    image.StreamSource = mem;
//                    image.CacheOption = BitmapCacheOption.OnLoad;
//                    image.EndInit();
//                    image.Freeze();
//                    return image;
//                }
//            }

//            static ToastTemplate GetInfoTemplate()
//            {
//                var template = GetBaseTemplate();
//                template.ImageSource = ImageFromBitmap(Resources.final_info_icon_white);
//                return template;
//            }

//            static ToastTemplate GetSuccessTemplate()
//            {
//                var template = GetBaseTemplate();
//                template.ImageSource = ImageFromBitmap(Resources.final_success_icon_white);
//                return template;
//            }

//            static ToastTemplate GetWarningTemplate()
//            {
//                var template = GetBaseTemplate();
//                template.ImageSource = ImageFromBitmap(Resources.final_warning_icon_white);
//                return template;
//            }

//            static ToastTemplate GetErrorTemplate()
//            {
//                var template = GetBaseTemplate();
//                template.ImageSource = ImageFromBitmap(Resources.final_error_icon_white);
//                return template;
//            }

//            static ToastSettings GetBaseConfiguration()
//            {
//                var config = new ToastSettings();
//                config.CanUserClose = true;
//                config.CloseOnRightClick = true;
//                config.CloseAfterClickAction = true;
//                config.Lifetime = 2500;
//                config.RefreshLifetimeOnMouseOver = true;
//                config.LeaveTime = 500;
//                return config;
//            }

//            static ToastTemplate GetBaseTemplate()
//            {
//                var template = new ToastTemplate();

//                template.ImageSize = 25;

//                template.ShowCloseButton = true;
//                template.CloseButtonStrokeBrush = new SolidColorBrush(Colors.White);
//                template.CloseButtonFillBrush = new SolidColorBrush(Colors.White);

//                template.AutoWidth = false;
//                template.AutoHeight = true;
//                template.Width = 325;

//                template.TitleBackgroundBrush = new SolidColorBrush(Colors.Green);
//                template.TitleForegroundBrush = new SolidColorBrush(Colors.White);
//                template.TitleFontSize = 15;
//                template.TitleFontWeight = FontWeights.Bold;
//                template.TitleMargin = new Thickness(8, 8, 8, 4);
//                template.TitleVerticalAlignment = VerticalAlignment.Center;
//                template.TitleHorizontalAlignment = HorizontalAlignment.Left;

//                template.MessageBackgroundBrush = new SolidColorBrush(Colors.Green);
//                template.MessageForegroundBrush = new SolidColorBrush(Colors.White);
//                template.MessageFontSize = 12;
//                template.MessageFontWeight = FontWeights.Normal;
//                template.MessageMargin = new Thickness(8, 4, 8, 8);
//                template.MessageVerticalAlignment = VerticalAlignment.Center;
//                template.MessageHorizontalAlignment = HorizontalAlignment.Left;

//                template.InnerBorderThickness = new Thickness(0);
//                template.InnerBorderBrush = new SolidColorBrush(Colors.WhiteSmoke);
//                template.CornerRadius = 3;

//                template.Opacity = 1;

//                return template;
//            }
//        }
//    }
//}
