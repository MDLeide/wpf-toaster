using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cashew.Toasty.Properties;
using Cashew.Toasty.Settings;

namespace Cashew.Toasty.Defaults
{
    static class DefaultToastViewTemplateProvider
    {
        public static ToastTemplate DefaultToastTemplate { get; } = GetInfoTemplate();
        public static ToastTemplate DefaultInfoTemplate { get; } = GetInfoTemplate();
        public static ToastTemplate DefaultSuccessTemplate { get; } = GetSuccessTemplate();
        public static ToastTemplate DefaultWarningTemplate { get; } = GetWarningTemplate();
        public static ToastTemplate DefaultErrorTemplate { get; } = GetErrorTemplate();


        static ToastTemplate GetInfoTemplate()
        {
            var template = GetBaseTemplate();
            template.ImageSource = ImageFromBitmap(Resources.final_info_icon_white);
            return template;
        }

        static ToastTemplate GetSuccessTemplate()
        {
            var template = GetBaseTemplate();
            template.ImageSource = ImageFromBitmap(Resources.final_success_icon_white);
            return template;
        }

        static ToastTemplate GetWarningTemplate()
        {
            var template = GetBaseTemplate();
            template.ImageSource = ImageFromBitmap(Resources.final_warning_icon_white);
            return template;
        }

        static ToastTemplate GetErrorTemplate()
        {
            var template = GetBaseTemplate();
            template.ImageSource = ImageFromBitmap(Resources.final_error_icon_white);
            return template;
        }
        
        static ToastTemplate GetBaseTemplate()
        {
            var template = new ToastTemplate();

            template.ImageSize = 25;

            template.ShowCloseButton = true;
            template.CloseButtonStrokeBrush = new SolidColorBrush(Colors.White);
            template.CloseButtonFillBrush = new SolidColorBrush(Colors.White);

            template.AutoWidth = false;
            template.AutoHeight = true;
            template.Width = 325;

            template.TitleBackgroundBrush = new SolidColorBrush(Colors.Green);
            template.TitleForegroundBrush = new SolidColorBrush(Colors.White);
            template.TitleFontSize = 15;
            template.TitleFontWeight = FontWeights.Bold;
            template.TitleMargin = new Thickness(8, 8, 8, 4);
            template.TitleVerticalAlignment = VerticalAlignment.Center;
            template.TitleHorizontalAlignment = HorizontalAlignment.Left;

            template.MessageBackgroundBrush = new SolidColorBrush(Colors.Green);
            template.MessageForegroundBrush = new SolidColorBrush(Colors.White);
            template.MessageFontSize = 12;
            template.MessageFontWeight = FontWeights.Normal;
            template.MessageMargin = new Thickness(8, 4, 8, 8);
            template.MessageVerticalAlignment = VerticalAlignment.Center;
            template.MessageHorizontalAlignment = HorizontalAlignment.Left;

            template.InnerBorderThickness = new Thickness(0);
            template.InnerBorderBrush = new SolidColorBrush(Colors.WhiteSmoke);
            template.CornerRadius = 3;

            template.Opacity = 1;

            return template;
        }

        static BitmapImage ImageFromBitmap(Bitmap bitmap)
        {
            using (var mem = new MemoryStream())
            {
                bitmap.Save(mem, ImageFormat.Png);
                mem.Position = 0;

                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = mem;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
    }
}
