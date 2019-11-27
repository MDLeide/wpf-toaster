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
        public static ToastViewTemplate DefaultToastTemplate { get; } = GetInfoTemplate();
        public static ToastViewTemplate DefaultInfoTemplate { get; } = GetInfoTemplate();
        public static ToastViewTemplate DefaultSuccessTemplate { get; } = GetSuccessTemplate();
        public static ToastViewTemplate DefaultWarningTemplate { get; } = GetWarningTemplate();
        public static ToastViewTemplate DefaultErrorTemplate { get; } = GetErrorTemplate();


        static ToastViewTemplate GetInfoTemplate()
        {
            var template = GetBaseTemplate();
            template.StandardImage = StandardImages.InfoWhite;
            template.MessageBackgroundBrush = new SolidColorBrush(Colors.DarkSlateGray);
            template.TitleBackgroundBrush = new SolidColorBrush(Colors.DarkSlateGray);
            return template;
        }

        static ToastViewTemplate GetSuccessTemplate()
        {
            var template = GetBaseTemplate();
            template.StandardImage = StandardImages.SuccessWhite;
            template.MessageBackgroundBrush = new SolidColorBrush(Colors.Green);
            template.TitleBackgroundBrush = new SolidColorBrush(Colors.Green);
            return template;
        }

        static ToastViewTemplate GetWarningTemplate()
        {
            var template = GetBaseTemplate();
            template.StandardImage = StandardImages.WarningBlack;
            template.MessageBackgroundBrush = new SolidColorBrush(Colors.Yellow);
            template.TitleBackgroundBrush = new SolidColorBrush(Colors.Yellow);
            template.MessageForegroundBrush = new SolidColorBrush(Colors.Black);
            template.TitleForegroundBrush = new SolidColorBrush(Colors.Black);
            return template;
        }

        static ToastViewTemplate GetErrorTemplate()
        {
            var template = GetBaseTemplate();
            template.StandardImage = StandardImages.ErrorWhite;
            template.TitleBackgroundBrush = new SolidColorBrush(Colors.Red);
            template.MessageBackgroundBrush = new SolidColorBrush(Colors.Red);
            return template;
        }
        
        static ToastViewTemplate GetBaseTemplate()
        {
            var template = new ToastViewTemplate();

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
